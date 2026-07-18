namespace CorporateWebsite.Core.DTOs;

public class NewsDto
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? CategorySlug { get; set; }
    public int? AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public string? MainImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsBreaking { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public bool AllowComments { get; set; }
    public string? CanonicalUrl { get; set; }
    public bool NoIndex { get; set; }
    public bool NoFollow { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Translations
    public NewsTranslationDto? Translation { get; set; }
    public ICollection<NewsTranslationDto> Translations { get; set; } = new List<NewsTranslationDto>();
    
    // Tags
    public ICollection<TagDto> Tags { get; set; } = new List<TagDto>();
    
    // Images
    public ICollection<NewsImageDto> Images { get; set; } = new List<NewsImageDto>();
    
    // Videos
    public ICollection<NewsVideoDto> Videos { get; set; } = new List<NewsVideoDto>();
    
    // Files
    public ICollection<NewsFileDto> Files { get; set; } = new List<NewsFileDto>();
}

public class NewsTranslationDto
{
    public int Id { get; set; }
    public int NewsId { get; set; }
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
}

public class NewsImageDto
{
    public int Id { get; set; }
    public int NewsId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsMain { get; set; }
}

public class NewsVideoDto
{
    public int Id { get; set; }
    public int NewsId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int DisplayOrder { get; set; }
}

public class NewsFileDto
{
    public int Id { get; set; }
    public int NewsId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? FileType { get; set; }
    public long? FileSize { get; set; }
    public int DisplayOrder { get; set; }
}

public class CreateNewsDto
{
    public string Slug { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public string? MainImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; } = false;
    public DateTime? ScheduledAt { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsBreaking { get; set; }
    public bool AllowComments { get; set; } = true;
    public string? CanonicalUrl { get; set; }
    public bool NoIndex { get; set; }
    public bool NoFollow { get; set; }
    public List<CreateNewsTranslationDto> Translations { get; set; } = new List<CreateNewsTranslationDto>();
    public List<int> TagIds { get; set; } = new List<int>();
    public List<CreateNewsImageDto> Images { get; set; } = new List<CreateNewsImageDto>();
    public List<CreateNewsVideoDto> Videos { get; set; } = new List<CreateNewsVideoDto>();
    public List<CreateNewsFileDto> Files { get; set; } = new List<CreateNewsFileDto>();
    public List<int> RelatedNewsIds { get; set; } = new List<int>();
}

public class CreateNewsTranslationDto
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
}

public class CreateNewsImageDto
{
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsMain { get; set; }
}

public class CreateNewsVideoDto
{
    public string Url { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int DisplayOrder { get; set; }
}

public class CreateNewsFileDto
{
    public string Url { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? FileType { get; set; }
    public long? FileSize { get; set; }
    public int DisplayOrder { get; set; }
}

public class UpdateNewsDto
{
    public string Slug { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public string? MainImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsBreaking { get; set; }
    public bool AllowComments { get; set; }
    public string? CanonicalUrl { get; set; }
    public bool NoIndex { get; set; }
    public bool NoFollow { get; set; }
    public List<UpdateNewsTranslationDto> Translations { get; set; } = new List<UpdateNewsTranslationDto>();
    public List<int> TagIds { get; set; } = new List<int>();
    public List<UpdateNewsImageDto> Images { get; set; } = new List<UpdateNewsImageDto>();
    public List<UpdateNewsVideoDto> Videos { get; set; } = new List<UpdateNewsVideoDto>();
    public List<UpdateNewsFileDto> Files { get; set; } = new List<UpdateNewsFileDto>();
    public List<int> RelatedNewsIds { get; set; } = new List<int>();
}

public class UpdateNewsTranslationDto
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
}

public class UpdateNewsImageDto
{
    public int? Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsMain { get; set; }
}

public class UpdateNewsVideoDto
{
    public int? Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int DisplayOrder { get; set; }
}

public class UpdateNewsFileDto
{
    public int? Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? FileType { get; set; }
    public long? FileSize { get; set; }
    public int DisplayOrder { get; set; }
}

public class NewsFilterDto
{
    public string? Search { get; set; }
    public bool? IsPublished { get; set; }
    public int? CategoryId { get; set; }
    public bool? IsFeatured { get; set; }
    public bool? IsBreaking { get; set; }
    public int? AuthorId { get; set; }
    public string? LanguageCode { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "PublishedAt";
    public string? SortDirection { get; set; } = "desc";
}

public class NewsCategoryDto
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public string? ImageUrl { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; }
    public bool ShowInMenu { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Translations
    public NewsCategoryTranslationDto? Translation { get; set; }
    public ICollection<NewsCategoryTranslationDto> Translations { get; set; } = new List<NewsCategoryTranslationDto>();
    
    // Children
    public ICollection<NewsCategoryDto> Children { get; set; } = new List<NewsCategoryDto>();
}

public class NewsCategoryTranslationDto
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

public class CreateNewsCategoryDto
{
    public string Slug { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public string? ImageUrl { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; } = true;
    public bool ShowInMenu { get; set; } = true;
    public List<CreateNewsCategoryTranslationDto> Translations { get; set; } = new List<CreateNewsCategoryTranslationDto>();
}

public class CreateNewsCategoryTranslationDto
{
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}

public class UpdateNewsCategoryDto
{
    public string Slug { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public string? ImageUrl { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; }
    public bool ShowInMenu { get; set; }
    public List<UpdateNewsCategoryTranslationDto> Translations { get; set; } = new List<UpdateNewsCategoryTranslationDto>();
}

public class UpdateNewsCategoryTranslationDto
{
    public int? Id { get; set; }
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}

public class TagDto
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public int UsageCount { get; set; }
    public TagTranslationDto? Translation { get; set; }
    public ICollection<TagTranslationDto> Translations { get; set; } = new List<TagTranslationDto>();
}

public class TagTranslationDto
{
    public int Id { get; set; }
    public int TagId { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class CreateTagDto
{
    public string Slug { get; set; } = string.Empty;
    public List<CreateTagTranslationDto> Translations { get; set; } = new List<CreateTagTranslationDto>();
}

public class CreateTagTranslationDto
{
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class UpdateTagDto
{
    public string Slug { get; set; } = string.Empty;
    public List<UpdateTagTranslationDto> Translations { get; set; } = new List<UpdateTagTranslationDto>();
}

public class UpdateTagTranslationDto
{
    public int? Id { get; set; }
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class TagFilterDto
{
    public string? Search { get; set; }
    public string? LanguageCode { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "UsageCount";
    public string? SortDirection { get; set; } = "desc";
}