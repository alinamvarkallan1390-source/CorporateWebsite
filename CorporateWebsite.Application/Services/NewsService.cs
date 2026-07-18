using CorporateWebsite.Core.DTOs;
using CorporateWebsite.Core.Entities;
using CorporateWebsite.Core.Interfaces;
using CorporateWebsite.Application.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace CorporateWebsite.Application.Services;

public class NewsService : INewsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILanguageService _languageService;

    public NewsService(IUnitOfWork unitOfWork, IMapper mapper, ILanguageService languageService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _languageService = languageService;
    }

    public async Task<NewsDto?> GetByIdAsync(int id, string? languageCode = null)
    {
        var news = await _unitOfWork.News.GetByIdAsync(id,
            n => n.Translations,
            n => n.NewsTags,
            n => n.Images,
            n => n.Videos,
            n => n.Files);
        
        if (news == null) return null;

        var dto = _mapper.Map<NewsDto>(news);
        await SetTranslationAsync(dto, languageCode);
        
        return dto;
    }

    public async Task<NewsDto?> GetBySlugAsync(string slug, string languageCode)
    {
        var language = await _languageService.GetByCodeAsync(languageCode);
        if (language == null) return null;

        var translation = await _unitOfWork.NewsTranslations.FirstOrDefaultAsync(
            t => t.Slug == slug && t.LanguageId == language.Id,
            t => t.News);
        
        if (translation?.News == null) return null;

        var news = translation.News;
        var dto = _mapper.Map<NewsDto>(news);
        dto.Translation = _mapper.Map<NewsTranslationDto>(translation);
        
        return dto;
    }

    public async Task<IReadOnlyList<NewsDto>> GetAllAsync(string? languageCode = null, bool? isPublished = null, int? categoryId = null)
    {
        var predicate = BuildPredicate(languageCode, isPublished, categoryId);
        var news = await _unitOfWork.News.GetAllAsync(predicate, q => q.OrderByDescending(n => n.PublishedAt),
            n => n.Translations, n => n.NewsTags, n => n.Images, n => n.Videos, n => n.Files);
        
        var dtos = _mapper.Map<IReadOnlyList<NewsDto>>(news);
        await SetTranslationsAsync(dtos, languageCode);
        
        return dtos;
    }

    public async Task<(IReadOnlyList<NewsDto> Items, int TotalCount)> GetPagedAsync(NewsFilterDto filter)
    {
        var predicate = BuildPredicate(filter.LanguageCode, filter.IsPublished, filter.CategoryId);
        
        if (filter.IsFeatured.HasValue)
        {
            predicate = CombinePredicates(predicate, n => n.IsFeatured == filter.IsFeatured.Value);
        }

        if (filter.IsBreaking.HasValue)
        {
            predicate = CombinePredicates(predicate, n => n.IsBreaking == filter.IsBreaking.Value);
        }

        if (filter.AuthorId.HasValue)
        {
            predicate = CombinePredicates(predicate, n => n.AuthorId == filter.AuthorId.Value);
        }

        if (filter.FromDate.HasValue)
        {
            predicate = CombinePredicates(predicate, n => n.PublishedAt >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            predicate = CombinePredicates(predicate, n => n.PublishedAt <= filter.ToDate.Value);
        }

        if (!string.IsNullOrEmpty(filter.Search))
        {
            var search = filter.Search.ToLower();
            predicate = CombinePredicates(predicate, n => 
                n.Translations.Any(t => t.Title.ToLower().Contains(search) || 
                                       t.Content.ToLower().Contains(search)));
        }

        var (items, totalCount) = await _unitOfWork.News.GetPagedAsync(
            filter.Page, 
            filter.PageSize, 
            predicate, 
            BuildOrderBy(filter.SortBy, filter.SortDirection),
            n => n.Translations, n => n.NewsTags, n => n.Images, n => n.Videos, n => n.Files);

        var dtos = _mapper.Map<IReadOnlyList<NewsDto>>(items);
        await SetTranslationsAsync(dtos, filter.LanguageCode);
        
        return (dtos, totalCount);
    }

    public async Task<NewsDto> CreateAsync(CreateNewsDto dto)
    {
        if (await ExistsSlugAsync(dto.Slug))
        {
            throw new InvalidOperationException($"News with slug '{dto.Slug}' already exists.");
        }

        var news = _mapper.Map<News>(dto);
        news.CreatedAt = DateTime.UtcNow;
        
        // Map translations
        foreach (var transDto in dto.Translations)
        {
            var translation = _mapper.Map<NewsTranslation>(transDto);
            translation.News = news;
            news.Translations.Add(translation);
        }

        // Map tags
        foreach (var tagId in dto.TagIds)
        {
            news.NewsTags.Add(new NewsTag { News = news, TagId = tagId });
        }

        // Map images
        foreach (var imageDto in dto.Images)
        {
            var image = _mapper.Map<NewsImage>(imageDto);
            image.News = news;
            news.Images.Add(image);
        }

        // Map videos
        foreach (var videoDto in dto.Videos)
        {
            var video = _mapper.Map<NewsVideo>(videoDto);
            video.News = news;
            news.Videos.Add(video);
        }

        // Map files
        foreach (var fileDto in dto.Files)
        {
            var file = _mapper.Map<NewsFile>(fileDto);
            file.News = news;
            news.Files.Add(file);
        }

        // Map related news
        foreach (var relatedId in dto.RelatedNewsIds)
        {
            news.RelatedNews.Add(new RelatedNews { News = news, RelatedNewsId = relatedId });
        }

        await _unitOfWork.News.AddAsync(news);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<NewsDto>(news);
    }

    public async Task<NewsDto> UpdateAsync(int id, UpdateNewsDto dto)
    {
        var news = await _unitOfWork.News.GetByIdAsync(id,
            n => n.Translations, n => n.NewsTags, n => n.Images, n => n.Videos, n => n.Files, n => n.RelatedNews);
        
        if (news == null)
        {
            throw new KeyNotFoundException($"News with id {id} not found.");
        }

        if (news.Slug != dto.Slug && await ExistsSlugAsync(dto.Slug, id))
        {
            throw new InvalidOperationException($"News with slug '{dto.Slug}' already exists.");
        }

        _mapper.Map(dto, news);
        news.UpdatedAt = DateTime.UtcNow;

        // Update translations
        await UpdateTranslationsAsync(news.Translations, dto.Translations, (t, d) => _mapper.Map(d, t));

        // Update tags
        var newTagIds = dto.TagIds.ToHashSet();
        var existingTagIds = news.NewsTags.Select(nt => nt.TagId).ToHashSet();
        
        foreach (var tagId in existingTagIds.Except(newTagIds))
        {
            var nt = news.NewsTags.First(t => t.TagId == tagId);
            nt.IsDeleted = true;
            nt.DeletedAt = DateTime.UtcNow;
        }
        
        foreach (var tagId in newTagIds.Except(existingTagIds))
        {
            news.NewsTags.Add(new NewsTag { News = news, TagId = tagId });
        }

        // Update images
        await UpdateCollectionAsync(news.Images, dto.Images,
            (i, d) => _mapper.Map(d, i),
            d => _mapper.Map<NewsImage>(d),
            (i, d) => i.Id == d.Id);

        // Update videos
        await UpdateCollectionAsync(news.Videos, dto.Videos,
            (v, d) => _mapper.Map(d, v),
            d => _mapper.Map<NewsVideo>(d),
            (v, d) => v.Id == d.Id);

        // Update files
        await UpdateCollectionAsync(news.Files, dto.Files,
            (f, d) => _mapper.Map(d, f),
            d => _mapper.Map<NewsFile>(d),
            (f, d) => f.Id == d.Id);

        // Update related news
        var newRelatedIds = dto.RelatedNewsIds.ToHashSet();
        var existingRelatedIds = news.RelatedNews.Select(rn => rn.RelatedNewsId).ToHashSet();
        
        foreach (var relatedId in existingRelatedIds.Except(newRelatedIds))
        {
            var rn = news.RelatedNews.First(r => r.RelatedNewsId == relatedId);
            rn.IsDeleted = true;
            rn.DeletedAt = DateTime.UtcNow;
        }
        
        foreach (var relatedId in newRelatedIds.Except(existingRelatedIds))
        {
            news.RelatedNews.Add(new RelatedNews { News = news, RelatedNewsId = relatedId });
        }

        await _unitOfWork.News.UpdateAsync(news);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<NewsDto>(news);
    }

    public async Task DeleteAsync(int id)
    {
        var news = await _unitOfWork.News.GetByIdAsync(id);
        if (news == null)
        {
            throw new KeyNotFoundException($"News with id {id} not found.");
        }

        await _unitOfWork.News.DeleteAsync(news);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task PublishAsync(int id)
    {
        var news = await _unitOfWork.News.GetByIdAsync(id);
        if (news == null)
        {
            throw new KeyNotFoundException($"News with id {id} not found.");
        }

        news.IsPublished = true;
        news.PublishedAt = DateTime.UtcNow;
        news.UpdatedAt = DateTime.UtcNow;
        
        await _unitOfWork.News.UpdateAsync(news);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UnpublishAsync(int id)
    {
        var news = await _unitOfWork.News.GetByIdAsync(id);
        if (news == null)
        {
            throw new KeyNotFoundException($"News with id {id} not found.");
        }

        news.IsPublished = false;
        news.UpdatedAt = DateTime.UtcNow;
        
        await _unitOfWork.News.UpdateAsync(news);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ScheduleAsync(int id, DateTime scheduledAt)
    {
        var news = await _unitOfWork.News.GetByIdAsync(id);
        if (news == null)
        {
            throw new KeyNotFoundException($"News with id {id} not found.");
        }

        news.IsPublished = false;
        news.ScheduledAt = scheduledAt;
        news.UpdatedAt = DateTime.UtcNow;
        
        await _unitOfWork.News.UpdateAsync(news);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> ExistsSlugAsync(string slug, int? excludeId = null, int? languageId = null)
    {
        if (languageId.HasValue)
        {
            return await _unitOfWork.NewsTranslations.ExistsAsync(
                t => t.Slug == slug && t.LanguageId == languageId.Value && (!excludeId.HasValue || t.NewsId != excludeId.Value));
        }
        
        return await _unitOfWork.NewsTranslations.ExistsAsync(
            t => t.Slug == slug && (!excludeId.HasValue || t.NewsId != excludeId.Value));
    }

    public async Task IncrementViewCountAsync(int id)
    {
        var news = await _unitOfWork.News.GetByIdAsync(id);
        if (news != null)
        {
            news.ViewCount++;
            news.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.News.UpdateAsync(news);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IReadOnlyList<NewsDto>> GetFeaturedAsync(string languageCode, int count = 5)
    {
        var news = await _unitOfWork.News.GetAllAsync(
            n => n.IsPublished && n.IsFeatured, 
            q => q.OrderByDescending(n => n.PublishedAt).Take(count),
            n => n.Translations, n => n.NewsTags, n => n.Images, n => n.Videos, n => n.Files);
        
        var dtos = _mapper.Map<IReadOnlyList<NewsDto>>(news);
        await SetTranslationsAsync(dtos, languageCode);
        
        return dtos;
    }

    public async Task<IReadOnlyList<NewsDto>> GetBreakingAsync(string languageCode, int count = 3)
    {
        var news = await _unitOfWork.News.GetAllAsync(
            n => n.IsPublished && n.IsBreaking, 
            q => q.OrderByDescending(n => n.PublishedAt).Take(count),
            n => n.Translations, n => n.NewsTags, n => n.Images, n => n.Videos, n => n.Files);
        
        var dtos = _mapper.Map<IReadOnlyList<NewsDto>>(news);
        await SetTranslationsAsync(dtos, languageCode);
        
        return dtos;
    }

    public async Task<IReadOnlyList<NewsDto>> GetLatestAsync(string languageCode, int count = 10, int? excludeId = null)
    {
        var predicate = BuildPredicate(languageCode, true, null);
        
        if (excludeId.HasValue)
        {
            predicate = CombinePredicates(predicate, n => n.Id != excludeId.Value);
        }

        var news = await _unitOfWork.News.GetAllAsync(
            predicate, 
            q => q.OrderByDescending(n => n.PublishedAt).Take(count),
            n => n.Translations, n => n.NewsTags, n => n.Images, n => n.Videos, n => n.Files);
        
        var dtos = _mapper.Map<IReadOnlyList<NewsDto>>(news);
        await SetTranslationsAsync(dtos, languageCode);
        
        return dtos;
    }

    public async Task<IReadOnlyList<NewsDto>> GetRelatedAsync(int newsId, string languageCode, int count = 4)
    {
        var news = await _unitOfWork.News.GetByIdAsync(newsId);
        if (news == null) return new List<NewsDto>();

        // Get related by category and tags
        var tagIds = news.NewsTags.Select(nt => nt.TagId).ToList();
        
        var relatedNews = await _unitOfWork.News.GetAllAsync(
            n => n.IsPublished && n.Id != newsId && 
                 (n.CategoryId == news.CategoryId || n.NewsTags.Any(nt => tagIds.Contains(nt.TagId))), 
            q => q.OrderByDescending(n => n.PublishedAt).Take(count),
            n => n.Translations, n => n.NewsTags, n => n.Images, n => n.Videos, n => n.Files);
        
        var dtos = _mapper.Map<IReadOnlyList<NewsDto>>(relatedNews);
        await SetTranslationsAsync(dtos, languageCode);
        
        return dtos;
    }

    public async Task<IReadOnlyList<NewsDto>> GetByTagAsync(int tagId, string languageCode, int page = 1, int pageSize = 12)
    {
        var newsIds = await _unitOfWork.NewsTags.Query()
            .Where(nt => nt.TagId == tagId)
            .Select(nt => nt.NewsId)
            .ToListAsync();

        var news = await _unitOfWork.News.GetAllAsync(
            n => n.IsPublished && newsIds.Contains(n.Id),
            q => q.OrderByDescending(n => n.PublishedAt).Skip((page - 1) * pageSize).Take(pageSize),
            n => n.Translations, n => n.NewsTags, n => n.Images, n => n.Videos, n => n.Files);
        
        var dtos = _mapper.Map<IReadOnlyList<NewsDto>>(news);
        await SetTranslationsAsync(dtos, languageCode);
        
        return dtos;
    }

    public async Task<IReadOnlyList<NewsDto>> GetByAuthorAsync(int authorId, string languageCode, int page = 1, int pageSize = 12)
    {
        var news = await _unitOfWork.News.GetAllAsync(
            n => n.IsPublished && n.AuthorId == authorId,
            q => q.OrderByDescending(n => n.PublishedAt).Skip((page - 1) * pageSize).Take(pageSize),
            n => n.Translations, n => n.NewsTags, n => n.Images, n => n.Videos, n => n.Files);
        
        var dtos = _mapper.Map<IReadOnlyList<NewsDto>>(news);
        await SetTranslationsAsync(dtos, languageCode);
        
        return dtos;
    }

    public async Task ProcessScheduledNewsAsync()
    {
        var now = DateTime.UtcNow;
        var scheduledNews = await _unitOfWork.News.GetAllAsync(
            n => !n.IsPublished && n.ScheduledAt.HasValue && n.ScheduledAt.Value <= now);
        
        foreach (var news in scheduledNews)
        {
            news.IsPublished = true;
            news.PublishedAt = now;
            news.ScheduledAt = null;
            news.UpdatedAt = now;
            await _unitOfWork.News.UpdateAsync(news);
        }
        
        if (scheduledNews.Any())
        {
            await _unitOfWork.SaveChangesAsync();
        }
    }

    private async Task SetTranslationAsync(NewsDto dto, string? languageCode)
    {
        if (!string.IsNullOrEmpty(languageCode))
        {
            var language = await _languageService.GetByCodeAsync(languageCode);
            if (language != null)
            {
                dto.Translation = dto.Translations.FirstOrDefault(t => t.LanguageId == language.Id);
            }
        }
    }

    private async Task SetTranslationsAsync(IReadOnlyList<NewsDto> dtos, string? languageCode)
    {
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
    }

    private Expression<Func<News, bool>> BuildPredicate(string? languageCode, bool? isPublished, int? categoryId)
    {
        Expression<Func<News, bool>> predicate = n => true;

        if (isPublished.HasValue)
        {
            predicate = CombinePredicates(predicate, n => n.IsPublished == isPublished.Value);
        }

        if (categoryId.HasValue)
        {
            predicate = CombinePredicates(predicate, n => n.CategoryId == categoryId.Value);
        }

        return predicate;
    }

    private Expression<Func<News, bool>> CombinePredicates(Expression<Func<News, bool>> first, Expression<Func<News, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(News));
        var firstBody = Expression.Invoke(first, parameter);
        var secondBody = Expression.Invoke(second, parameter);
        var combined = Expression.AndAlso(firstBody, secondBody);
        return Expression.Lambda<Func<News, bool>>(combined, parameter);
    }

    private Func<IQueryable<News>, IOrderedQueryable<News>> BuildOrderBy(string? sortBy, string? sortDirection)
    {
        var isDesc = sortDirection?.ToLower() == "desc";
        
        return sortBy?.ToLower() switch
        {
            "title" => isDesc ? q => q.OrderByDescending(n => n.Translations.FirstOrDefault()!.Title) : q => q.OrderBy(n => n.Translations.FirstOrDefault()!.Title),
            "createdat" => isDesc ? q => q.OrderByDescending(n => n.CreatedAt) : q => q.OrderBy(n => n.CreatedAt),
            "updatedat" => isDesc ? q => q.OrderByDescending(n => n.UpdatedAt) : q => q.OrderBy(n => n.UpdatedAt),
            "publishedat" => isDesc ? q => q.OrderByDescending(n => n.PublishedAt) : q => q.OrderBy(n => n.PublishedAt),
            "viewcount" => isDesc ? q => q.OrderByDescending(n => n.ViewCount) : q => q.OrderBy(n => n.ViewCount),
            _ => isDesc ? q => q.OrderByDescending(n => n.PublishedAt) : q => q.OrderByDescending(n => n.PublishedAt)
        };
    }

    private async Task UpdateTranslationsAsync<TEntity, TDto>(
        ICollection<TEntity> existing, 
        List<TDto> dtos,
        Action<TEntity, TDto> updateAction)
        where TEntity : BaseEntity
    {
        var existingDict = existing.ToDictionary(e => e.Id);
        var processedIds = new HashSet<int>();

        foreach (var dto in dtos)
        {
            var idProp = typeof(TDto).GetProperty("Id");
            var id = idProp?.GetValue(dto) as int?;
            
            if (id.HasValue && existingDict.TryGetValue(id.Value, out var entity))
            {
                updateAction(entity, dto);
                entity.UpdatedAt = DateTime.UtcNow;
                processedIds.Add(id.Value);
            }
        }

        foreach (var entity in existing.Where(e => !processedIds.Contains(e.Id)))
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }
    }

    private async Task UpdateCollectionAsync<TEntity, TDto>(
        ICollection<TEntity> existing,
        List<TDto> dtos,
        Action<TEntity, TDto> updateAction,
        Func<TDto, TEntity> createAction,
        Func<TEntity, TDto, bool> matchAction)
        where TEntity : BaseEntity
    {
        var existingList = existing.ToList();
        var processedExisting = new HashSet<int>();

        foreach (var dto in dtos)
        {
            var idProp = typeof(TDto).GetProperty("Id");
            var id = idProp?.GetValue(dto) as int?;
            
            if (id.HasValue)
            {
                var entity = existingList.FirstOrDefault(e => matchAction(e, dto));
                if (entity != null)
                {
                    updateAction(entity, dto);
                    entity.UpdatedAt = DateTime.UtcNow;
                    processedExisting.Add(entity.Id);
                }
            }
            else
            {
                var newEntity = createAction(dto);
                existing.Add(newEntity);
            }
        }

        foreach (var entity in existingList.Where(e => !processedExisting.Contains(e.Id)))
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }
    }
}