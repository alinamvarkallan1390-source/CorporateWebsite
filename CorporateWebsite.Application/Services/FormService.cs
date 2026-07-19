using CorporateWebsite.Core.DTOs;
using System.Linq.Expressions;
using CorporateWebsite.Core.Entities;
using CorporateWebsite.Core.Interfaces;
using CorporateWebsite.Application.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace CorporateWebsite.Application.Services;

public class FormService : IFormService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILanguageService _languageService;
    private readonly IEmailService _emailService;
    private readonly ISettingService _settingService;

    public FormService(IUnitOfWork unitOfWork, IMapper mapper, ILanguageService languageService, IEmailService emailService, ISettingService settingService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _languageService = languageService;
        _emailService = emailService;
        _settingService = settingService;
    }

    public async Task<FormDto?> GetByIdAsync(int id, string? languageCode = null)
    {
        var form = await _unitOfWork.Forms.GetByIdAsync(id, f => f.Translations, f => f.Fields);
        if (form == null) return null;

        var dto = _mapper.Map<FormDto>(form);
        await SetTranslationAsync(dto, languageCode);
        
        return dto;
    }

    public async Task<FormDto?> GetBySlugAsync(string slug, string languageCode)
    {
        var language = await _languageService.GetByCodeAsync(languageCode);
        if (language == null) return null;

        var translation = await _unitOfWork.FormTranslations.FirstOrDefaultAsync(
            t => t.Slug == slug && t.LanguageId == language.Id,
            t => t.Form);
        
        if (translation?.Form == null) return null;

        var form = translation.Form;
        var dto = _mapper.Map<FormDto>(form);
        dto.Translation = _mapper.Map<FormTranslationDto>(translation);
        
        return dto;
    }

    public async Task<FormDto?> GetByPageAsync(int pageId, string languageCode)
    {
        var language = await _languageService.GetByCodeAsync(languageCode);
        if (language == null) return null;

        var form = await _unitOfWork.Forms.FirstOrDefaultAsync(
            f => f.PageId == pageId && f.IsActive,
            f => f.Translations, f => f.Fields);
        
        if (form == null) return null;

        var dto = _mapper.Map<FormDto>(form);
        await SetTranslationAsync(dto, languageCode);
        
        return dto;
    }

    public async Task<IReadOnlyList<FormDto>> GetAllAsync(string? languageCode = null, bool? isActive = null)
    {
        Expression<Func<Form, bool>> predicate = f => true;
        
        if (isActive.HasValue)
        {
            predicate = CombinePredicates(predicate, f => f.IsActive == isActive.Value);
        }

        var forms = await _unitOfWork.Forms.GetAllAsync(predicate, q => q.OrderBy(f => f.Name), f => f.Translations, f => f.Fields);
        var dtos = _mapper.Map<IReadOnlyList<FormDto>>(forms);
        await SetTranslationsAsync(dtos, languageCode);
        
        return dtos;
    }

    public async Task<FormDto> CreateAsync(CreateFormDto dto)
    {
        var form = _mapper.Map<Form>(dto);
        form.CreatedAt = DateTime.UtcNow;
        
        foreach (var transDto in dto.Translations)
        {
            var translation = _mapper.Map<FormTranslation>(transDto);
            translation.Form = form;
            form.Translations.Add(translation);
        }

        foreach (var fieldDto in dto.Fields)
        {
            var field = _mapper.Map<FormField>(fieldDto);
            field.Form = form;
            foreach (var transDto in fieldDto.Translations)
            {
                var translation = _mapper.Map<FormFieldTranslation>(transDto);
                translation.Field = field;
                field.Translations.Add(translation);
            }
            form.Fields.Add(field);
        }

        await _unitOfWork.Forms.AddAsync(form);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<FormDto>(form);
    }

    public async Task<FormDto> UpdateAsync(int id, UpdateFormDto dto)
    {
        var form = await _unitOfWork.Forms.GetByIdAsync(id, f => f.Translations, f => f.Fields);
        if (form == null)
        {
            throw new KeyNotFoundException($"Form with id {id} not found.");
        }

        _mapper.Map(dto, form);
        form.UpdatedAt = DateTime.UtcNow;

        await UpdateTranslationsAsync(form.Translations, dto.Translations, (t, d) => _mapper.Map(d, t));

        await UpdateFieldsAsync(form.Fields, dto.Fields);

        await _unitOfWork.Forms.UpdateAsync(form);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<FormDto>(form);
    }

    public async Task DeleteAsync(int id)
    {
        var form = await _unitOfWork.Forms.GetByIdAsync(id);
        if (form == null)
        {
            throw new KeyNotFoundException($"Form with id {id} not found.");
        }

        await _unitOfWork.Forms.DeleteAsync(form);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<FormSubmissionDto> SubmitAsync(SubmitFormDto dto, string ipAddress, string userAgent, string referrerUrl, int languageId)
    {
        var form = await _unitOfWork.Forms.GetByIdAsync(dto.FormId, f => f.Fields, f => f.Translations);
        if (form == null)
        {
            throw new KeyNotFoundException($"Form with id {dto.FormId} not found.");
        }

        if (!form.IsActive)
        {
            throw new InvalidOperationException("Form is not active.");
        }

        // Validate reCAPTCHA if required
        if (form.RequireCaptcha && !string.IsNullOrEmpty(dto.RecaptchaToken))
        {
            var isValid = await ValidateRecaptchaAsync(dto.RecaptchaToken);
            if (!isValid)
            {
                throw new InvalidOperationException("Invalid reCAPTCHA.");
            }
        }

        // Check rate limiting
        if (form.RateLimitCount.HasValue && form.RateLimitWindowMinutes.HasValue)
        {
            var recentSubmissions = await _unitOfWork.FormSubmissions.CountAsync(
                s => s.FormId == form.Id && s.IpAddress == ipAddress && s.CreatedAt >= DateTime.UtcNow.AddMinutes(-form.RateLimitWindowMinutes.Value));
            
            if (recentSubmissions >= form.RateLimitCount.Value)
            {
                throw new InvalidOperationException("Too many submissions from this IP. Please try again later.");
            }
        }

        // Check multiple submissions
        if (!form.AllowMultipleSubmissions)
        {
            var existingSubmission = await _unitOfWork.FormSubmissions.FirstOrDefaultAsync(
                s => s.FormId == form.Id && s.IpAddress == ipAddress);
            
            if (existingSubmission != null)
            {
                throw new InvalidOperationException("You have already submitted this form.");
            }
        }

        // Validate fields
        await ValidateSubmissionAsync(dto, form.Id);

        // Create submission
        var submission = new FormSubmission
        {
            FormId = form.Id,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            ReferrerUrl = referrerUrl,
            LanguageId = languageId,
            Status = "New",
            CreatedAt = DateTime.UtcNow
        };

        // Process field values
        foreach (var field in form.Fields.Where(f => f.IsVisible))
        {
            if (dto.FieldValues.TryGetValue(field.Name, out var value))
            {
                submission.FieldValues.Add(new FormFieldValue
                {
                    FieldId = field.Id,
                    Value = value
                });
            }
            else if (field.IsRequired)
            {
                throw new InvalidOperationException($"Required field '{field.Label}' is missing.");
            }
        }

        await _unitOfWork.FormSubmissions.AddAsync(submission);
        await _unitOfWork.SaveChangesAsync();

        // Send notification emails
        await SendNotificationEmailsAsync(submission, form);

        // Send confirmation email
        if (form.SendConfirmationEmail)
        {
            await SendConfirmationEmailAsync(submission, form);
        }

        return _mapper.Map<FormSubmissionDto>(submission);
    }

    public async Task<FormSubmissionDto?> GetSubmissionByIdAsync(int id)
    {
        var submission = await _unitOfWork.FormSubmissions.GetByIdAsync(id, s => s.FieldValues, s => s.Files, s => s.Form);
        if (submission == null) return null;

        return _mapper.Map<FormSubmissionDto>(submission);
    }

    public async Task<(IReadOnlyList<FormSubmissionDto> Items, int TotalCount)> GetSubmissionsPagedAsync(FormSubmissionFilterDto filter)
    {
        Expression<Func<FormSubmission, bool>> predicate = s => true;
        
        if (filter.FormId.HasValue)
        {
            predicate = CombinePredicates(predicate, s => s.FormId == filter.FormId.Value);
        }

        if (!string.IsNullOrEmpty(filter.Status))
        {
            predicate = CombinePredicates(predicate, s => s.Status == filter.Status);
        }

        if (!string.IsNullOrEmpty(filter.Search))
        {
            var search = filter.Search.ToLower();
            predicate = CombinePredicates(predicate, s => 
                s.FieldValues.Any(fv => fv.Value != null && fv.Value.ToLower().Contains(search)) ||
                s.IpAddress != null && s.IpAddress.Contains(search));
        }

        if (filter.FromDate.HasValue)
        {
            predicate = CombinePredicates(predicate, s => s.CreatedAt >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            predicate = CombinePredicates(predicate, s => s.CreatedAt <= filter.ToDate.Value);
        }

        var (items, totalCount) = await _unitOfWork.FormSubmissions.GetPagedAsync(
            filter.Page, 
            filter.PageSize, 
            predicate, 
            BuildOrderBy(filter.SortBy, filter.SortDirection),
            s => s.FieldValues, s => s.Files, s => s.Form);

        var dtos = _mapper.Map<IReadOnlyList<FormSubmissionDto>>(items);
        return (dtos, totalCount);
    }

    public async Task UpdateSubmissionStatusAsync(int id, string status, string? notes = null, string? processedBy = null)
    {
        var submission = await _unitOfWork.FormSubmissions.GetByIdAsync(id);
        if (submission == null)
        {
            throw new KeyNotFoundException($"Submission with id {id} not found.");
        }

        submission.Status = status;
        submission.Notes = notes;
        submission.ProcessedAt = DateTime.UtcNow;
        submission.ProcessedBy = processedBy;
        submission.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.FormSubmissions.UpdateAsync(submission);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<byte[]> ExportSubmissionsToExcelAsync(int formId, FormSubmissionFilterDto filter)
    {
        var (submissions, _) = await GetSubmissionsPagedAsync(new FormSubmissionFilterDto
        {
            FormId = formId,
            Page = 1,
            PageSize = int.MaxValue,
            SortBy = filter.SortBy,
            SortDirection = filter.SortDirection
        });

        using var workbook = new ClosedXML.Excel.XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Submissions");

        // Headers
        var form = await _unitOfWork.Forms.GetByIdAsync(formId, f => f.Fields);
        var fields = form?.Fields.OrderBy(f => f.DisplayOrder).ToList() ?? new List<FormField>();

        int col = 1;
        worksheet.Cell(1, col++).Value = "ID";
        worksheet.Cell(1, col++).Value = "Date";
        worksheet.Cell(1, col++).Value = "IP Address";
        worksheet.Cell(1, col++).Value = "Status";
        
        foreach (var field in fields)
        {
            worksheet.Cell(1, col++).Value = field.Label ?? field.Name;
        }

        worksheet.Cell(1, col++).Value = "Notes";

        // Data
        int row = 2;
        foreach (var submission in submissions)
        {
            col = 1;
            worksheet.Cell(row, col++).Value = submission.Id;
            worksheet.Cell(row, col++).Value = submission.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
            worksheet.Cell(row, col++).Value = submission.IpAddress;
            worksheet.Cell(row, col++).Value = submission.Status;
            
            foreach (var field in fields)
            {
                var value = submission.FieldValues.FirstOrDefault(fv => fv.FieldId == field.Id)?.Value;
                worksheet.Cell(row, col++).Value = value ?? string.Empty;
            }
            
            worksheet.Cell(row, col++).Value = submission.Notes;
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportSubmissionsToCsvAsync(int formId, FormSubmissionFilterDto filter)
    {
        var (submissions, _) = await GetSubmissionsPagedAsync(new FormSubmissionFilterDto
        {
            FormId = formId,
            Page = 1,
            PageSize = int.MaxValue,
            SortBy = filter.SortBy,
            SortDirection = filter.SortDirection
        });

        var form = await _unitOfWork.Forms.GetByIdAsync(formId, f => f.Fields);
        var fields = form?.Fields.OrderBy(f => f.DisplayOrder).ToList() ?? new List<FormField>();

        var csv = new StringBuilder();
        
        // Headers
        csv.AppendLine("ID,Date,IP Address,Status," + string.Join(",", fields.Select(f => $"\"{(f.Label ?? f.Name).Replace("\"", "\"\"")}\"")) + ",Notes");

        // Data
        foreach (var submission in submissions)
        {
            var values = new List<string>
            {
                submission.Id.ToString(),
                submission.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                submission.IpAddress ?? "",
                submission.Status
            };
            
            foreach (var field in fields)
            {
                var value = submission.FieldValues.FirstOrDefault(fv => fv.FieldId == field.Id)?.Value ?? "";
                values.Add($"\"{value.Replace("\"", "\"\"")}\"");
            }
            
            values.Add($"\"{(submission.Notes ?? "").Replace("\"", "\"\"")}\"");
            csv.AppendLine(string.Join(",", values));
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    public async Task<bool> ValidateSubmissionAsync(SubmitFormDto dto, int formId)
    {
        var form = await _unitOfWork.Forms.GetByIdAsync(formId, f => f.Fields);
        if (form == null) return false;

        foreach (var field in form.Fields.Where(f => f.IsVisible && f.IsRequired))
        {
            if (!dto.FieldValues.TryGetValue(field.Name, out var value) || string.IsNullOrEmpty(value))
            {
                return false;
            }

            // Validate regex
            if (!string.IsNullOrEmpty(field.ValidationRegex) && !string.IsNullOrEmpty(value))
            {
                var regex = new Regex(field.ValidationRegex);
                if (!regex.IsMatch(value))
                {
                    return false;
                }
            }

            // Validate field type
            if (!ValidateFieldType(field.FieldType, value))
            {
                return false;
            }
        }

        return true;
    }

    public async Task SendNotificationEmailsAsync(FormSubmissionDto submission)
    {
        var form = await _unitOfWork.Forms.GetByIdAsync(submission.FormId);
        if (form == null || string.IsNullOrEmpty(form.NotificationEmails)) return;

        var emails = form.NotificationEmails.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        
        var body = BuildNotificationEmailBody(submission, form);
        
        foreach (var email in emails)
        {
            await _emailService.SendAsync(email, $"New Form Submission: {form.Name}", body);
        }
    }

    public async Task SendConfirmationEmailAsync(FormSubmissionDto submission)
    {
        var form = await _unitOfWork.Forms.GetByIdAsync(submission.FormId);
        if (form == null) return;

        // Find email field value
        var emailField = form.Fields.FirstOrDefault(f => f.FieldType == "Email");
        if (emailField == null) return;

        var emailValue = submission.FieldValues.FirstOrDefault(fv => fv.FieldId == emailField.Id)?.Value;
        if (string.IsNullOrEmpty(emailValue)) return;

        var subject = form.ConfirmationEmailSubject ?? $"Thank you for your submission: {form.Name}";
        var body = form.ConfirmationEmailTemplate ?? BuildConfirmationEmailBody(submission, form);
        
        await _emailService.SendAsync(emailValue, subject, body);
    }

    private async Task<bool> ValidateRecaptchaAsync(string token)
    {
        var secretKey = await _settingService.GetValueAsync("RecaptchaSecretKey");
        if (string.IsNullOrEmpty(secretKey)) return true; // Skip if not configured

        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}",
                null);
            
            var json = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<RecaptchaResponse>(json);
            
            return result?.Success == true && result.Score >= 0.5;
        }
        catch
        {
            return false;
        }
    }

    private bool ValidateFieldType(string fieldType, string value)
    {
        return fieldType switch
        {
            "Email" => new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(value),
            "Phone" => Regex.IsMatch(value, @"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$"),
            "Number" => double.TryParse(value, out _),
            "Date" => DateTime.TryParse(value, out _),
            _ => true
        };
    }

    private string BuildNotificationEmailBody(FormSubmissionDto submission, Form form)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"<h2>New Form Submission</h2>");
        sb.AppendLine($"<p><strong>Form:</strong> {form.Name}</p>");
        sb.AppendLine($"<p><strong>Submitted at:</strong> {submission.CreatedAt:yyyy-MM-dd HH:mm:ss}</p>");
        sb.AppendLine($"<p><strong>IP Address:</strong> {submission.IpAddress}</p>");
        sb.AppendLine($"<p><strong>Language:</strong> {submission.LanguageCode}</p>");
        sb.AppendLine("<h3>Field Values:</h3>");
        sb.AppendLine("<table border='1' cellpadding='5' cellspacing='0'>");
        sb.AppendLine("<tr><th>Field</th><th>Value</th></tr>");
        
        foreach (var fieldValue in submission.FieldValues)
        {
            sb.AppendLine($"<tr><td>{fieldValue.FieldLabel}</td><td>{fieldValue.Value}</td></tr>");
        }
        
        sb.AppendLine("</table>");
        
        if (submission.Files.Any())
        {
            sb.AppendLine("<h3>Attachments:</h3>");
            sb.AppendLine("<ul>");
            foreach (var file in submission.Files)
            {
                sb.AppendLine($"<li>{file.OriginalFileName} ({file.FileSize} bytes)</li>");
            }
            sb.AppendLine("</ul>");
        }
        
        return sb.ToString();
    }

    private string BuildConfirmationEmailBody(FormSubmissionDto submission, Form form)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"<h2>Thank you for your submission</h2>");
        sb.AppendLine($"<p>We have received your submission for <strong>{form.Name}</strong>.</p>");
        sb.AppendLine($"<p><strong>Submitted at:</strong> {submission.CreatedAt:yyyy-MM-dd HH:mm:ss}</p>");
        sb.AppendLine("<p>We will review your submission and get back to you soon.</p>");
        return sb.ToString();
    }

    private async Task UpdateFieldsAsync(ICollection<FormField> existing, List<UpdateFormFieldDto> dtos)
    {
        var existingDict = existing.ToDictionary(f => f.Id);
        var processedIds = new HashSet<int>();

        foreach (var dto in dtos)
        {
            if (dto.Id.HasValue && existingDict.TryGetValue(dto.Id.Value, out var field))
            {
                _mapper.Map(dto, field);
                field.UpdatedAt = DateTime.UtcNow;
                
                await UpdateTranslationsAsync(field.Translations, dto.Translations, (t, d) => _mapper.Map(d, t));
                
                processedIds.Add(dto.Id.Value);
            }
            else if (!dto.Id.HasValue)
            {
                var newField = _mapper.Map<FormField>(dto);
                foreach (var transDto in dto.Translations)
                {
                    var translation = _mapper.Map<FormFieldTranslation>(transDto);
                    translation.Field = newField;
                    newField.Translations.Add(translation);
                }
                existing.Add(newField);
            }
        }

        foreach (var field in existing.Where(f => !processedIds.Contains(f.Id)))
        {
            field.IsDeleted = true;
            field.DeletedAt = DateTime.UtcNow;
        }
    }

    private Expression<Func<FormSubmission, bool>> CombinePredicates(Expression<Func<FormSubmission, bool>> first, Expression<Func<FormSubmission, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(FormSubmission));
        var firstBody = Expression.Invoke(first, parameter);
        var secondBody = Expression.Invoke(second, parameter);
        var combined = Expression.AndAlso(firstBody, secondBody);
        return Expression.Lambda<Func<FormSubmission, bool>>(combined, parameter);
    }

    private Func<IQueryable<FormSubmission>, IOrderedQueryable<FormSubmission>> BuildOrderBy(string? sortBy, string? sortDirection)
    {
        var isDesc = sortDirection?.ToLower() == "desc";
        
        return sortBy?.ToLower() switch
        {
            "createdat" => isDesc ? q => q.OrderByDescending(s => s.CreatedAt) : q => q.OrderBy(s => s.CreatedAt),
            "status" => isDesc ? q => q.OrderByDescending(s => s.Status) : q => q.OrderBy(s => s.Status),
            _ => isDesc ? q => q.OrderByDescending(s => s.CreatedAt) : q => q.OrderByDescending(s => s.CreatedAt)
        };
    }

    private async Task UpdateTranslationsAsync<TEntity, TDto>(
        ICollection<TEntity> existing, 
        List<TDto> dtos,
        Action<TEntity, TDto> updateAction)
        where TEntity : BaseEntity
    {
        var existingDict = existing.ToDictionary(e => e.Id);
        var processedIds = new HashSet<int>();

        foreach (var dto in dtos)
        {
            var idProp = typeof(TDto).GetProperty("Id");
            var id = idProp?.GetValue(dto) as int?;
            
            if (id.HasValue && existingDict.TryGetValue(id.Value, out var entity))
            {
                updateAction(entity, dto);
                entity.UpdatedAt = DateTime.UtcNow;
                processedIds.Add(id.Value);
            }
        }

        foreach (var entity in existing.Where(e => !processedIds.Contains(e.Id)))
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }
    }

    private class RecaptchaResponse
    {
        public bool Success { get; set; }
        public double Score { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTime ChallengeTs { get; set; }
        public string Hostname { get; set; } = string.Empty;
        public string[]? ErrorCodes { get; set; }
    }
}