namespace CorporateWebsite.Core.Entities;

public class Project : BaseEntity
{
    public string Slug { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public string? ClientName { get; set; }
    public string? ClientUrl { get; set; }
    public string? Location { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Budget { get; set; }
    public string? MainImageUrl { get; set; }
    public string? VideoUrl { get; set; }
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
    public ProjectCategory? Category { get; set; }
    public ICollection<ProjectTranslation> Translations { get; set; } = new List<ProjectTranslation>();
    public ICollection<ProjectImage> Images { get; set; } = new List<ProjectImage>();
    public ICollection<ProjectVideo> Videos { get; set; } = new List<ProjectVideo>();
    public ICollection<ProjectFile> Files { get; set; } = new List<ProjectFile>();
    public ICollection<ProjectTag> ProjectTags { get; set; } = new List<ProjectTag>();
    public ICollection<ProjectFeature> Features { get; set; } = new List<ProjectFeature>();
    public ICollection<ProjectTeamMember> TeamMembers { get; set; } = new List<ProjectTeamMember>();
}

public class ProjectTranslation : BaseEntity
{
    public int ProjectId { get; set; }
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
    public Project Project { get; set; } = null!;
    public Language Language { get; set; } = null!;
}

public class ProjectCategory : BaseEntity
{
    public string Slug { get; set; } = string.Empty.
    public int? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation
    public ProjectCategory? Parent { get; set; }
    public ICollection<ProjectCategory> Children { get; set; } = new List<ProjectCategory>();
    public ICollection<ProjectCategoryTranslation> Translations { get; set; } = new List<ProjectCategoryTranslation>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}

public class ProjectCategoryTranslation : BaseEntity
{
    public int CategoryId { get; set; }
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    
    // Navigation
    public ProjectCategory Category { get; set; } = null!;
    public Language Language { get; set; } = null!;
}

public class ProjectImage : BaseEntity
{
    public int ProjectId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsMain { get; set; }
    
    // Navigation
    public Project Project { get; set; } = null!;
}

public class ProjectVideo : BaseEntity
{
    public int ProjectId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int DisplayOrder { get; set; }
    
    // Navigation
    public Project Project { get; set; } = null!;
}

public class ProjectFile : BaseEntity
{
    public int ProjectId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? FileType { get; set; }
    public long? FileSize { get; set; }
    public int DisplayOrder { get; set; }
    
    // Navigation
    public Project Project { get; set; } = null!;
}

public class Tag : BaseEntity
{
    public string Slug { get; set; } = string.Empty;
    public int UsageCount { get; set; } = 0;
    
    // Navigation
    public ICollection<TagTranslation> Translations { get; set; } = new List<TagTranslation>();
    public ICollection<ProjectTag> ProjectTags { get; set; } = new List<ProjectTag>();
    public ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
}

public class TagTranslation : BaseEntity
{
    public int TagId { get; set; }
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    
    // Navigation
    public Tag Tag { get; set; } = null!;
    public Language Language { get; set; } = null!;
}

public class ProjectTag : BaseEntity
{
    public int ProjectId { get; set; }
    public int TagId { get; set; }
    
    // Navigation
    public Project Project { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}

public class ProjectFeature : BaseEntity
{
    public int ProjectId { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    
    // Navigation
    public Project Project { get; set; } = null!;
    public ICollection<ProjectFeatureTranslation> Translations { get; set; } = new List<ProjectFeatureTranslation>();
}

public class ProjectFeatureTranslation : BaseEntity
{
    public int FeatureId { get; set; }
    public int LanguageId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Navigation
    public ProjectFeature Feature { get; set; } = null!;
    public Language Language { get; set; } = null!;
}

public class ProjectTeamMember : BaseEntity
{
    public int ProjectId { get; set; }
    public int DisplayOrder { get; set; }
    public string? Role { get; set; }
    public string? ImageUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    
    // Navigation
    public Project Project { get; set; } = null!;
    public ICollection<ProjectTeamMemberTranslation> Translations { get; set; } = new List<ProjectTeamMemberTranslation>();
}

public class ProjectTeamMemberTranslation : BaseEntity
{
    public int TeamMemberId { get; set; }
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Navigation
    public ProjectTeamMember TeamMember { get; set; } = null!;
    public Language Language { get; set; } = null!;
}