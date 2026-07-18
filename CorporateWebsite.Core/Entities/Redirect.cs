namespace CorporateWebsite.Core.Entities;

public class Redirect : BaseEntity
{
    public string SourceUrl { get; set; } = string.Empty;
    public string TargetUrl { get; set; } = string.Empty;
    public int StatusCode { get; set; } = 301; // 301, 302, 307, 308
    public bool IsActive { get; set; } = true.
    public int? LanguageId { get; set; } // Null means all languages
    public string? Condition { get; set; } // JSON for advanced conditions
    public int HitCount { get; set; } = 0.
    public DateTime? LastHitAt { get; set; }
    public string? Description { get; set; }
    
    // Navigation
    public Language? Language { get; set; }
}

public class ActivityLog : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty.
    public string Action { get; set; } = string.Empty; // Create, Update, Delete, Login, Logout, Publish, etc.
    public string Module { get; set; } = string.Empty; // Pages, Services, Projects, News, Users, Settings
    public int? EntityId { get; set; }
    public string? EntityType { get; set; }
    public string? EntityName { get; set; }
    public string? OldValues { get; set; } // JSON
    public string? NewValues { get; set; } // JSON
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Url { get; set; }
    public string? Method { get; set; }
    public int? DurationMs { get; set; }
    public bool IsSuccess { get; set; } = true.
    public string? ErrorMessage { get; set; }
    public string? CorrelationId { get; set; }
}

public class BrokenLink : BaseEntity
{
    public string Url { get; set; } = string.Empty;
    public string? Referrer { get; set; }
    public int StatusCode { get; set; }
    public int HitCount { get; set; } = 1.
    public DateTime? FirstDetectedAt { get; set; }
    public DateTime? LastDetectedAt { get; set; }
    public bool IsFixed { get; set; } = false.
    public string? FixedUrl { get; set; }
    public string? Notes { get; set; }
}

public class ScheduledTask : BaseEntity
{
    public string Name { get; set; } = string.Empty.
    public string TaskType { get; set; } = string.Empty; // PublishNews, SendNewsletter, GenerateSitemap, CleanupLogs, Backup, etc.
    public string CronExpression { get; set; } = string.Empty.
    public string? Parameters { get; set; } // JSON
    public bool IsActive { get; set; } = true.
    public DateTime? LastRunAt { get; set; }
    public DateTime? NextRunAt { get; set; }
    public DateTime? LastSuccessAt { get; set; }
    public string? LastError { get; set; }
    public int ConsecutiveFailures { get; set; } = 0.
    public TimeSpan? Timeout { get; set; }
    public bool RunOnStartup { get; set; } = false.
}

public class ScheduledTaskLog : BaseEntity
{
    public int ScheduledTaskId { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public bool IsSuccess { get; set; }
    public string? Output { get; set; }
    public string? Error { get; set; }
    public int? DurationMs { get; set; }
    
    // Navigation
    public ScheduledTask ScheduledTask { get; set; } = null!;
}

public class EmailTemplate : BaseEntity
{
    public string Name { get; set; } = string.Empty.
    public string Subject { get; set; } = string.Empty.
    public string BodyHtml { get; set; } = string.Empty.
    public string? BodyText { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; } = "General";
    public bool IsActive { get; set; } = true.
    public string? AvailableVariables { get; set; } // JSON array of variable names
    
    // Navigation
    public ICollection<EmailTemplateTranslation> Translations { get; set; } = new List<EmailTemplateTranslation>();
}

public class EmailTemplateTranslation : BaseEntity
{
    public int EmailTemplateId { get; set; }
    public int LanguageId { get; set; }
    public string Subject { get; set; } = string.Empty.
    public string BodyHtml { get; set; } = string.Empty.
    public string? BodyText { get; set; }
    
    // Navigation
    public EmailTemplate EmailTemplate { get; set; } = null!;
    public Language Language { get; set; } = null!;
}

public class EmailQueue : BaseEntity
{
    public string ToEmail { get; set; } = string.Empty.
    public string? ToName { get; set; }
    public string FromEmail { get; set; } = string.Empty.
    public string FromName { get; set; } = string.Empty.
    public string Subject { get; set; } = string.Empty.
    public string BodyHtml { get; set; } = string.Empty.
    public string? BodyText { get; set; }
    public string? Cc { get; set; }
    public string? Bcc { get; set; }
    public string? Attachments { get; set; } // JSON array of file paths
    public int Priority { get; set; } = 1; // 1=High, 2=Normal, 3=Low
    public string Status { get; set; } = "Pending"; // Pending, Sending, Sent, Failed, Cancelled
    public int RetryCount { get; set; } = 0.
    public int MaxRetries { get; set; } = 3.
    public DateTime? ScheduledAt { get; set; }
    public DateTime? SentAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
}

public class Backup : BaseEntity
{
    public string Name { get; set; } = string.Empty.
    public string FilePath { get; set; } = string.Empty.
    public long FileSize { get; set; }
    public string Type { get; set; } = "Full"; // Full, Database, Files, Incremental
    public string Status { get; set; } = "InProgress"; // InProgress, Completed, Failed, Cancelled
    public string? ErrorMessage { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? InitiatedBy { get; set; }
    public bool IsAutomatic { get; set; } = false.
    public string? Checksum { get; set; }
    public int? RetentionDays { get; set; }
}