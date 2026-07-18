namespace CorporateWebsite.Core.DTOs;

public class ProjectDto
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? CategorySlug { get; set; }
    public string? ClientName { get; set; }
    public string? ClientUrl { get; set; }
    public string? Location { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Budget { get; set; }
    public string? MainImageUrl { get; set; }
    public string? VideoUrl { get; set; }
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
    public ProjectTranslationDto? Translation { get; set; }
    public ICollection<ProjectTranslationDto> Translations { get; set; } = new List<ProjectTranslationDto>();
    
    // Images
    public ICollection<ProjectImageDto> Images { get; set; } = new List<ProjectImageDto>();
    
    // Videos
    public ICollection<ProjectVideoDto> Videos { get; set; } = new List<ProjectVideoDto>();
    
    // Files
    public ICollection<ProjectFileDto> Files { get; set; } = new List<ProjectFileDto>();
    
    // Tags
    public ICollection<TagDto> Tags { get; set; } = new List<TagDto>();
    
    // Features
    public ICollection<ProjectFeatureDto> Features { get; set; } = new List<ProjectFeatureDto>();
    
    // Team Members
    public ICollection<ProjectTeamMemberDto> TeamMembers { get; set; } = new List<ProjectTeamMemberDto>();
}

public class ProjectTranslationDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
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

public class ProjectImageDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsMain { get; set; }
}

public class ProjectVideoDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int DisplayOrder { get; set; }
}

public class ProjectFileDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? FileType { get; set; }
    public long? FileSize { get; set; }
    public int DisplayOrder { get; set; }
}

public class ProjectFeatureDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public ProjectFeatureTranslationDto? Translation { get; set; }
    public ICollection<ProjectFeatureTranslationDto> Translations { get; set; } = new List<ProjectFeatureTranslationDto>();
}

public class ProjectFeatureTranslationDto
{
    public int Id { get; set; }
    public int FeatureId { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class ProjectTeamMemberDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int DisplayOrder { get; set; }
    public string? Role { get; set; }
    public string? ImageUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public ProjectTeamMemberTranslationDto? Translation { get; set; }
    public ICollection<ProjectTeamMemberTranslationDto> Translations { get; set; } = new List<ProjectTeamMemberTranslationDto>();
}

public class ProjectTeamMemberTranslationDto
{
    public int Id { get; set; }
    public int TeamMemberId { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateProjectDto
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
    public bool IsFeatured { get; set; }
    public string? CanonicalUrl { get; set; }
    public bool NoIndex { get; set; }
    public bool NoFollow { get; set; }
    public List<CreateProjectTranslationDto> Translations { get; set; } = new List<CreateProjectTranslationDto>();
    public List<CreateProjectImageDto> Images { get; set; } = new List<CreateProjectImageDto>();
    public List<CreateProjectVideoDto> Videos { get; set; } = new List<CreateProjectVideoDto>();
    public List<CreateProjectFileDto> Files { get; set; } = new List<CreateProjectFileDto>();
    public List<int> TagIds { get; set; } = new List<int>();
    public List<CreateProjectFeatureDto> Features { get; set; } = new List<CreateProjectFeatureDto>();
    public List<CreateProjectTeamMemberDto> TeamMembers { get; set; } = new List<CreateProjectTeamMemberDto>();
}

public class CreateProjectTranslationDto
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

public class CreateProjectImageDto
{
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsMain { get; set; }
}

public class CreateProjectVideoDto
{
    public string Url { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int DisplayOrder { get; set; }
}

public class CreateProjectFileDto
{
    public string Url { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? FileType { get; set; }
    public long? FileSize { get; set; }
    public int DisplayOrder { get; set; }
}

public class CreateProjectFeatureDto
{
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public List<CreateProjectFeatureTranslationDto> Translations { get; set; } = new List<CreateProjectFeatureTranslationDto>();
}

public class CreateProjectFeatureTranslationDto
{
    public int LanguageId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateProjectTeamMemberDto
{
    public int DisplayOrder { get; set; }
    public string? Role { get; set; }
    public string? ImageUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public List<CreateProjectTeamMemberTranslationDto> Translations { get; set; } = new List<CreateProjectTeamMemberTranslationDto>();
}

public class CreateProjectTeamMemberTranslationDto
{
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateProjectDto
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
    public bool IsPublished { get; set; }
    public bool IsFeatured { get; set; }
    public string? CanonicalUrl { get; set; }
    public bool NoIndex { get; set; }
    public bool NoFollow { get; set; }
    public List<UpdateProjectTranslationDto> Translations { get; set; } = new List<UpdateProjectTranslationDto>();
    public List<UpdateProjectImageDto> Images { get; set; } = new List<UpdateProjectImageDto>();
    public List<UpdateProjectVideoDto> Videos { get; set; } = new List<UpdateProjectVideoDto>();
    public List<UpdateProjectFileDto> Files { get; set; } = new List<UpdateProjectFileDto>();
    public List<int> TagIds { get; set; } = new List<int>();
    public List<UpdateProjectFeatureDto> Features { get; set; } = new List<UpdateProjectFeatureDto>();
    public List<UpdateProjectTeamMemberDto> TeamMembers { get; set; } = new List<UpdateProjectTeamMemberDto>();
}

public class UpdateProjectTranslationDto
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

public class UpdateProjectImageDto
{
    public int? Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsMain { get; set; }
}

public class UpdateProjectVideoDto
{
    public int? Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int DisplayOrder { get; set; }
}

public class UpdateProjectFileDto
{
    public int? Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? FileType { get; set; }
    public long? FileSize { get; set; }
    public int DisplayOrder { get; set; }
}

public class UpdateProjectFeatureDto
{
    public int? Id { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public List<UpdateProjectFeatureTranslationDto> Translations { get; set; } = new List<UpdateProjectFeatureTranslationDto>();
}

public class UpdateProjectFeatureTranslationDto
{
    public int? Id { get; set; }
    public int LanguageId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateProjectTeamMemberDto
{
    public int? Id { get; set; }
    public int DisplayOrder { get; set; }
    public string? Role { get; set; }
    public string? ImageUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public List<UpdateProjectTeamMemberTranslationDto> Translations { get; set; } = new List<UpdateProjectTeamMemberTranslationDto>();
}

public class UpdateProjectTeamMemberTranslationDto
{
    public int? Id { get; set; }
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class ProjectFilterDto
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

public class ProjectCategoryDto
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
    public ProjectCategoryTranslationDto? Translation { get; set; }
    public ICollection<ProjectCategoryTranslationDto> Translations { get; set; } = new List<ProjectCategoryTranslationDto>();
    
    // Children
    public ICollection<ProjectCategoryDto> Children { get; set; } = new List<ProjectCategoryDto>();
}

public class ProjectCategoryTranslationDto
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

public class CreateProjectCategoryDto
{
    public string Slug { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public List<CreateProjectCategoryTranslationDto> Translations { get; set; } = new List<CreateProjectCategoryTranslationDto>();
}

public class CreateProjectCategoryTranslationDto
{
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}

public class UpdateProjectCategoryDto
{
    public string Slug { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public List<UpdateProjectCategoryTranslationDto> Translations { get; set; } = new List<UpdateProjectCategoryTranslationDto>();
}

public class UpdateProjectCategoryTranslationDto
{
    public int? Id { get; set; }
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}