namespace CorporateWebsite.Core.DTOs;

public class RedirectDto
{
    public int Id { get; set; }
    public string SourceUrl { get; set; } = string.Empty;
    public string TargetUrl { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public bool IsActive { get; set; }
    public int? LanguageId { get; set; }
    public string? LanguageCode { get; set; }
    public string? Condition { get; set; }
    public int HitCount { get; set; }
    public DateTime? LastHitAt { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateRedirectDto
{
    public string SourceUrl { get; set; } = string.Empty;
    public string TargetUrl { get; set; } = string.Empty;
    public int StatusCode { get; set; } = 301;
    public bool IsActive { get; set; } = true;
    public int? LanguageId { get; set; }
    public string? Condition { get; set; }
    public string? Description { get; set; }
}

public class UpdateRedirectDto
{
    public string SourceUrl { get; set; } = string.Empty;
    public string TargetUrl { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public bool IsActive { get; set; }
    public int? LanguageId { get; set; }
    public string? Condition { get; set; }
    public string? Description { get; set; }
}

public class RedirectFilterDto
{
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
    public int? StatusCode { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "CreatedAt";
    public string? SortDirection { get; set; } = "desc";
}

public class BrokenLinkDto
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Referrer { get; set; }
    public int StatusCode { get; set; }
    public int HitCount { get; set; }
    public DateTime? FirstDetectedAt { get; set; }
    public DateTime? LastDetectedAt { get; set; }
    public bool IsFixed { get; set; }
    public string? FixedUrl { get; set; }
    public string? Notes { get; set; }
}

public class ActivityLogDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public string? EntityType { get; set; }
    public string? EntityName { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Url { get; set; }
    public string? Method { get; set; }
    public int? DurationMs { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public string? CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ActivityLogFilterDto
{
    public string? UserId { get; set; }
    public string? Action { get; set; }
    public string? Module { get; set; }
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }
    public bool? IsSuccess { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? IpAddress { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortBy { get; set; } = "CreatedAt";
    public string? SortDirection { get; set; } = "desc";
}

public class BackupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string Type { get; set; } = "Full";
    public string Status { get; set; } = "InProgress";
    public string? ErrorMessage { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? InitiatedBy { get; set; }
    public bool IsAutomatic { get; set; }
    public string? Checksum { get; set; }
    public int? RetentionDays { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BackupStatusDto
{
    public int TotalBackups { get; set; }
    public long TotalSize { get; set; }
    public DateTime? LastBackupAt { get; set; }
    public string? LastBackupStatus { get; set; }
    public DateTime? NextScheduledBackup { get; set; }
    public int AvailableDiskSpace { get; set; }
}

public class ScheduledTaskDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TaskType { get; set; } = string.Empty;
    public string CronExpression { get; set; } = string.Empty;
    public string? Parameters { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastRunAt { get; set; }
    public DateTime? NextRunAt { get; set; }
    public DateTime? LastSuccessAt { get; set; }
    public string? LastError { get; set; }
    public int ConsecutiveFailures { get; set; }
    public TimeSpan? Timeout { get; set; }
    public bool RunOnStartup { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateScheduledTaskDto
{
    public string Name { get; set; } = string.Empty;
    public string TaskType { get; set; } = string.Empty;
    public string CronExpression { get; set; } = string.Empty;
    public string? Parameters { get; set; }
    public bool IsActive { get; set; } = true;
    public TimeSpan? Timeout { get; set; }
    public bool RunOnStartup { get; set; }
}

public class UpdateScheduledTaskDto
{
    public string Name { get; set; } = string.Empty;
    public string TaskType { get; set; } = string.Empty;
    public string CronExpression { get; set; } = string.Empty;
    public string? Parameters { get; set; }
    public bool IsActive { get; set; }
    public TimeSpan? Timeout { get; set; }
    public bool RunOnStartup { get; set; }
}

public class ScheduledTaskLogDto
{
    public int Id { get; set; }
    public int ScheduledTaskId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsSuccess { get; set; }
    public string? Output { get; set; }
    public string? Error { get; set; }
    public int? DurationMs { get; set; }
}

public class EmailTemplateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string BodyHtml { get; set; } = string.Empty;
    public string? BodyText { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; } = "General";
    public bool IsActive { get; set; }
    public string? AvailableVariables { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Translations
    public EmailTemplateTranslationDto? Translation { get; set; }
    public ICollection<EmailTemplateTranslationDto> Translations { get; set; } = new List<EmailTemplateTranslationDto>();
}

public class EmailTemplateTranslationDto
{
    public int Id { get; set; }
    public int EmailTemplateId { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string BodyHtml { get; set; } = string.Empty;
    public string? BodyText { get; set; }
}