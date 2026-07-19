using CorporateWebsite.Core.DTOs;
using System.Linq.Expressions;
using CorporateWebsite.Core.Entities;
using CorporateWebsite.Core.Interfaces;
using CorporateWebsite.Application.Interfaces;
using AutoMapper;

namespace CorporateWebsite.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILanguageService _languageService;

    public ProjectService(IUnitOfWork unitOfWork, IMapper mapper, ILanguageService languageService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _languageService = languageService;
    }

    public async Task<ProjectDto?> GetByIdAsync(int id, string? languageCode = null)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(id,
            p => p.Translations,
            p => p.Images,
            p => p.Videos,
            p => p.Files,
            p => p.ProjectTags,
            p => p.Features,
            p => p.TeamMembers);
        
        if (project == null) return null;

        var dto = _mapper.Map<ProjectDto>(project);
        await SetTranslationAsync(dto, languageCode);
        
        return dto;
    }

    public async Task<ProjectDto?> GetBySlugAsync(string slug, string languageCode)
    {
        var language = await _languageService.GetByCodeAsync(languageCode);
        if (language == null) return null;

        var translation = await _unitOfWork.ProjectTranslations.FirstOrDefaultAsync(
            t => t.Slug == slug && t.LanguageId == language.Id,
            t => t.Project);
        
        if (translation?.Project == null) return null;

        var project = translation.Project;
        var dto = _mapper.Map<ProjectDto>(project);
        dto.Translation = _mapper.Map<ProjectTranslationDto>(translation);
        
        return dto;
    }

    public async Task<IReadOnlyList<ProjectDto>> GetAllAsync(string? languageCode = null, bool? isPublished = null, int? categoryId = null)
    {
        var predicate = BuildPredicate(languageCode, isPublished, categoryId);
        var projects = await _unitOfWork.Projects.GetAllAsync(predicate, q => q.OrderBy(p => p.DisplayOrder),
            p => p.Translations, p => p.Images, p => p.Videos, p => p.Files,
            p => p.ProjectTags, p => p.Features, p => p.TeamMembers);
        
        var dtos = _mapper.Map<IReadOnlyList<ProjectDto>>(projects);
        await SetTranslationsAsync(dtos, languageCode);
        
        return dtos;
    }

    public async Task<(IReadOnlyList<ProjectDto> Items, int TotalCount)> GetPagedAsync(ProjectFilterDto filter)
    {
        var predicate = BuildPredicate(filter.LanguageCode, filter.IsPublished, filter.CategoryId);
        
        if (filter.IsFeatured.HasValue)
        {
            predicate = CombinePredicates(predicate, p => p.IsFeatured == filter.IsFeatured.Value);
        }

        if (!string.IsNullOrEmpty(filter.Search))
        {
            var search = filter.Search.ToLower();
            predicate = CombinePredicates(predicate, p => 
                p.Translations.Any(t => t.Title.ToLower().Contains(search) || 
                                       t.Content.ToLower().Contains(search)));
        }

        var (items, totalCount) = await _unitOfWork.Projects.GetPagedAsync(
            filter.Page, 
            filter.PageSize, 
            predicate, 
            BuildOrderBy(filter.SortBy, filter.SortDirection),
            p => p.Translations, p => p.Images, p => p.Videos, p => p.Files,
            p => p.ProjectTags, p => p.Features, p => p.TeamMembers);

        var dtos = _mapper.Map<IReadOnlyList<ProjectDto>>(items);
        await SetTranslationsAsync(dtos, filter.LanguageCode);
        
        return (dtos, totalCount);
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto)
    {
        if (await ExistsSlugAsync(dto.Slug))
        {
            throw new InvalidOperationException($"Project with slug '{dto.Slug}' already exists.");
        }

        var project = _mapper.Map<Project>(dto);
        project.CreatedAt = DateTime.UtcNow;
        
        // Map translations
        foreach (var transDto in dto.Translations)
        {
            var translation = _mapper.Map<ProjectTranslation>(transDto);
            translation.Project = project;
            project.Translations.Add(translation);
        }

        // Map images
        foreach (var imageDto in dto.Images)
        {
            var image = _mapper.Map<ProjectImage>(imageDto);
            image.Project = project;
            project.Images.Add(image);
        }

        // Map videos
        foreach (var videoDto in dto.Videos)
        {
            var video = _mapper.Map<ProjectVideo>(videoDto);
            video.Project = project;
            project.Videos.Add(video);
        }

        // Map files
        foreach (var fileDto in dto.Files)
        {
            var file = _mapper.Map<ProjectFile>(fileDto);
            file.Project = project;
            project.Files.Add(file);
        }

        // Map tags
        foreach (var tagId in dto.TagIds)
        {
            project.ProjectTags.Add(new ProjectTag { Project = project, TagId = tagId });
        }

        // Map features
        foreach (var featureDto in dto.Features)
        {
            var feature = _mapper.Map<ProjectFeature>(featureDto);
            feature.Project = project;
            foreach (var transDto in featureDto.Translations)
            {
                var translation = _mapper.Map<ProjectFeatureTranslation>(transDto);
                translation.Feature = feature;
                feature.Translations.Add(translation);
            }
            project.Features.Add(feature);
        }

        // Map team members
        foreach (var memberDto in dto.TeamMembers)
        {
            var member = _mapper.Map<ProjectTeamMember>(memberDto);
            member.Project = project;
            foreach (var transDto in memberDto.Translations)
            {
                var translation = _mapper.Map<ProjectTeamMemberTranslation>(transDto);
                translation.TeamMember = member;
                member.Translations.Add(translation);
            }
            project.TeamMembers.Add(member);
        }

        await _unitOfWork.Projects.AddAsync(project);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<ProjectDto>(project);
    }

    public async Task<ProjectDto> UpdateAsync(int id, UpdateProjectDto dto)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(id,
            p => p.Translations, p => p.Images, p => p.Videos, p => p.Files,
            p => p.ProjectTags, p => p.Features, p => p.TeamMembers);
        
        if (project == null)
        {
            throw new KeyNotFoundException($"Project with id {id} not found.");
        }

        if (project.Slug != dto.Slug && await ExistsSlugAsync(dto.Slug, id))
        {
            throw new InvalidOperationException($"Project with slug '{dto.Slug}' already exists.");
        }

        _mapper.Map(dto, project);
        project.UpdatedAt = DateTime.UtcNow;

        // Update translations
        await UpdateTranslationsAsync(project.Translations, dto.Translations, (t, d) => _mapper.Map(d, t));

        // Update images
        await UpdateCollectionAsync(project.Images, dto.Images,
            (i, d) => _mapper.Map(d, i),
            d => _mapper.Map<ProjectImage>(d),
            (i, d) => i.Id == d.Id);

        // Update videos
        await UpdateCollectionAsync(project.Videos, dto.Videos,
            (v, d) => _mapper.Map(d, v),
            d => _mapper.Map<ProjectVideo>(d),
            (v, d) => v.Id == d.Id);

        // Update files
        await UpdateCollectionAsync(project.Files, dto.Files,
            (f, d) => _mapper.Map(d, f),
            d => _mapper.Map<ProjectFile>(d),
            (f, d) => f.Id == d.Id);

        // Update tags
        var newTagIds = dto.TagIds.ToHashSet();
        var existingTagIds = project.ProjectTags.Select(pt => pt.TagId).ToHashSet();
        
        // Remove old tags
        foreach (var tagId in existingTagIds.Except(newTagIds))
        {
            var pt = project.ProjectTags.First(t => t.TagId == tagId);
            pt.IsDeleted = true;
            pt.DeletedAt = DateTime.UtcNow;
        }
        
        // Add new tags
        foreach (var tagId in newTagIds.Except(existingTagIds))
        {
            project.ProjectTags.Add(new ProjectTag { Project = project, TagId = tagId });
        }

        // Update features
        await UpdateCollectionAsync(project.Features, dto.Features,
            (f, d) => _mapper.Map(d, f),
            d => _mapper.Map<ProjectFeature>(d),
            (f, d) => f.Id == d.Id);

        // Update team members
        await UpdateCollectionAsync(project.TeamMembers, dto.TeamMembers,
            (m, d) => _mapper.Map(d, m),
            d => _mapper.Map<ProjectTeamMember>(d),
            (m, d) => m.Id == d.Id);

        await _unitOfWork.Projects.UpdateAsync(project);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<ProjectDto>(project);
    }

    public async Task DeleteAsync(int id)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(id);
        if (project == null)
        {
            throw new KeyNotFoundException($"Project with id {id} not found.");
        }

        await _unitOfWork.Projects.DeleteAsync(project);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task PublishAsync(int id)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(id);
        if (project == null)
        {
            throw new KeyNotFoundException($"Project with id {id} not found.");
        }

        project.IsPublished = true;
        project.PublishedAt = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;
        
        await _unitOfWork.Projects.UpdateAsync(project);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UnpublishAsync(int id)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(id);
        if (project == null)
        {
            throw new KeyNotFoundException($"Project with id {id} not found.");
        }

        project.IsPublished = false;
        project.UpdatedAt = DateTime.UtcNow;
        
        await _unitOfWork.Projects.UpdateAsync(project);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ReorderAsync(List<ReorderItemDto> items)
    {
        foreach (var item in items)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(item.Id);
            if (project != null)
            {
                project.DisplayOrder = item.DisplayOrder;
                project.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Projects.UpdateAsync(project);
            }
        }
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> ExistsSlugAsync(string slug, int? excludeId = null, int? languageId = null)
    {
        if (languageId.HasValue)
        {
            return await _unitOfWork.ProjectTranslations.ExistsAsync(
                t => t.Slug == slug && t.LanguageId == languageId.Value && (!excludeId.HasValue || t.ProjectId != excludeId.Value));
        }
        
        return await _unitOfWork.ProjectTranslations.ExistsAsync(
            t => t.Slug == slug && (!excludeId.HasValue || t.ProjectId != excludeId.Value));
    }

    public async Task IncrementViewCountAsync(int id)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(id);
        if (project != null)
        {
            project.ViewCount++;
            project.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Projects.UpdateAsync(project);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IReadOnlyList<ProjectDto>> GetFeaturedAsync(string languageCode, int count = 6)
    {
        var projects = await _unitOfWork.Projects.GetAllAsync(
            p => p.IsPublished && p.IsFeatured, 
            q => q.OrderBy(p => p.DisplayOrder).Take(count),
            p => p.Translations, p => p.Images, p => p.Videos, p => p.Files,
            p => p.ProjectTags, p => p.Features, p => p.TeamMembers);
        
        var dtos = _mapper.Map<IReadOnlyList<ProjectDto>>(projects);
        await SetTranslationsAsync(dtos, languageCode);
        
        return dtos;
    }

    public async Task<IReadOnlyList<ProjectDto>> GetRelatedAsync(int projectId, string languageCode, int count = 4)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId);
        if (project == null) return new List<ProjectDto>();

        var projects = await _unitOfWork.Projects.GetAllAsync(
            p => p.IsPublished && p.Id != projectId && p.CategoryId == project.CategoryId, 
            q => q.OrderBy(p => p.DisplayOrder).Take(count),
            p => p.Translations, p => p.Images, p => p.Videos, p => p.Files,
            p => p.ProjectTags, p => p.Features, p => p.TeamMembers);
        
        var dtos = _mapper.Map<IReadOnlyList<ProjectDto>>(projects);
        await SetTranslationsAsync(dtos, languageCode);
        
        return dtos;
    }

    public async Task<IReadOnlyList<ProjectDto>> GetByTagAsync(int tagId, string languageCode, int page = 1, int pageSize = 12)
    {
        var projectIds = await _unitOfWork.ProjectTags.Query()
            .Where(pt => pt.TagId == tagId)
            .Select(pt => pt.ProjectId)
            .ToListAsync();

        var projects = await _unitOfWork.Projects.GetAllAsync(
            p => p.IsPublished && projectIds.Contains(p.Id),
            q => q.OrderBy(p => p.DisplayOrder).Skip((page - 1) * pageSize).Take(pageSize),
            p => p.Translations, p => p.Images, p => p.Videos, p => p.Files,
            p => p.ProjectTags, p => p.Features, p => p.TeamMembers);
        
        var dtos = _mapper.Map<IReadOnlyList<ProjectDto>>(projects);
        await SetTranslationsAsync(dtos, languageCode);
        
        return dtos;
    }

    private async Task SetTranslationAsync(ProjectDto dto, string? languageCode)
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
                
                foreach (var member in dto.TeamMembers)
                {
                    member.Translation = member.Translations.FirstOrDefault(t => t.LanguageId == language.Id);
                }
            }
        }
    }

    private async Task SetTranslationsAsync(IReadOnlyList<ProjectDto> dtos, string? languageCode)
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
                    
                    foreach (var member in dto.TeamMembers)
                    {
                        member.Translation = member.Translations.FirstOrDefault(t => t.LanguageId == language.Id);
                    }
                }
            }
        }
    }

    private Expression<Func<Project, bool>> BuildPredicate(string? languageCode, bool? isPublished, int? categoryId)
    {
        Expression<Func<Project, bool>> predicate = p => true;

        if (isPublished.HasValue)
        {
            predicate = CombinePredicates(predicate, p => p.IsPublished == isPublished.Value);
        }

        if (categoryId.HasValue)
        {
            predicate = CombinePredicates(predicate, p => p.CategoryId == categoryId.Value);
        }

        return predicate;
    }

    private Expression<Func<Project, bool>> CombinePredicates(Expression<Func<Project, bool>> first, Expression<Func<Project, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(Project));
        var firstBody = Expression.Invoke(first, parameter);
        var secondBody = Expression.Invoke(second, parameter);
        var combined = Expression.AndAlso(firstBody, secondBody);
        return Expression.Lambda<Func<Project, bool>>(combined, parameter);
    }

    private Func<IQueryable<Project>, IOrderedQueryable<Project>> BuildOrderBy(string? sortBy, string? sortDirection)
    {
        var isDesc = sortDirection?.ToLower() == "desc";
        
        return sortBy?.ToLower() switch
        {
            "title" => isDesc ? q => q.OrderByDescending(p => p.Translations.FirstOrDefault()!.Title) : q => q.OrderBy(p => p.Translations.FirstOrDefault()!.Title),
            "createdat" => isDesc ? q => q.OrderByDescending(p => p.CreatedAt) : q => q.OrderBy(p => p.CreatedAt),
            "updatedat" => isDesc ? q => q.OrderByDescending(p => p.UpdatedAt) : q => q.OrderBy(p => p.UpdatedAt),
            "publishedat" => isDesc ? q => q.OrderByDescending(p => p.PublishedAt) : q => q.OrderBy(p => p.PublishedAt),
            "viewcount" => isDesc ? q => q.OrderByDescending(p => p.ViewCount) : q => q.OrderBy(p => p.ViewCount),
            _ => isDesc ? q => q.OrderByDescending(p => p.DisplayOrder) : q => q.OrderBy(p => p.DisplayOrder)
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