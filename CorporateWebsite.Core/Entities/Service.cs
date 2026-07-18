namespace CorporateWebsite.Core.Entities;

public class Service : BaseEntity
{
    public string Slug { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public string? Icon { get; set; }
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? FileUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; } = false;
    public DateTime? PublishedAt { get; set; }
    public bool IsFeatured { get; set; }
    public int ViewCount { get; set; } = 0;
    
    // SEO
    public string? CanonicalUrl { get; set; }
    public bool NoIndex { get; set; }
    public bool NoFollow { get; set; }
    
    // Navigation
    public ServiceCategory? Category { get; set; }
    public ICollection<ServiceTranslation> Translations { get; set; } = new List<ServiceTranslation>();
    public ICollection<ServiceFeature> Features { get; set; } = new List<ServiceFeature>();
    public ICollection<ServiceFaq> Faqs { get; set; } = new List<ServiceFaq>();
    public ICollection<ServiceImage> Images { get; set; } = new List<ServiceImage>();
}

public class ServiceTranslation : BaseEntity
{
    public int ServiceId { get; set; }
    public int LanguageId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public string? OgTitle { get; set; }
    public string? OgDescription { get; set; }
    public string? OgImage { get; set; }
    public string? SchemaJson { get; set; }
    
    // Navigation
    public Service Service { get; set; } = null!;
    public Language Language { get; set; } = null!;
}

public class ServiceCategory : BaseEntity
{
    public string Slug { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation
    public ServiceCategory? Parent { get; set; }
    public ICollection<ServiceCategory> Children { get; set; } = new List<ServiceCategory>();
    public ICollection<ServiceCategoryTranslation> Translations { get; set; } = new List<ServiceCategoryTranslation>();
    public ICollection<Service> Services { get; set; } = new List<Service>();
}

public class ServiceCategoryTranslation : BaseEntity
{
    public int CategoryId { get; set; }
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    
    // Navigation
    public ServiceCategory Category { get; set; } = null!;
    public Language Language { get; set; } = null!;
}

public class ServiceFeature : BaseEntity
{
    public int ServiceId { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    
    // Navigation
    public Service Service { get; set; } = null!;
    public ICollection<ServiceFeatureTranslation> Translations { get; set; } = new List<ServiceFeatureTranslation>();
}

public class ServiceFeatureTranslation : BaseEntity
{
    public int FeatureId { get; set; }
    public int LanguageId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Navigation
    public ServiceFeature Feature { get; set; } = null!;
    public Language Language { get; set; } = null!;
}

public class ServiceFaq : BaseEntity
{
    public int ServiceId { get; set; }
    public int DisplayOrder { get; set; }
    
    // Navigation
    public Service Service { get; set; } = null!;
    public ICollection<ServiceFaqTranslation> Translations { get; set; } = new List<ServiceFaqTranslation>();
}

public class ServiceFaqTranslation : BaseEntity
{
    public int FaqId { get; set; }
    public int LanguageId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    
    // Navigation
    public ServiceFaq Faq { get; set; } = null!;
    public Language Language { get; set; } = null!;
}

public class ServiceImage : BaseEntity
{
    public int ServiceId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsMain { get; set; }
    
    // Navigation
    public Service Service { get; set; } = null!;
}