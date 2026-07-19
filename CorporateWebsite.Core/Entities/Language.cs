namespace CorporateWebsite.Core.Entities;

public class Language : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NativeName { get; set; } = string.Empty;
    public string CultureCode { get; set; } = string.Empty;
    public bool IsRtl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; }
    public string? FlagIcon { get; set; }
    
    public ICollection<PageTranslation> PageTranslations { get; set; } = new List<PageTranslation>();
    public ICollection<ServiceTranslation> ServiceTranslations { get; set; } = new List<ServiceTranslation>();
    public ICollection<ProjectTranslation> ProjectTranslations { get; set; } = new List<ProjectTranslation>();
    public ICollection<NewsTranslation> NewsTranslations { get; set; } = new List<NewsTranslation>();
    public ICollection<ServiceCategoryTranslation> ServiceCategoryTranslations { get; set; } = new List<ServiceCategoryTranslation>();
    public ICollection<ProjectCategoryTranslation> ProjectCategoryTranslations { get; set; } = new List<ProjectCategoryTranslation>();
    public ICollection<NewsCategoryTranslation> NewsCategoryTranslations { get; set; } = new List<NewsCategoryTranslation>();
    public ICollection<MenuTranslation> MenuTranslations { get; set; } = new List<MenuTranslation>();
    public ICollection<SettingTranslation> SettingTranslations { get; set; } = new List<SettingTranslation>();
    public ICollection<FormTranslation> FormTranslations { get; set; } = new List<FormTranslation>();
    public ICollection<FormFieldTranslation> FormFieldTranslations { get; set; } = new List<FormFieldTranslation>();
}