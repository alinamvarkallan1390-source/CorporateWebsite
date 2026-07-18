using CorporateWebsite.Core.DTOs;
using CorporateWebsite.Core.Entities;
using CorporateWebsite.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CorporateWebsite.Infrastructure.Services;

public class ScheduledTaskService : IScheduledTaskService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISettingService _settingService;
    private readonly ISitemapService _sitemapService;
    private readonly IEmailService _emailService;
    private readonly INewsService _newsService;
    private readonly IBackupService _backupService;
    private readonly IActivityLogService _activityLogService;

    public ScheduledTaskService(
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
        IServiceProvider serviceProvider,
        ISettingService settingService,
        ISitemapService sitemapService,
        IEmailService emailService,
        INewsService newsService,
        IBackupService backupService,
        IActivityLogService activityLogService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _serviceProvider = serviceProvider;
        _settingService = settingService;
        _sitemapService = sitemapService;
        _emailService = emailService;
        _newsService = newsService;
        _backupService = backupService;
        _activityLogService = activityLogService;
    }

    public async Task<ScheduledTaskDto?> GetByIdAsync(int id)
    {
        var task = await _unitOfWork.ScheduledTasks.GetByIdAsync(id);
        return task != null ? _mapper.Map<ScheduledTaskDto>(task) : null;
    }

    public async Task<IReadOnlyList<ScheduledTaskDto>> GetAllAsync(bool? isActive = null)
    {
        Expression<Func<ScheduledTask, bool>> predicate = t => true;
        
        if (isActive.HasValue)
        {
            predicate = CombinePredicates(predicate, t => t.IsActive == isActive.Value);
        }

        var tasks = await _unitOfWork.ScheduledTasks.GetAllAsync(predicate, q => q.OrderBy(t => t.Name));
        return _mapper.Map<IReadOnlyList<ScheduledTaskDto>>(tasks);
    }

    public async Task<ScheduledTaskDto> CreateAsync(CreateScheduledTaskDto dto)
    {
        var task = _mapper.Map<ScheduledTask>(dto);
        task.CreatedAt = DateTime.UtcNow;
        task.NextRunAt = CalculateNextRun(task.CronExpression);

        await _unitOfWork.ScheduledTasks.AddAsync(task);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ScheduledTaskDto>(task);
    }

    public async Task<ScheduledTaskDto> UpdateAsync(int id, UpdateScheduledTaskDto dto)
    {
        var task = await _unitOfWork.ScheduledTasks.GetByIdAsync(id);
        if (task == null)
        {
            throw new KeyNotFoundException($"Scheduled task with id {id} not found.");
        }

        _mapper.Map(dto, task);
        task.UpdatedAt = DateTime.UtcNow;
        task.NextRunAt = CalculateNextRun(task.CronExpression);

        await _unitOfWork.ScheduledTasks.UpdateAsync(task);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ScheduledTaskDto>(task);
    }

    public async Task DeleteAsync(int id)
    {
        var task = await _unitOfWork.ScheduledTasks.GetByIdAsync(id);
        if (task == null)
        {
            throw new KeyNotFoundException($"Scheduled task with id {id} not found.");
        }

        await _unitOfWork.ScheduledTasks.DeleteAsync(task);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RunTaskAsync(int id)
    {
        var task = await _unitOfWork.ScheduledTasks.GetByIdAsync(id);
        if (task == null)
        {
            throw new KeyNotFoundException($"Scheduled task with id {id} not found.");
        }

        var log = new ScheduledTaskLog
        {
            ScheduledTaskId = task.Id,
            StartedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.ScheduledTaskLogs.AddAsync(log);
        
        task.LastRunAt = DateTime.UtcNow;
        await _unitOfWork.ScheduledTasks.UpdateAsync(task);
        await _unitOfWork.SaveChangesAsync();

        try
        {
            await ExecuteTaskAsync(task);
            
            log.CompletedAt = DateTime.UtcNow;
            log.IsSuccess = true;
            log.DurationMs = (int)(log.CompletedAt.Value - log.StartedAt).TotalMilliseconds;
            
            task.LastSuccessAt = DateTime.UtcNow;
            task.ConsecutiveFailures = 0;
            task.NextRunAt = CalculateNextRun(task.CronExpression);
        }
        catch (Exception ex)
        {
            log.CompletedAt = DateTime.UtcNow;
            log.IsSuccess = false;
            log.Error = ex.Message;
            log.DurationMs = (int)(log.CompletedAt.Value - log.StartedAt).TotalMilliseconds;
            
            task.ConsecutiveFailures++;
            task.LastError = ex.Message;
        }
        finally
        {
            await _unitOfWork.ScheduledTaskLogs.UpdateAsync(log);
            await _unitOfWork.ScheduledTasks.UpdateAsync(task);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task ProcessDueTasksAsync()
    {
        var now = DateTime.UtcNow;
        var dueTasks = await _unitOfWork.ScheduledTasks.GetAllAsync(
            t => t.IsActive && t.NextRunAt.HasValue && t.NextRunAt.Value <= now,
            q => q.OrderBy(t => t.NextRunAt));

        foreach (var task in dueTasks)
        {
            try
            {
                await RunTaskAsync(task.Id);
            }
            catch (Exception ex)
            {
                // Log error but continue with other tasks
                await _activityLogService.LogAsync(
                    "system", "System", "Error", "ScheduledTasks",
                    entityId: task.Id, entityType: "ScheduledTask", entityName: task.Name,
                    newValues: new { Error = ex.Message },
                    isSuccess: false, errorMessage: ex.Message);
            }
        }
    }

    public async Task<ScheduledTaskLogDto?> GetLastLogAsync(int taskId)
    {
        var log = await _unitOfWork.ScheduledTaskLogs.GetAllAsync(
            l => l.ScheduledTaskId == taskId,
            q => q.OrderByDescending(l => l.StartedAt).Take(1));
        
        return log.FirstOrDefault() != null ? _mapper.Map<ScheduledTaskLogDto>(log.First()) : null;
    }

    private async Task ExecuteTaskAsync(ScheduledTask task)
    {
        var parameters = string.IsNullOrEmpty(task.Parameters) 
            ? new Dictionary<string, string>() 
            : JsonSerializer.Deserialize<Dictionary<string, string>>(task.Parameters);

        switch (task.TaskType)
        {
            case "PublishNews":
                await _newsService.ProcessScheduledNewsAsync();
                break;

            case "GenerateSitemap":
                var sitemap = await _sitemapService.GenerateSitemapAsync();
                var sitemapPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "sitemap.xml");
                await File.WriteAllTextAsync(sitemapPath, sitemap);
                break;

            case "SendNewsletter":
                // Implementation for newsletter sending
                break;

            case "CleanupLogs":
                var days = parameters.TryGetValue("days", out var daysStr) && int.TryParse(daysStr, out var d) ? d : 90;
                await _activityLogService.ClearOldLogsAsync(days);
                break;

            case "Backup":
                var type = parameters.TryGetValue("type", out var typeStr) ? typeStr : "Full";
                await _backupService.CreateBackupAsync($"Auto Backup - {DateTime.UtcNow:yyyy-MM-dd}", type, "System", true);
                break;

            case "ProcessEmailQueue":
                await _emailService.ProcessQueueAsync();
                break;

            case "RetryFailedEmails":
                await _emailService.RetryFailedEmailsAsync();
                break;

            case "CleanupOldBackups":
                var retentionDays = parameters.TryGetValue("retentionDays", out var retStr) && int.TryParse(retStr, out var rd) ? rd : 30;
                await _backupService.CleanupOldBackupsAsync(retentionDays);
                break;

            default:
                throw new NotSupportedException($"Task type '{task.TaskType}' is not supported.");
        }
    }

    private DateTime? CalculateNextRun(string cronExpression)
    {
        // Simplified cron parsing - in production use a library like NCrontab
        try
        {
            // This is a very basic implementation
            // For production, use a proper cron library
            return DateTime.UtcNow.AddHours(1); // Default to 1 hour
        }
        catch
        {
            return DateTime.UtcNow.AddHours(1);
        }
    }

    private Expression<Func<ScheduledTask, bool>> CombinePredicates(Expression<Func<ScheduledTask, bool>> first, Expression<Func<ScheduledTask, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(ScheduledTask));
        var firstBody = Expression.Invoke(first, parameter);
        var secondBody = Expression.Invoke(second, parameter);
        var combined = Expression.AndAlso(firstBody, secondBody);
        return Expression.Lambda<Func<ScheduledTask, bool>>(combined, parameter);
    }
}