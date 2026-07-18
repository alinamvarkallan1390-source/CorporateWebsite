namespace CorporateWebsite.Core.DTOs;

public class ServiceDto
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? CategorySlug { get; set; }
    public string? Icon { get; set; }
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? FileUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public bool IsFeatured { get; set; }
    public int ViewCount { get; set; }
    public string? CanonicalUrl { get; set; }
    public bool NoIndex { get; set; }
    public bool NoFollow { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Translations
    public ServiceTranslationDto? Translation { get; set; }
    public ICollection<ServiceTranslationDto> Translations { get; set; } = new List<ServiceTranslationDto>();
    
    // Features
    public ICollection<ServiceFeatureDto> Features { get; set; } = new List<ServiceFeatureDto>();
    
    // FAQs
    public ICollection<ServiceFaqDto> Faqs { get; set; } = new List<ServiceFaqDto>();
    
    // Images
    public ICollection<ServiceImageDto> Images { get; set; } = new List<ServiceImageDto>();
}

public class ServiceTranslationDto
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
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
}

public class ServiceFeatureDto
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public ServiceFeatureTranslationDto? Translation { get; set; }
    public ICollection<ServiceFeatureTranslationDto> Translations { get; set; } = new List<ServiceFeatureTranslationDto>();
}

public class ServiceFeatureTranslationDto
{
    public int Id { get; set; }
    public int FeatureId { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class ServiceFaqDto
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public int DisplayOrder { get; set; }
    public ServiceFaqTranslationDto? Translation { get; set; }
    public ICollection<ServiceFaqTranslationDto> Translations { get; set; } = new List<ServiceFaqTranslationDto>();
}

public class ServiceFaqTranslationDto
{
    public int Id { get; set; }
    public int FaqId { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}

public class ServiceImageDto
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsMain { get; set; }
}

public class CreateServiceDto
{
    public string Slug { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public string? Icon { get; set; }
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? FileUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; } = false;
    public bool IsFeatured { get; set; }
    public string? CanonicalUrl { get; set; }
    public bool NoIndex { get; set; }
    public bool NoFollow { get; set; }
    public List<CreateServiceTranslationDto> Translations { get; set; } = new List<CreateServiceTranslationDto>();
    public List<CreateServiceFeatureDto> Features { get; set; } = new List<CreateServiceFeatureDto>();
    public List<CreateServiceFaqDto> Faqs { get; set; } = new List<CreateServiceFaqDto>();
    public List<CreateServiceImageDto> Images { get; set; } = new List<CreateServiceImageDto>();
}

public class CreateServiceTranslationDto
{
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
}

public class CreateServiceFeatureDto
{
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public List<CreateServiceFeatureTranslationDto> Translations { get; set; } = new List<CreateServiceFeatureTranslationDto>();
}

public class CreateServiceFeatureTranslationDto
{
    public int LanguageId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateServiceFaqDto
{
    public int DisplayOrder { get; set; }
    public List<CreateServiceFaqTranslationDto> Translations { get; set; } = new List<CreateServiceFaqTranslationDto>();
}

public class CreateServiceFaqTranslationDto
{
    public int LanguageId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}

public class CreateServiceImageDto
{
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsMain { get; set; }
}

public class UpdateServiceDto
{
    public string Slug { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public string? Icon { get; set; }
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? FileUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; }
    public bool IsFeatured { get; set; }
    public string? CanonicalUrl { get; set; }
    public bool NoIndex { get; set; }
    public bool NoFollow { get; set; }
    public List<UpdateServiceTranslationDto> Translations { get; set; } = new List<UpdateServiceTranslationDto>();
    public List<UpdateServiceFeatureDto> Features { get; set; } = new List<UpdateServiceFeatureDto>();
    public List<UpdateServiceFaqDto> Faqs { get; set; } = new List<UpdateServiceFaqDto>();
    public List<UpdateServiceImageDto> Images { get; set; } = new List<UpdateServiceImageDto>();
}

public class UpdateServiceTranslationDto
{
    public int? Id { get; set; }
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
}

public class UpdateServiceFeatureDto
{
    public int? Id { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public List<UpdateServiceFeatureTranslationDto> Translations { get; set; } = new List<UpdateServiceFeatureTranslationDto>();
}

public class UpdateServiceFeatureTranslationDto
{
    public int? Id { get; set; }
    public int LanguageId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateServiceFaqDto
{
    public int? Id { get; set; }
    public int DisplayOrder { get; set; }
    public List<UpdateServiceFaqTranslationDto> Translations { get; set; } = new List<UpdateServiceFaqTranslationDto>();
}

public class UpdateServiceFaqTranslationDto
{
    public int? Id { get; set; }
    public int LanguageId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}

public class UpdateServiceImageDto
{
    public int? Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsMain { get; set; }
}

public class ServiceFilterDto
{
    public string? Search { get; set; }
    public bool? IsPublished { get; set; }
    public int? CategoryId { get; set; }
    public bool? IsFeatured { get; set; }
    public string? LanguageCode { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "DisplayOrder";
    public string? SortDirection { get; set; } = "asc";
}

public class ServiceCategoryDto
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Translations
    public ServiceCategoryTranslationDto? Translation { get; set; }
    public ICollection<ServiceCategoryTranslationDto> Translations { get; set; } = new List<ServiceCategoryTranslationDto>();
    
    // Children
    public ICollection<ServiceCategoryDto> Children { get; set; } = new List<ServiceCategoryDto>();
}

public class ServiceCategoryTranslationDto
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}

public class CreateServiceCategoryDto
{
    public string Slug { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public List<CreateServiceCategoryTranslationDto> Translations { get; set; } = new List<CreateServiceCategoryTranslationDto>();
}

public class CreateServiceCategoryTranslationDto
{
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}

public class UpdateServiceCategoryDto
{
    public string Slug { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public List<UpdateServiceCategoryTranslationDto> Translations { get; set; } = new List<UpdateServiceCategoryTranslationDto>();
}

public class UpdateServiceCategoryTranslationDto
{
    public int? Id { get; set; }
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}