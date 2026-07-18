namespace CorporateWebsite.Core.Entities;

public class Language : BaseEntity
{
    public string Code { get; set; } = string.Empty; // fa, en, ar
    public string Name { get; set; } = string.Empty; // فارسی, English, العربية
    public string NativeName { get; set; } = string.Empty;
    public string CultureCode { get; set; } = string.Empty; // fa-IR, en-US, ar-SA
    public bool IsRtl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; }
    public string? FlagIcon { get; set; } // emoji or icon class
    
    // Navigation
    public ICollection<PageTranslation> PageTranslations { get; set; } = new List<PageTranslation>();
    public ICollection<ServiceTranslation> ServiceTranslations { get; set; } = new List<ServiceTranslation>();
    public ICollection<ProjectTranslation> ProjectTranslations { get; set; } = new List<ProjectTranslation>();
    public ICollection<NewsTranslation> NewsTranslations { get; set; } = new List<NewsTranslation>();
    public ICollection<CategoryTranslation> CategoryTranslations { get; set; } = new List<CategoryTranslation>();
    public ICollection<MenuTranslation> MenuTranslations { get; set; } = new List<MenuTranslation>();
    public ICollection<SettingTranslation> SettingTranslations { get; set; } = new List<SettingTranslation>();
    public ICollection<FormTranslation> FormTranslations { get; set; } = new List<FormTranslation>();
    public ICollection<FormFieldTranslation> FormFieldTranslations { get; set; } = new List<FormFieldTranslation>();
}