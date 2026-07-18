using CorporateWebsite.Core.DTOs;
using CorporateWebsite.Core.Entities;
using CorporateWebsite.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace CorporateWebsite.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly ISettingService _settingService;

    public EmailService(IUnitOfWork unitOfWork, IConfiguration configuration, ISettingService settingService)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _settingService = settingService;
    }

    public async Task SendAsync(string toEmail, string subject, string bodyHtml, string? bodyText = null, string? fromEmail = null, string? fromName = null, string? cc = null, string? bcc = null, IEnumerable<string>? attachments = null)
    {
        var message = new MimeMessage();
        
        // From
        fromEmail ??= await _settingService.GetValueAsync("SmtpFromEmail") ?? _configuration["Email:FromEmail"];
        fromName ??= await _settingService.GetValueAsync("SmtpFromName") ?? _configuration["Email:FromName"] ?? "Corporate Website";
        
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        
        // To
        message.To.Add(new MailboxAddress("", toEmail));
        
        // CC
        if (!string.IsNullOrEmpty(cc))
        {
            foreach (var addr in cc.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                message.Cc.Add(new MailboxAddress("", addr));
            }
        }
        
        // BCC
        if (!string.IsNullOrEmpty(bcc))
        {
            foreach (var addr in bcc.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                message.Bcc.Add(new MailboxAddress("", addr));
            }
        }
        
        // Subject
        message.Subject = subject;
        
        // Body
        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = bodyHtml,
            TextBody = bodyText ?? StripHtml(bodyHtml)
        };
        
        // Attachments
        if (attachments != null)
        {
            foreach (var attachment in attachments)
            {
                if (File.Exists(attachment))
                {
                    bodyBuilder.Attachments.Add(attachment);
                }
            }
        }
        
        message.Body = bodyBuilder.ToMessageBody();
        
        await SendMessageAsync(message);
    }

    public async Task SendTemplateAsync(string templateName, string toEmail, Dictionary<string, string> variables, string languageCode = "fa", string? fromEmail = null, string? fromName = null)
    {
        var template = await _unitOfWork.EmailTemplates.FirstOrDefaultAsync(
            t => t.Name == templateName && t.IsActive,
            t => t.Translations);
        
        if (template == null)
        {
            throw new KeyNotFoundException($"Email template '{templateName}' not found.");
        }

        var translation = template.Translations.FirstOrDefault(t => t.Language.Code == languageCode) 
            ?? template.Translations.FirstOrDefault();
        
        if (translation == null)
        {
            throw new KeyNotFoundException($"Email template '{templateName}' has no translation for language '{languageCode}'.");
        }

        var subject = ReplaceVariables(translation.Subject, variables);
        var bodyHtml = ReplaceVariables(translation.BodyHtml, variables);
        var bodyText = string.IsNullOrEmpty(translation.BodyText) ? null : ReplaceVariables(translation.BodyText, variables);
        
        await SendAsync(toEmail, subject, bodyHtml, bodyText, fromEmail, fromName);
    }

    public async Task QueueEmailAsync(EmailQueueDto dto)
    {
        var emailQueue = new EmailQueue
        {
            ToEmail = dto.ToEmail,
            ToName = dto.ToName,
            FromEmail = dto.FromEmail,
            FromName = dto.FromName,
            Subject = dto.Subject,
            BodyHtml = dto.BodyHtml,
            BodyText = dto.BodyText,
            Cc = dto.Cc,
            Bcc = dto.Bcc,
            Attachments = dto.Attachments != null ? JsonSerializer.Serialize(dto.Attachments) : null,
            Priority = dto.Priority,
            Status = "Pending",
            ScheduledAt = dto.ScheduledAt,
            RelatedEntityId = dto.RelatedEntityId,
            RelatedEntityType = dto.RelatedEntityType,
            MaxRetries = dto.MaxRetries > 0 ? dto.MaxRetries : 3,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.EmailQueue.AddAsync(emailQueue);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ProcessQueueAsync(int batchSize = 50)
    {
        var pendingEmails = await _unitOfWork.EmailQueue.GetAllAsync(
            e => e.Status == "Pending" && (e.ScheduledAt == null || e.ScheduledAt <= DateTime.UtcNow),
            q => q.OrderBy(e => e.Priority).ThenBy(e => e.CreatedAt).Take(batchSize));

        foreach (var email in pendingEmails)
        {
            email.Status = "Sending";
            email.RetryCount++;
            await _unitOfWork.EmailQueue.UpdateAsync(email);
            
            try
            {
                var attachments = !string.IsNullOrEmpty(email.Attachments) 
                    ? JsonSerializer.Deserialize<string[]>(email.Attachments) 
                    : null;
                
                await SendAsync(
                    email.ToEmail, 
                    email.Subject, 
                    email.BodyHtml, 
                    email.BodyText, 
                    email.FromEmail, 
                    email.FromName, 
                    email.Cc, 
                    email.Bcc, 
                    attachments);
                
                email.Status = "Sent";
                email.SentAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                email.Status = email.RetryCount >= email.MaxRetries ? "Failed" : "Pending";
                email.ErrorMessage = ex.Message;
                
                if (email.Status == "Failed")
                {
                    // Log error
                }
            }
            
            await _unitOfWork.EmailQueue.UpdateAsync(email);
        }
        
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<EmailQueueDto>> GetFailedEmailsAsync(int maxRetries = 3)
    {
        var emails = await _unitOfWork.EmailQueue.GetAllAsync(
            e => e.Status == "Failed" && e.RetryCount < maxRetries,
            q => q.OrderBy(e => e.CreatedAt));
        
        return _mapper.Map<IReadOnlyList<EmailQueueDto>>(emails);
    }

    public async Task RetryFailedEmailsAsync()
    {
        var failedEmails = await _unitOfWork.EmailQueue.GetAllAsync(
            e => e.Status == "Failed",
            q => q.OrderBy(e => e.CreatedAt));

        foreach (var email in failedEmails)
        {
            email.Status = "Pending";
            email.RetryCount = 0;
            email.ErrorMessage = null;
            await _unitOfWork.EmailQueue.UpdateAsync(email);
        }
        
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task SendMessageAsync(MimeMessage message)
    {
        var host = await _settingService.GetValueAsync("SmtpHost") ?? _configuration["Email:SmtpHost"];
        var port = await _settingService.GetValueAsync<int>("SmtpPort") ?? _configuration.GetValue<int>("Email:SmtpPort", 587);
        var username = await _settingService.GetValueAsync("SmtpUsername") ?? _configuration["Email:SmtpUsername"];
        var password = await _settingService.GetValueAsync("SmtpPassword") ?? _configuration["Email:SmtpPassword"];
        var enableSsl = await _settingService.GetValueAsync<bool>("SmtpEnableSsl") ?? _configuration.GetValue<bool>("Email:SmtpEnableSsl", true);

        if (string.IsNullOrEmpty(host))
        {
            throw new InvalidOperationException("SMTP host not configured.");
        }

        using var client = new SmtpClient();
        
        try
        {
            if (enableSsl)
            {
                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            }
            else
            {
                await client.ConnectAsync(host, port, SecureSocketOptions.None);
            }
            
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                await client.AuthenticateAsync(username, password);
            }
            
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
        }
    }

    private string StripHtml(string html)
    {
        return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);
    }

    private string ReplaceVariables(string template, Dictionary<string, string> variables)
    {
        var result = template;
        foreach (var kvp in variables)
        {
            result = result.Replace($"{{{kvp.Key}}}", kvp.Value);
            result = result.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
        }
        return result;
    }
}