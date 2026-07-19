namespace CorporateWebsite.Core.Entities;

public class Form : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Slug { get; set; } = string.Empty;
    public int? PageId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool AllowMultipleSubmissions { get; set; } = false;
    public bool RequireCaptcha { get; set; } = true;
    public int? RateLimitCount { get; set; }
    public int? RateLimitWindowMinutes { get; set; } = 60;
    public string? SubmitButtonText { get; set; }
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
    public string? RedirectUrl { get; set; }
    public string? NotificationEmails { get; set; }
    public bool SendConfirmationEmail { get; set; } = false;
    public string? ConfirmationEmailSubject { get; set; }
    public string? ConfirmationEmailTemplate { get; set; }
    public bool StoreInDatabase { get; set; } = true;
    public bool ExportToExcel { get; set; } = true;
    
    public Page? Page { get; set; }
    public ICollection<FormTranslation> Translations { get; set; } = new List<FormTranslation>();
    public ICollection<FormField> Fields { get; set; } = new List<FormField>();
    public ICollection<FormSubmission> Submissions { get; set; } = new List<FormSubmission>();
}

public class FormTranslation : BaseEntity
{
    public int FormId { get; set; }
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? SubmitButtonText { get; set; }
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
    
    public Form Form { get; set; } = null!;
    public Language Language { get; set; } = null!;
}

public class FormField : BaseEntity
{
    public int FormId { get; set; }
    public string FieldType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Label { get; set; }
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public bool IsRequired { get; set; } = false;
    public int DisplayOrder { get; set; }
    public string? ValidationRegex { get; set; }
    public string? ValidationMessage { get; set; }
    public string? DefaultValue { get; set; }
    public string? Options { get; set; }
    public string? CssClass { get; set; }
    public string? Attributes { get; set; }
    public long? MaxFileSizeBytes { get; set; }
    public string? AllowedFileTypes { get; set; }
    public int? MaxFiles { get; set; } = 1;
    public bool IsVisible { get; set; } = true;
    public string? ConditionalLogic { get; set; }
    
    public Form Form { get; set; } = null!;
    public ICollection<FormFieldTranslation> Translations { get; set; } = new List<FormFieldTranslation>();
    public ICollection<FormFieldValue> Values { get; set; } = new List<FormFieldValue>();
}

public class FormFieldTranslation : BaseEntity
{
    public int FieldId { get; set; }
    public int LanguageId { get; set; }
    public string? Label { get; set; }
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public string? ValidationMessage { get; set; }
    public string? Options { get; set; }
    
    public FormField Field { get; set; } = null!;
    public Language Language { get; set; } = null!;
}

public class FormSubmission : BaseEntity
{
    public int FormId { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? ReferrerUrl { get; set; }
    public int LanguageId { get; set; }
    public string Status { get; set; } = "New";
    public string? Notes { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? ProcessedBy { get; set; }
    
    public Form Form { get; set; } = null!;
    public Language Language { get; set; } = null!;
    public ICollection<FormFieldValue> FieldValues { get; set; } = new List<FormFieldValue>();
    public ICollection<FormSubmissionFile> Files { get; set; } = new List<FormSubmissionFile>();
}

public class FormFieldValue : BaseEntity
{
    public int SubmissionId { get; set; }
    public int FieldId { get; set; }
    public string? Value { get; set; }
    
    public FormSubmission Submission { get; set; } = null!;
    public FormField Field { get; set; } = null!;
}

public class FormSubmissionFile : BaseEntity
{
    public int SubmissionId { get; set; }
    public int FieldId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    
    public FormSubmission Submission { get; set; } = null!;
    public FormField Field { get; set; } = null!;
}

public static class FormFieldTypes
{
    public const string Text = "Text";
    public const string TextArea = "TextArea";
    public const string Email = "Email";
    public const string Phone = "Phone";
    public const string Number = "Number";
    public const string Date = "Date";
    public const string DateTime = "DateTime";
    public const string Select = "Select";
    public const string Radio = "Radio";
    public const string Checkbox = "Checkbox";
    public const string CheckboxList = "CheckboxList";
    public const string File = "File";
    public const string Hidden = "Hidden";
    public const string Captcha = "Captcha";
    public const string Html = "Html";
    public const string Rating = "Rating";
    public const string Signature = "Signature";
}