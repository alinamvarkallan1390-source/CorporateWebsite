using CorporateWebsite.Core.DTOs;
using System.Linq.Expressions;
using CorporateWebsite.Core.Entities;
using CorporateWebsite.Core.Interfaces;
using CorporateWebsite.Application.Interfaces;
using AutoMapper;

namespace CorporateWebsite.Application.Services;

public class ServiceService : IServiceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILanguageService _languageService;

    public ServiceService(IUnitOfWork unitOfWork, IMapper mapper, ILanguageService languageService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _languageService = languageService;
    }

    public async Task<ServiceDto?> GetByIdAsync(int id, string? languageCode = null)
    {
        var service = await _unitOfWork.Services.GetByIdAsync(id, 
            s => s.Translations, 
            s => s.Features, 
            s => s.Faqs, 
            s => s.Images);
        
        if (service == null) return null;

        var dto = _mapper.Map<ServiceDto>(service);
        await SetTranslationAsync(dto, languageCode);
        
        return dto;
    }

    public async Task<ServiceDto?> GetBySlugAsync(string slug, string languageCode)
    {
        var language = await _languageService.GetByCodeAsync(languageCode);
        if (language == null) return null;

        var translation = await _unitOfWork.ServiceTranslations.FirstOrDefaultAsync(
            t => t.Slug == slug && t.LanguageId == language.Id,
            t => t.Service);
        
        if (translation?.Service == null) return null;

        var service = translation.Service;
        var dto = _mapper.Map<ServiceDto>(service);
        dto.Translation = _mapper.Map<ServiceTranslationDto>(translation);
        
        return dto;
    }

    public async Task<IReadOnlyList<ServiceDto>> GetAllAsync(string? languageCode = null, bool? isPublished = null, int? categoryId = null)
    {
        var predicate = BuildPredicate(languageCode, isPublished, categoryId);
        var services = await _unitOfWork.Services.GetAllAsync(predicate, q => q.OrderBy(s => s.DisplayOrder), 
            s => s.Translations, s => s.Features, s => s.Faqs, s => s.Images);
        
        var dtos = _mapper.Map<IReadOnlyList<ServiceDto>>(services);
        await SetTranslationsAsync(dtos, languageCode);
        
        return dtos;
    }

    public async Task<(IReadOnlyList<ServiceDto> Items, int TotalCount)> GetPagedAsync(ServiceFilterDto filter)
    {
        var predicate = BuildPredicate(filter.LanguageCode, filter.IsPublished, filter.CategoryId);
        
        if (filter.IsFeatured.HasValue)
        {
            predicate = CombinePredicates(predicate, s => s.IsFeatured == filter.IsFeatured.Value);
        }

        if (!string.IsNullOrEmpty(filter.Search))
        {
            var search = filter.Search.ToLower();
            predicate = CombinePredicates(predicate, s => 
                s.Translations.Any(t => t.Title.ToLower().Contains(search) || 
                                       t.Content.ToLower().Contains(search)));
        }

        var (items, totalCount) = await _unitOfWork.Services.GetPagedAsync(
            filter.Page, 
            filter.PageSize, 
            predicate, 
            BuildOrderBy(filter.SortBy, filter.SortDirection),
            s => s.Translations, s => s.Features, s => s.Faqs, s => s.Images);

        var dtos = _mapper.Map<IReadOnlyList<ServiceDto>>(items);
        await SetTranslationsAsync(dtos, filter.LanguageCode);
        
        return (dtos, totalCount);
    }

    public async Task<ServiceDto> CreateAsync(CreateServiceDto dto)
    {
        if (await ExistsSlugAsync(dto.Slug))
        {
            throw new InvalidOperationException($"Service with slug '{dto.Slug}' already exists.");
        }

        var service = _mapper.Map<Service>(dto);
        service.CreatedAt = DateTime.UtcNow;
        
        // Map translations
        foreach (var transDto in dto.Translations)
        {
            var translation = _mapper.Map<ServiceTranslation>(transDto);
            translation.Service = service;
            service.Translations.Add(translation);
        }

        // Map features
        foreach (var featureDto in dto.Features)
        {
            var feature = _mapper.Map<ServiceFeature>(featureDto);
            feature.Service = service;
            foreach (var transDto in featureDto.Translations)
            {
                var translation = _mapper.Map<ServiceFeatureTranslation>(transDto);
                translation.Feature = feature;
                feature.Translations.Add(translation);
            }
            service.Features.Add(feature);
        }

        // Map FAQs
        foreach (var faqDto in dto.Faqs)
        {
            var faq = _mapper.Map<ServiceFaq>(faqDto);
            faq.Service = service;
            foreach (var transDto in faqDto.Translations)
            {
                var translation = _mapper.Map<ServiceFaqTranslation>(transDto);
                translation.Faq = faq;
                faq.Translations.Add(translation);
            }
            service.Faqs.Add(faq);
        }

        // Map images
        foreach (var imageDto in dto.Images)
        {
            var image = _mapper.Map<ServiceImage>(imageDto);
            image.Service = service;
            service.Images.Add(image);
        }

        await _unitOfWork.Services.AddAsync(service);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<ServiceDto>(service);
    }

    public async Task<ServiceDto> UpdateAsync(int id, UpdateServiceDto dto)
    {
        var service = await _unitOfWork.Services.GetByIdAsync(id, 
            s => s.Translations, s => s.Features, s => s.Faqs, s => s.Images);
        
        if (service == null)
        {
            throw new KeyNotFoundException($"Service with id {id} not found.");
        }

        if (service.Slug != dto.Slug && await ExistsSlugAsync(dto.Slug, id))
        {
            throw new InvalidOperationException($"Service with slug '{dto.Slug}' already exists.");
        }

        _mapper.Map(dto, service);
        service.UpdatedAt = DateTime.UtcNow;

        // Update translations
        await UpdateTranslationsAsync(service.Translations, dto.Translations, (t, d) => _mapper.Map(d, t));

        // Update features
        await UpdateCollectionAsync(service.Features, dto.Features, 
            (f, d) => _mapper.Map(d, f),
            d => _mapper.Map<ServiceFeature>(d),
            (f, d) => f.Id == d.Id);

        // Update FAQs
        await UpdateCollectionAsync(service.Faqs, dto.Faqs,
            (f, d) => _mapper.Map(d, f),
            d => _mapper.Map<ServiceFaq>(d),
            (f, d) => f.Id == d.Id);

        // Update images
        await UpdateCollectionAsync(service.Images, dto.Images,
            (i, d) => _mapper.Map(d, i),
            d => _mapper.Map<ServiceImage>(d),
            (i, d) => i.Id == d.Id);

        await _unitOfWork.Services.UpdateAsync(service);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<ServiceDto>(service);
    }

    public async Task DeleteAsync(int id)
    {
        var service = await _unitOfWork.Services.GetByIdAsync(id);
        if (service == null)
        {
            throw new KeyNotFoundException($"Service with id {id} not found.");
        }

        await _unitOfWork.Services.DeleteAsync(service);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task PublishAsync(int id)
    {
        var service = await _unitOfWork.Services.GetByIdAsync(id);
        if (service == null)
        {
            throw new KeyNotFoundException($"Service with id {id} not found.");
        }

        service.IsPublished = true;
        service.PublishedAt = DateTime.UtcNow;
        service.UpdatedAt = DateTime.UtcNow;
        
        await _unitOfWork.Services.UpdateAsync(service);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UnpublishAsync(int id)
    {
        var service = await _unitOfWork.Services.GetByIdAsync(id);
        if (service == null)
        {
            throw new KeyNotFoundException($"Service with id {id} not found.");
        }

        service.IsPublished = false;
        service.UpdatedAt = DateTime.UtcNow;
        
        await _unitOfWork.Services.UpdateAsync(service);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ReorderAsync(List<ReorderItemDto> items)
    {
        foreach (var item in items)
        {
            var service = await _unitOfWork.Services.GetByIdAsync(item.Id);
            if (service != null)
            {
                service.DisplayOrder = item.DisplayOrder;
                service.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Services.UpdateAsync(service);
            }
        }
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> ExistsSlugAsync(string slug, int? excludeId = null, int? languageId = null)
    {
        if (languageId.HasValue)
        {
            return await _unitOfWork.ServiceTranslations.ExistsAsync(
                t => t.Slug == slug && t.LanguageId == languageId.Value && (!excludeId.HasValue || t.ServiceId != excludeId.Value));
        }
        
        return await _unitOfWork.ServiceTranslations.ExistsAsync(
            t => t.Slug == slug && (!excludeId.HasValue || t.ServiceId != excludeId.Value));
    }

    public async Task IncrementViewCountAsync(int id)
    {
        var service = await _unitOfWork.Services.GetByIdAsync(id);
        if (service != null)
        {
            service.ViewCount++;
            service.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Services.UpdateAsync(service);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IReadOnlyList<ServiceDto>> GetFeaturedAsync(string languageCode, int count = 6)
    {
        var services = await _unitOfWork.Services.GetAllAsync(
            s => s.IsPublished && s.IsFeatured, 
            q => q.OrderBy(s => s.DisplayOrder).Take(count),
            s => s.Translations, s => s.Features, s => s.Faqs, s => s.Images);
        
        var dtos = _mapper.Map<IReadOnlyList<ServiceDto>>(services);
        await SetTranslationsAsync(dtos, languageCode);
        
        return dtos;
    }

    public async Task<IReadOnlyList<ServiceDto>> GetRelatedAsync(int serviceId, string languageCode, int count = 4)
    {
        var service = await _unitOfWork.Services.GetByIdAsync(serviceId);
        if (service == null) return new List<ServiceDto>();

        var services = await _unitOfWork.Services.GetAllAsync(
            s => s.IsPublished && s.Id != serviceId && s.CategoryId == service.CategoryId, 
            q => q.OrderBy(s => s.DisplayOrder).Take(count),
            s => s.Translations, s => s.Features, s => s.Faqs, s => s.Images);
        
        var dtos = _mapper.Map<IReadOnlyList<ServiceDto>>(services);
        await SetTranslationsAsync(dtos, languageCode);
        
        return dtos;
    }

    private async Task SetTranslationAsync(ServiceDto dto, string? languageCode)
    {
        if (!string.IsNullOrEmpty(languageCode))
        {
            var language = await _languageService.GetByCodeAsync(languageCode);
            if (language != null)
            {
                dto.Translation = dto.Translations.FirstOrDefault(t => t.LanguageId == language.Id);
                
                foreach (var feature in dto.Features)
                {
                    feature.Translation = feature.Translations.FirstOrDefault(t => t.LanguageId == language.Id);
                }
                
                foreach (var faq in dto.Faqs)
                {
                    faq.Translation = faq.Translations.FirstOrDefault(t => t.LanguageId == language.Id);
                }
            }
        }
    }

    private async Task SetTranslationsAsync(IReadOnlyList<ServiceDto> dtos, string? languageCode)
    {
        if (!string.IsNullOrEmpty(languageCode))
        {
            var language = await _languageService.GetByCodeAsync(languageCode);
            if (language != null)
            {
                foreach (var dto in dtos)
                {
                    dto.Translation = dto.Translations.FirstOrDefault(t => t.LanguageId == language.Id);
                    
                    foreach (var feature in dto.Features)
                    {
                        feature.Translation = feature.Translations.FirstOrDefault(t => t.LanguageId == language.Id);
                    }
                    
                    foreach (var faq in dto.Faqs)
                    {
                        faq.Translation = faq.Translations.FirstOrDefault(t => t.LanguageId == language.Id);
                    }
                }
            }
        }
    }

    private Expression<Func<Service, bool>> BuildPredicate(string? languageCode, bool? isPublished, int? categoryId)
    {
        Expression<Func<Service, bool>> predicate = s => true;

        if (isPublished.HasValue)
        {
            predicate = CombinePredicates(predicate, s => s.IsPublished == isPublished.Value);
        }

        if (categoryId.HasValue)
        {
            predicate = CombinePredicates(predicate, s => s.CategoryId == categoryId.Value);
        }

        return predicate;
    }

    private Expression<Func<Service, bool>> CombinePredicates(Expression<Func<Service, bool>> first, Expression<Func<Service, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(Service));
        var firstBody = Expression.Invoke(first, parameter);
        var secondBody = Expression.Invoke(second, parameter);
        var combined = Expression.AndAlso(firstBody, secondBody);
        return Expression.Lambda<Func<Service, bool>>(combined, parameter);
    }

    private Func<IQueryable<Service>, IOrderedQueryable<Service>> BuildOrderBy(string? sortBy, string? sortDirection)
    {
        var isDesc = sortDirection?.ToLower() == "desc";
        
        return sortBy?.ToLower() switch
        {
            "title" => isDesc ? q => q.OrderByDescending(s => s.Translations.FirstOrDefault()!.Title) : q => q.OrderBy(s => s.Translations.FirstOrDefault()!.Title),
            "createdat" => isDesc ? q => q.OrderByDescending(s => s.CreatedAt) : q => q.OrderBy(s => s.CreatedAt),
            "updatedat" => isDesc ? q => q.OrderByDescending(s => s.UpdatedAt) : q => q.OrderBy(s => s.UpdatedAt),
            "publishedat" => isDesc ? q => q.OrderByDescending(s => s.PublishedAt) : q => q.OrderBy(s => s.PublishedAt),
            "viewcount" => isDesc ? q => q.OrderByDescending(s => s.ViewCount) : q => q.OrderBy(s => s.ViewCount),
            _ => isDesc ? q => q.OrderByDescending(s => s.DisplayOrder) : q => q.OrderBy(s => s.DisplayOrder)
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
            else if (!id.HasValue)
            {
                // This would require creating new entities, but we don't have the factory here
                // This is handled in the specific update methods
            }
        }

        // Mark deleted
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

        // Mark deleted
        foreach (var entity in existingList.Where(e => !processedExisting.Contains(e.Id)))
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }
    }
}