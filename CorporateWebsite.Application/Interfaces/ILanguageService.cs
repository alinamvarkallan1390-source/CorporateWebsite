using CorporateWebsite.Core.Entities;
using CorporateWebsite.Core.DTOs;

namespace CorporateWebsite.Application.Interfaces;

public interface ILanguageService
{
    Task<LanguageDto?> GetByIdAsync(int id);
    Task<LanguageDto?> GetByCodeAsync(string code);
    Task<LanguageDto?> GetDefaultAsync();
    Task<IReadOnlyList<LanguageDto>> GetAllActiveAsync();
    Task<IReadOnlyList<LanguageDto>> GetAllAsync();
    Task<LanguageDto> CreateAsync(CreateLanguageDto dto);
    Task<LanguageDto> UpdateAsync(int id, UpdateLanguageDto dto);
    Task DeleteAsync(int id);
    Task SetDefaultAsync(int id);
    Task ReorderAsync(List<ReorderItemDto> items);
    Task<bool> ExistsCodeAsync(string code, int? excludeId = null);
    Task<LanguageDto?> GetByCultureCodeAsync(string cultureCode);
}

public interface IPageService
{
    Task<PageDto?> GetByIdAsync(int id, string? languageCode = null);
    Task<PageDto?> GetBySlugAsync(string slug, string languageCode);
    Task<IReadOnlyList<PageDto>> GetAllAsync(string? languageCode = null, bool? isPublished = null);
    Task<(IReadOnlyList<PageDto> Items, int TotalCount)> GetPagedAsync(PageFilterDto filter);
    Task<PageDto> CreateAsync(CreatePageDto dto);
    Task<PageDto> UpdateAsync(int id, UpdatePageDto dto);
    Task DeleteAsync(int id);
    Task PublishAsync(int id);
    Task UnpublishAsync(int id);
    Task ReorderAsync(List<ReorderItemDto> items);
    Task<bool> ExistsSlugAsync(string slug, int? excludeId = null, int? languageId = null);
    Task<IReadOnlyList<PageDto>> GetMenuItemsAsync(string menuName, string languageCode);
    Task<PageDto?> GetHomePageAsync(string languageCode);
    Task<IReadOnlyList<BreadcrumbDto>> GetBreadcrumbsAsync(int pageId, string languageCode);
}

public interface IServiceService
{
    Task<ServiceDto?> GetByIdAsync(int id, string? languageCode = null);
    Task<ServiceDto?> GetBySlugAsync(string slug, string languageCode);
    Task<IReadOnlyList<ServiceDto>> GetAllAsync(string? languageCode = null, bool? isPublished = null, int? categoryId = null);
    Task<(IReadOnlyList<ServiceDto> Items, int TotalCount)> GetPagedAsync(ServiceFilterDto filter);
    Task<ServiceDto> CreateAsync(CreateServiceDto dto);
    Task<ServiceDto> UpdateAsync(int id, UpdateServiceDto dto);
    Task DeleteAsync(int id);
    Task PublishAsync(int id);
    Task UnpublishAsync(int id);
    Task ReorderAsync(List<ReorderItemDto> items);
    Task<bool> ExistsSlugAsync(string slug, int? excludeId = null, int? languageId = null);
    Task IncrementViewCountAsync(int id);
    Task<IReadOnlyList<ServiceDto>> GetFeaturedAsync(string languageCode, int count = 6);
    Task<IReadOnlyList<ServiceDto>> GetRelatedAsync(int serviceId, string languageCode, int count = 4);
}

public interface IServiceCategoryService
{
    Task<ServiceCategoryDto?> GetByIdAsync(int id, string? languageCode = null);
    Task<ServiceCategoryDto?> GetBySlugAsync(string slug, string languageCode);
    Task<IReadOnlyList<ServiceCategoryDto>> GetAllAsync(string? languageCode = null, bool? isActive = null);
    Task<ServiceCategoryDto> CreateAsync(CreateServiceCategoryDto dto);
    Task<ServiceCategoryDto> UpdateAsync(int id, UpdateServiceCategoryDto dto);
    Task DeleteAsync(int id);
    Task ReorderAsync(List<ReorderItemDto> items);
    Task<bool> ExistsSlugAsync(string slug, int? excludeId = null, int? languageId = null);
    Task<IReadOnlyList<ServiceCategoryDto>> GetTreeAsync(string languageCode);
}

public interface IProjectService
{
    Task<ProjectDto?> GetByIdAsync(int id, string? languageCode = null);
    Task<ProjectDto?> GetBySlugAsync(string slug, string languageCode);
    Task<IReadOnlyList<ProjectDto>> GetAllAsync(string? languageCode = null, bool? isPublished = null, int? categoryId = null);
    Task<(IReadOnlyList<ProjectDto> Items, int TotalCount)> GetPagedAsync(ProjectFilterDto filter);
    Task<ProjectDto> CreateAsync(CreateProjectDto dto);
    Task<ProjectDto> UpdateAsync(int id, UpdateProjectDto dto);
    Task DeleteAsync(int id);
    Task PublishAsync(int id);
    Task UnpublishAsync(int id);
    Task ReorderAsync(List<ReorderItemDto> items);
    Task<bool> ExistsSlugAsync(string slug, int? excludeId = null, int? languageId = null);
    Task IncrementViewCountAsync(int id);
    Task<IReadOnlyList<ProjectDto>> GetFeaturedAsync(string languageCode, int count = 6);
    Task<IReadOnlyList<ProjectDto>> GetRelatedAsync(int projectId, string languageCode, int count = 4);
    Task<IReadOnlyList<ProjectDto>> GetByTagAsync(int tagId, string languageCode, int page = 1, int pageSize = 12);
}

public interface IProjectCategoryService
{
    Task<ProjectCategoryDto?> GetByIdAsync(int id, string? languageCode = null);
    Task<ProjectCategoryDto?> GetBySlugAsync(string slug, string languageCode);
    Task<IReadOnlyList<ProjectCategoryDto>> GetAllAsync(string? languageCode = null, bool? isActive = null);
    Task<ProjectCategoryDto> CreateAsync(CreateProjectCategoryDto dto);
    Task<ProjectCategoryDto> UpdateAsync(int id, UpdateProjectCategoryDto dto);
    Task DeleteAsync(int id);
    Task ReorderAsync(List<ReorderItemDto> items);
    Task<bool> ExistsSlugAsync(string slug, int? excludeId = null, int? languageId = null);
    Task<IReadOnlyList<ProjectCategoryDto>> GetTreeAsync(string languageCode);
}

public interface INewsService
{
    Task<NewsDto?> GetByIdAsync(int id, string? languageCode = null);
    Task<NewsDto?> GetBySlugAsync(string slug, string languageCode);
    Task<IReadOnlyList<NewsDto>> GetAllAsync(string? languageCode = null, bool? isPublished = null, int? categoryId = null);
    Task<(IReadOnlyList<NewsDto> Items, int TotalCount)> GetPagedAsync(NewsFilterDto filter);
    Task<NewsDto> CreateAsync(CreateNewsDto dto);
    Task<NewsDto> UpdateAsync(int id, UpdateNewsDto dto);
    Task DeleteAsync(int id);
    Task PublishAsync(int id);
    Task UnpublishAsync(int id);
    Task ScheduleAsync(int id, DateTime scheduledAt);
    Task<bool> ExistsSlugAsync(string slug, int? excludeId = null, int? languageId = null);
    Task IncrementViewCountAsync(int id);
    Task<IReadOnlyList<NewsDto>> GetFeaturedAsync(string languageCode, int count = 5);
    Task<IReadOnlyList<NewsDto>> GetBreakingAsync(string languageCode, int count = 3);
    Task<IReadOnlyList<NewsDto>> GetLatestAsync(string languageCode, int count = 10, int? excludeId = null);
    Task<IReadOnlyList<NewsDto>> GetRelatedAsync(int newsId, string languageCode, int count = 4);
    Task<IReadOnlyList<NewsDto>> GetByTagAsync(int tagId, string languageCode, int page = 1, int pageSize = 12);
    Task<IReadOnlyList<NewsDto>> GetByAuthorAsync(int authorId, string languageCode, int page = 1, int pageSize = 12);
    Task ProcessScheduledNewsAsync();
}

public interface INewsCategoryService
{
    Task<NewsCategoryDto?> GetByIdAsync(int id, string? languageCode = null);
    Task<NewsCategoryDto?> GetBySlugAsync(string slug, string languageCode);
    Task<IReadOnlyList<NewsCategoryDto>> GetAllAsync(string? languageCode = null, bool? isActive = null);
    Task<NewsCategoryDto> CreateAsync(CreateNewsCategoryDto dto);
    Task<NewsCategoryDto> UpdateAsync(int id, UpdateNewsCategoryDto dto);
    Task DeleteAsync(int id);
    Task ReorderAsync(List<ReorderItemDto> items);
    Task<bool> ExistsSlugAsync(string slug, int? excludeId = null, int? languageId = null);
    Task<IReadOnlyList<NewsCategoryDto>> GetTreeAsync(string languageCode);
    Task<IReadOnlyList<NewsCategoryDto>> GetMenuCategoriesAsync(string languageCode);
}

public interface ITagService
{
    Task<TagDto?> GetByIdAsync(int id, string? languageCode = null);
    Task<TagDto?> GetBySlugAsync(string slug, string languageCode);
    Task<IReadOnlyList<TagDto>> GetAllAsync(string? languageCode = null);
    Task<(IReadOnlyList<TagDto> Items, int TotalCount)> GetPagedAsync(TagFilterDto filter);
    Task<TagDto> CreateAsync(CreateTagDto dto);
    Task<TagDto> UpdateAsync(int id, UpdateTagDto dto);
    Task DeleteAsync(int id);
    Task<IReadOnlyList<TagDto>> GetPopularAsync(string languageCode, int count = 20);
    Task<IReadOnlyList<TagDto>> GetByEntityAsync(string entityType, int entityId, string languageCode);
}