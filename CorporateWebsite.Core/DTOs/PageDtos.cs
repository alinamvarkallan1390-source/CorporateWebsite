namespace CorporateWebsite.Core.DTOs;

public class PageDto
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Layout { get; set; } = "Default";
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public bool ShowInMenu { get; set; }
    public string? Icon { get; set; }
    public string? Template { get; set; }
    public string? CanonicalUrl { get; set; }
    public bool NoIndex { get; set; }
    public bool NoFollow { get; set; }
    public string? OgType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    public PageTranslationDto? Translation { get; set; }
    public ICollection<PageTranslationDto> Translations { get; set; } = new List<PageTranslationDto>();
    public ICollection<PageDto> Children { get; set; } = new List<PageDto>();
}

public class PageTranslationDto
{
    public int Id { get; set; }
    public int PageId { get; set; }
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
    public string? TwitterCard { get; set; }
    public string? TwitterTitle { get; set; }
    public string? TwitterDescription { get; set; }
    public string? TwitterImage { get; set; }
    public string? SchemaJson { get; set; }
    public string Slug { get; set; } = string.Empty;
}

public class CreatePageDto
{
    public string Slug { get; set; } = string.Empty;
    public string Layout { get; set; } = "Default";
    public int? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public bool ShowInMenu { get; set; } = true;
    public string? Icon { get; set; }
    public string? Template { get; set; }
    public string? CanonicalUrl { get; set; }
    public bool NoIndex { get; set; }
    public bool NoFollow { get; set; }
    public string? OgType { get; set; } = "website";
    public bool IsPublished { get; set; } = false;
    public List<CreatePageTranslationDto> Translations { get; set; } = new List<CreatePageTranslationDto>();
}

public class CreatePageTranslationDto
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
    public string? TwitterCard { get; set; }
    public string? TwitterTitle { get; set; }
    public string? TwitterDescription { get; set; }
    public string? TwitterImage { get; set; }
    public string? SchemaJson { get; set; }
    public string Slug { get; set; } = string.Empty;
}

public class UpdatePageDto
{
    public string Slug { get; set; } = string.Empty;
    public string Layout { get; set; } = "Default";
    public int? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public bool ShowInMenu { get; set; }
    public string? Icon { get; set; }
    public string? Template { get; set; }
    public string? CanonicalUrl { get; set; }
    public bool NoIndex { get; set; }
    public bool NoFollow { get; set; }
    public string? OgType { get; set; }
    public bool IsPublished { get; set; }
    public List<UpdatePageTranslationDto> Translations { get; set; } = new List<UpdatePageTranslationDto>();
}

public class UpdatePageTranslationDto
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
    public string? TwitterCard { get; set; }
    public string? TwitterTitle { get; set; }
    public string? TwitterDescription { get; set; }
    public string? TwitterImage { get; set; }
    public string? SchemaJson { get; set; }
    public string Slug { get; set; } = string.Empty;
}

public class PageFilterDto
{
    public string? Search { get; set; }
    public bool? IsPublished { get; set; }
    public int? ParentId { get; set; }
    public string? LanguageCode { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "DisplayOrder";
    public string? SortDirection { get; set; } = "asc";
}

public class BreadcrumbDto
{
    public string Title { get; set; } = string.Empty;
    public string? Url { get; set; }
    public bool IsCurrent { get; set; }
}