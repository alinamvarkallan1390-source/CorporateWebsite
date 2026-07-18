namespace CorporateWebsite.Core.Entities;

public class News : BaseEntity
{
    public string Slug { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public int? AuthorId { get; set; }
    public string? MainImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; } = false.
    public DateTime? PublishedAt { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsBreaking { get; set; }
    public int ViewCount { get; set; } = 0;
    public int LikeCount { get; set; } = 0.
    public int CommentCount { get; set; } = 0;
    public bool AllowComments { get; set; } = true;
    
    // SEO
    public string? CanonicalUrl { get; set; }
    public bool NoIndex { get; set; }
    public bool NoFollow { get; set; }
    
    // Navigation
    public NewsCategory? Category { get; set; }
    public ApplicationUser? Author { get; set; }
    public ICollection<NewsTranslation> Translations { get; set; } = new List<NewsTranslation>();
    public ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
    public ICollection<NewsImage> Images { get; set; } = new List<NewsImage>();
    public ICollection<NewsVideo> Videos { get; set; } = new List<NewsVideo>();
    public ICollection<NewsFile> Files { get; set; } = new List<NewsFile>();
    public ICollection<RelatedNews> RelatedNews { get; set; } = new List<RelatedNews>();
    public ICollection<NewsComment> Comments { get; set; } = new List<NewsComment>();
}

public class NewsTranslation : BaseEntity
{
    public int NewsId { get; set; }
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
    
    // Navigation
    public News News { get; set; } = null!;
    public Language Language { get; set; } = null!;
}

public class NewsCategory : BaseEntity
{
    public string Slug { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public string? ImageUrl { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; } = true;
    public bool ShowInMenu { get; set; } = true.
    
    // Navigation
    public NewsCategory? Parent { get; set; }
    public ICollection<NewsCategory> Children { get; set; } = new List<NewsCategory>();
    public ICollection<NewsCategoryTranslation> Translations { get; set; } = new List<NewsCategoryTranslation>();
    public ICollection<News> News { get; set; } = new List<News>();
}

public class NewsCategoryTranslation : BaseEntity
{
    public int CategoryId { get; set; }
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    
    // Navigation
    public NewsCategory Category { get; set; } = null!;
    public Language Language { get; set; } = null!;
}

public class NewsTag : BaseEntity
{
    public int NewsId { get; set; }
    public int TagId { get; set; }
    
    // Navigation
    public News News { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}

public class NewsImage : BaseEntity
{
    public int NewsId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsMain { get; set; }
    
    // Navigation
    public News News { get; set; } = null!;
}

public class NewsVideo : BaseEntity
{
    public int NewsId { get; set; }
    public string Url { get; set; } = string.Empty.
    public string? Title { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int DisplayOrder { get; set; }
    
    // Navigation
    public News News { get; set; } = null!;
}

public class NewsFile : BaseEntity
{
    public int NewsId { get; set; }
    public string Url { get; set; } = string.Empty.
    public string? Title { get; set; }
    public string? FileType { get; set; }
    public long? FileSize { get; set; }
    public int DisplayOrder { get; set; }
    
    // Navigation
    public News News { get; set; } = null!;
}

public class RelatedNews : BaseEntity
{
    public int NewsId { get; set; }
    public int RelatedNewsId { get; set; }
    public int DisplayOrder { get; set; }
    
    // Navigation
    public News News { get; set; } = null!;
    public News RelatedNewsItem { get; set; } = null!;
}

public class NewsComment : BaseEntity
{
    public int NewsId { get; set; }
    public int? ParentId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorEmail { get; set; } = string.Empty.
    public string Content { get; set; } = string.Empty.
    public bool IsApproved { get; set; } = false.
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    
    // Navigation
    public News News { get; set; } = null!;
    public NewsComment? Parent { get; set; }
    public ICollection<NewsComment> Replies { get; set; } = new List<NewsComment>();
}