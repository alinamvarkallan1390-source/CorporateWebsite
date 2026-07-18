using CorporateWebsite.Core.DTOs;
using CorporateWebsite.Core.Entities;
using CorporateWebsite.Core.Interfaces;
using CorporateWebsite.Application.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace CorporateWebsite.Application.Services;

public class PageService : IPageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILanguageService _languageService;

    public PageService(IUnitOfWork unitOfWork, IMapper mapper, ILanguageService languageService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _languageService = languageService;
    }

    public async Task<PageDto?> GetByIdAsync(int id, string? languageCode = null)
    {
        var page = await _unitOfWork.Pages.GetByIdAsync(id, p => p.Translations);
        if (page == null) return null;

        var dto = _mapper.Map<PageDto>(page);
        
        // Set the appropriate translation based on language
        if (!string.IsNullOrEmpty(languageCode))
        {
            var language = await _languageService.GetByCodeAsync(languageCode);
            if (language != null)
            {
                dto.Translation = dto.Translations.FirstOrDefault(t => t.LanguageId == language.Id);
            }
        }
        else
        {
            var defaultLang = await _languageService.GetDefaultAsync();
            if (defaultLang != null)
            {
                dto.Translation = dto.Translations.FirstOrDefault(t => t.LanguageId == defaultLang.Id);
            }
        }

        return dto;
    }

    public async Task<PageDto?> GetBySlugAsync(string slug, string languageCode)
    {
        var language = await _languageService.GetByCodeAsync(languageCode);
        if (language == null) return null;

        var translation = await _unitOfWork.PageTranslations.FirstOrDefaultAsync(
            t => t.Slug == slug && t.LanguageId == language.Id,
            t => t.Page);
        
        if (translation?.Page == null) return null;

        var page = translation.Page;
        var dto = _mapper.Map<PageDto>(page);
        dto.Translation = _mapper.Map<PageTranslationDto>(translation);
        
        return dto;
    }

    public async Task<IReadOnlyList<PageDto>> GetAllAsync(string? languageCode = null, bool? isPublished = null)
    {
        var predicate = BuildPredicate(languageCode, isPublished);
        var pages = await _unitOfWork.Pages.GetAllAsync(predicate, q => q.OrderBy(p => p.DisplayOrder), p => p.Translations);
        
        var dtos = _mapper.Map<IReadOnlyList<PageDto>>(pages);
        
        // Set translations
        if (!string.IsNullOrEmpty(languageCode))
        {
            var language = await _languageService.GetByCodeAsync(languageCode);
            if (language != null)
            {
                foreach (var dto in dtos)
                {
                    dto.Translation = dto.Translations.FirstOrDefault(t => t.LanguageId == language.Id);
                }
            }
        }

        return dtos;
    }

    public async Task<(IReadOnlyList<PageDto> Items, int TotalCount)> GetPagedAsync(PageFilterDto filter)
    {
        var predicate = BuildPredicate(filter.LanguageCode, filter.IsPublished);
        
        if (filter.ParentId.HasValue)
        {
            var parentId = filter.ParentId.Value;
            predicate = CombinePredicates(predicate, p => p.ParentId == parentId);
        }

        if (!string.IsNullOrEmpty(filter.Search))
        {
            var search = filter.Search.ToLower();
            predicate = CombinePredicates(predicate, p => 
                p.Translations.Any(t => t.Title.ToLower().Contains(search) || 
                                       t.Content.ToLower().Contains(search)));
        }

        var (items, totalCount) = await _unitOfWork.Pages.GetPagedAsync(
            filter.Page, 
            filter.PageSize, 
            predicate, 
            BuildOrderBy(filter.SortBy, filter.SortDirection),
            p => p.Translations);

        var dtos = _mapper.Map<IReadOnlyList<PageDto>>(items);
        
        // Set translations
        if (!string.IsNullOrEmpty(filter.LanguageCode))
        {
            var language = await _languageService.GetByCodeAsync(filter.LanguageCode);
            if (language != null)
            {
                foreach (var dto in dtos)
                {
                    dto.Translation = dto.Translations.FirstOrDefault(t => t.LanguageId == language.Id);
                }
            }
        }

        return (dtos, totalCount);
    }

    public async Task<PageDto> CreateAsync(CreatePageDto dto)
    {
        if (await ExistsSlugAsync(dto.Slug))
        {
            throw new InvalidOperationException($"Page with slug '{dto.Slug}' already exists.");
        }

        var page = _mapper.Map<Page>(dto);
        page.CreatedAt = DateTime.UtcNow;
        
        // Map translations
        foreach (var transDto in dto.Translations)
        {
            var translation = _mapper.Map<PageTranslation>(transDto);
            translation.Page = page;
            page.Translations.Add(translation);
        }

        await _unitOfWork.Pages.AddAsync(page);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<PageDto>(page);
    }

    public async Task<PageDto> UpdateAsync(int id, UpdatePageDto dto)
    {
        var page = await _unitOfWork.Pages.GetByIdAsync(id, p => p.Translations);
        if (page == null)
        {
            throw new KeyNotFoundException($"Page with id {id} not found.");
        }

        if (page.Slug != dto.Slug && await ExistsSlugAsync(dto.Slug, id))
        {
            throw new InvalidOperationException($"Page with slug '{dto.Slug}' already exists.");
        }

        _mapper.Map(dto, page);
        page.UpdatedAt = DateTime.UtcNow;

        // Update translations
        foreach (var transDto in dto.Translations)
        {
            if (transDto.Id.HasValue)
            {
                var translation = page.Translations.FirstOrDefault(t => t.Id == transDto.Id.Value);
                if (translation != null)
                {
                    _mapper.Map(transDto, translation);
                    translation.UpdatedAt = DateTime.UtcNow;
                }
            }
            else
            {
                var translation = _mapper.Map<PageTranslation>(transDto);
                translation.PageId = page.Id;
                page.Translations.Add(translation);
            }
        }

        await _unitOfWork.Pages.UpdateAsync(page);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<PageDto>(page);
    }

    public async Task DeleteAsync(int id)
    {
        var page = await _unitOfWork.Pages.GetByIdAsync(id);
        if (page == null)
        {
            throw new KeyNotFoundException($"Page with id {id} not found.");
        }

        // Check for children
        var hasChildren = await _unitOfWork.Pages.ExistsAsync(p => p.ParentId == id);
        if (hasChildren)
        {
            throw new InvalidOperationException("Cannot delete page that has child pages. Delete children first.");
        }

        await _unitOfWork.Pages.DeleteAsync(page);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task PublishAsync(int id)
    {
        var page = await _unitOfWork.Pages.GetByIdAsync(id);
        if (page == null)
        {
            throw new KeyNotFoundException($"Page with id {id} not found.");
        }

        page.IsPublished = true;
        page.PublishedAt = DateTime.UtcNow;
        page.UpdatedAt = DateTime.UtcNow;
        
        await _unitOfWork.Pages.UpdateAsync(page);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UnpublishAsync(int id)
    {
        var page = await _unitOfWork.Pages.GetByIdAsync(id);
        if (page == null)
        {
            throw new KeyNotFoundException($"Page with id {id} not found.");
        }

        page.IsPublished = false;
        page.UpdatedAt = DateTime.UtcNow;
        
        await _unitOfWork.Pages.UpdateAsync(page);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ReorderAsync(List<ReorderItemDto> items)
    {
        foreach (var item in items)
        {
            var page = await _unitOfWork.Pages.GetByIdAsync(item.Id);
            if (page != null)
            {
                page.DisplayOrder = item.DisplayOrder;
                page.ParentId = item.ParentId;
                page.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Pages.UpdateAsync(page);
            }
        }
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> ExistsSlugAsync(string slug, int? excludeId = null, int? languageId = null)
    {
        if (languageId.HasValue)
        {
            return await _unitOfWork.PageTranslations.ExistsAsync(
                t => t.Slug == slug && t.LanguageId == languageId.Value && (!excludeId.HasValue || t.PageId != excludeId.Value));
        }
        
        return await _unitOfWork.PageTranslations.ExistsAsync(
            t => t.Slug == slug && (!excludeId.HasValue || t.PageId != excludeId.Value));
    }

    public async Task<IReadOnlyList<PageDto>> GetMenuItemsAsync(string menuName, string languageCode)
    {
        var menu = await _unitOfWork.Menus.FirstOrDefaultAsync(m => m.Name == menuName, m => m.Items.Where(i => i.IsActive).OrderBy(i => i.DisplayOrder).ThenBy(i => i.Id));
        if (menu == null) return new List<PageDto>();

        var language = await _languageService.GetByCodeAsync(languageCode);
        if (language == null) return new List<PageDto>();

        var pageIds = menu.Items.Where(i => i.PageId.HasValue).Select(i => i.PageId.Value).ToList();
        var pages = await _unitOfWork.Pages.GetAllAsync(p => pageIds.Contains(p.Id), q => q.OrderBy(p => menu.Items.First(i => i.PageId == p.Id).DisplayOrder), p => p.Translations);
        
        var dtos = _mapper.Map<IReadOnlyList<PageDto>>(pages);
        
        foreach (var dto in dtos)
        {
            dto.Translation = dto.Translations.FirstOrDefault(t => t.LanguageId == language.Id);
        }

        return dtos;
    }

    public async Task<PageDto?> GetHomePageAsync(string languageCode)
    {
        var language = await _languageService.GetByCodeAsync(languageCode);
        if (language == null) return null;

        // Get home page (usually slug = "home" or parentId = null and displayOrder = 0)
        var translation = await _unitOfWork.PageTranslations.FirstOrDefaultAsync(
            t => t.Slug == "home" && t.LanguageId == language.Id,
            t => t.Page);
        
        if (translation?.Page != null)
        {
            var dto = _mapper.Map<PageDto>(translation.Page);
            dto.Translation = _mapper.Map<PageTranslationDto>(translation);
            return dto;
        }

        // Fallback to first published page
        var pages = await _unitOfWork.Pages.GetAllAsync(p => p.IsPublished && p.ParentId == null, q => q.OrderBy(p => p.DisplayOrder), p => p.Translations);
        var firstPage = pages.FirstOrDefault();
        
        if (firstPage != null)
        {
            var dto = _mapper.Map<PageDto>(firstPage);
            dto.Translation = dto.Translations.FirstOrDefault(t => t.LanguageId == language.Id);
            return dto;
        }

        return null;
    }

    public async Task<IReadOnlyList<BreadcrumbDto>> GetBreadcrumbsAsync(int pageId, string languageCode)
    {
        var language = await _languageService.GetByCodeAsync(languageCode);
        if (language == null) return new List<BreadcrumbDto>();

        var breadcrumbs = new List<BreadcrumbDto>();
        var currentPage = await _unitOfWork.Pages.GetByIdAsync(pageId, p => p.Translations);
        
        while (currentPage != null)
        {
            var translation = currentPage.Translations.FirstOrDefault(t => t.LanguageId == language.Id);
            if (translation != null)
            {
                breadcrumbs.Insert(0, new BreadcrumbDto
                {
                    Title = translation.Title,
                    Url = $"/{language.Code}/{translation.Slug}",
                    IsCurrent = currentPage.Id == pageId
                });
            }
            
            if (currentPage.ParentId.HasValue)
            {
                currentPage = await _unitOfWork.Pages.GetByIdAsync(currentPage.ParentId.Value, p => p.Translations);
            }
            else
            {
                break;
            }
        }

        return breadcrumbs;
    }

    private Expression<Func<Page, bool>> BuildPredicate(string? languageCode, bool? isPublished)
    {
        Expression<Func<Page, bool>> predicate = p => true;

        if (isPublished.HasValue)
        {
            predicate = CombinePredicates(predicate, p => p.IsPublished == isPublished.Value);
        }

        return predicate;
    }

    private Expression<Func<Page, bool>> CombinePredicates(Expression<Func<Page, bool>> first, Expression<Func<Page, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(Page));
        var firstBody = Expression.Invoke(first, parameter);
        var secondBody = Expression.Invoke(second, parameter);
        var combined = Expression.AndAlso(firstBody, secondBody);
        return Expression.Lambda<Func<Page, bool>>(combined, parameter);
    }

    private Func<IQueryable<Page>, IOrderedQueryable<Page>> BuildOrderBy(string? sortBy, string? sortDirection)
    {
        var isDesc = sortDirection?.ToLower() == "desc";
        
        return sortBy?.ToLower() switch
        {
            "title" => isDesc ? q => q.OrderByDescending(p => p.Translations.FirstOrDefault()!.Title) : q => q.OrderBy(p => p.Translations.FirstOrDefault()!.Title),
            "createdat" => isDesc ? q => q.OrderByDescending(p => p.CreatedAt) : q => q.OrderBy(p => p.CreatedAt),
            "updatedat" => isDesc ? q => q.OrderByDescending(p => p.UpdatedAt) : q => q.OrderBy(p => p.UpdatedAt),
            "publishedat" => isDesc ? q => q.OrderByDescending(p => p.PublishedAt) : q => q.OrderBy(p => p.PublishedAt),
            _ => isDesc ? q => q.OrderByDescending(p => p.DisplayOrder) : q => q.OrderBy(p => p.DisplayOrder)
        };
    }
}