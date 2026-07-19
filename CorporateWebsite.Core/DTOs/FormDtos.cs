namespace CorporateWebsite.Core.DTOs;

public class FormDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Slug { get; set; } = string.Empty;
    public int? PageId { get; set; }
    public bool IsActive { get; set; }
    public bool AllowMultipleSubmissions { get; set; }
    public bool RequireCaptcha { get; set; }
    public int? RateLimitCount { get; set; }
    public int? RateLimitWindowMinutes { get; set; }
    public string? SubmitButtonText { get; set; }
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
    public string? RedirectUrl { get; set; }
    public string? NotificationEmails { get; set; }
    public bool SendConfirmationEmail { get; set; }
    public string? ConfirmationEmailSubject { get; set; }
    public string? ConfirmationEmailTemplate { get; set; }
    public bool StoreInDatabase { get; set; }
    public bool ExportToExcel { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public FormTranslationDto? Translation { get; set; }
    public ICollection<FormTranslationDto> Translations { get; set; } = new List<FormTranslationDto>();
    public ICollection<FormFieldDto> Fields { get; set; } = new List<FormFieldDto>();
}

public class FormTranslationDto
{
    public int Id { get; set; }
    public int FormId { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? SubmitButtonText { get; set; }
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
}

public class CreateFormDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Slug { get; set; } = string.Empty;
    public int? PageId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool AllowMultipleSubmissions { get; set; }
    public bool RequireCaptcha { get; set; } = true;
    public int? RateLimitCount { get; set; }
    public int? RateLimitWindowMinutes { get; set; } = 60;
    public string? SubmitButtonText { get; set; }
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
    public string? RedirectUrl { get; set; }
    public string? NotificationEmails { get; set; }
    public bool SendConfirmationEmail { get; set; }
    public string? ConfirmationEmailSubject { get; set; }
    public string? ConfirmationEmailTemplate { get; set; }
    public bool StoreInDatabase { get; set; } = true;
    public bool ExportToExcel { get; set; } = true;
    public List<CreateFormTranslationDto> Translations { get; set; } = new List<CreateFormTranslationDto>();
    public List<CreateFormFieldDto> Fields { get; set; } = new List<CreateFormFieldDto>();
}

public class CreateFormTranslationDto
{
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? SubmitButtonText { get; set; }
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
}

public class UpdateFormDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Slug { get; set; } = string.Empty;
    public int? PageId { get; set; }
    public bool IsActive { get; set; }
    public bool AllowMultipleSubmissions { get; set; }
    public bool RequireCaptcha { get; set; }
    public int? RateLimitCount { get; set; }
    public int? RateLimitWindowMinutes { get; set; }
    public string? SubmitButtonText { get; set; }
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
    public string? RedirectUrl { get; set; }
    public string? NotificationEmails { get; set; }
    public bool SendConfirmationEmail { get; set; }
    public string? ConfirmationEmailSubject { get; set; }
    public string? ConfirmationEmailTemplate { get; set; }
    public bool StoreInDatabase { get; set; }
    public bool ExportToExcel { get; set; }
    public List<UpdateFormTranslationDto> Translations { get; set; } = new List<UpdateFormTranslationDto>();
    public List<UpdateFormFieldDto> Fields { get; set; } = new List<UpdateFormFieldDto>();
}

public class UpdateFormTranslationDto
{
    public int? Id { get; set; }
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? SubmitButtonText { get; set; }
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
}

public class FormFieldDto
{
    public int Id { get; set; }
    public int FormId { get; set; }
    public string FieldType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Label { get; set; }
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public string? ValidationRegex { get; set; }
    public string? ValidationMessage { get; set; }
    public string? DefaultValue { get; set; }
    public string? Options { get; set; }
    public string? CssClass { get; set; }
    public string? Attributes { get; set; }
    public long? MaxFileSizeBytes { get; set; }
    public string? AllowedFileTypes { get; set; }
    public int? MaxFiles { get; set; }
    public bool IsVisible { get; set; }
    public string? ConditionalLogic { get; set; }
    
    public FormFieldTranslationDto? Translation { get; set; }
    public ICollection<FormFieldTranslationDto> Translations { get; set; } = new List<FormFieldTranslationDto>();
}

public class FormFieldTranslationDto
{
    public int Id { get; set; }
    public int FieldId { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string? Label { get; set; }
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public string? ValidationMessage { get; set; }
    public string? Options { get; set; }
}

public class CreateFormFieldDto
{
    public string FieldType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Label { get; set; }
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public string? ValidationRegex { get; set; }
    public string? ValidationMessage { get; set; }
    public string? DefaultValue { get; set; }
    public string? Options { get; set; }
    public string? CssClass { get; set; }
    public string? Attributes { get; set; }
    public long? MaxFileSizeBytes { get; set; }
    public string? AllowedFileTypes { get; set; }
    public int? MaxFiles { get; set; }
    public bool IsVisible { get; set; } = true;
    public string? ConditionalLogic { get; set; }
    public List<CreateFormFieldTranslationDto> Translations { get; set; } = new List<CreateFormFieldTranslationDto>();
}

public class CreateFormFieldTranslationDto
{
    public int LanguageId { get; set; }
    public string? Label { get; set; }
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public string? ValidationMessage { get; set; }
    public string? Options { get; set; }
}

public class UpdateFormFieldDto
{
    public int? Id { get; set; }
    public string FieldType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Label { get; set; }
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public string? ValidationRegex { get; set; }
    public string? ValidationMessage { get; set; }
    public string? DefaultValue { get; set; }
    public string? Options { get; set; }
    public string? CssClass { get; set; }
    public string? Attributes { get; set; }
    public long? MaxFileSizeBytes { get; set; }
    public string? AllowedFileTypes { get; set; }
    public int? MaxFiles { get; set; }
    public bool IsVisible { get; set; }
    public string? ConditionalLogic { get; set; }
    public List<UpdateFormFieldTranslationDto> Translations { get; set; } = new List<UpdateFormFieldTranslationDto>();
}

public class UpdateFormFieldTranslationDto
{
    public int? Id { get; set; }
    public int LanguageId { get; set; }
    public string? Label { get; set; }
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public string? ValidationMessage { get; set; }
    public string? Options { get; set; }
}

public class FormSubmissionDto
{
    public int Id { get; set; }
    public int FormId { get; set; }
    public string FormName { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? ReferrerUrl { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Status { get; set; } = "New";
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? ProcessedBy { get; set; }
    
    public ICollection<FormFieldValueDto> FieldValues { get; set; } = new List<FormFieldValueDto>();
    public ICollection<FormSubmissionFileDto> Files { get; set; } = new List<FormSubmissionFileDto>();
}

public class FormFieldValueDto
{
    public int Id { get; set; }
    public int SubmissionId { get; set; }
    public int FieldId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string FieldLabel { get; set; } = string.Empty;
    public string? Value { get; set; }
}

public class FormSubmissionFileDto
{
    public int Id { get; set; }
    public int SubmissionId { get; set; }
    public int FieldId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
}

// SubmitFormDto moved to Application layer - uses IFormFile
public class FormSubmissionFilterDto
{
    public int? FormId { get; set; }
    public string? Status { get; set; }
    public string? Search { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "CreatedAt";
    public string? SortDirection { get; set; } = "desc";
}

public class EmailQueueDto
{
    public int Id { get; set; }
    public string ToEmail { get; set; } = string.Empty;
    public string? ToName { get; set; }
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string BodyHtml { get; set; } = string.Empty;
    public string? BodyText { get; set; }
    public string? Cc { get; set; }
    public string? Bcc { get; set; }
    public string? Attachments { get; set; }
    public int Priority { get; set; }
    public string Status { get; set; } = "Pending";
    public int RetryCount { get; set; }
    public int MaxRetries { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public DateTime? SentAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
}