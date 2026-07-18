using CorporateWebsite.Core.DTOs;
using CorporateWebsite.Core.Entities;
using CorporateWebsite.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CorporateWebsite.Infrastructure.Services;

public class RedirectService : IRedirectService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILanguageService _languageService;

    public RedirectService(IUnitOfWork unitOfWork, IMapper mapper, ILanguageService languageService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _languageService = languageService;
    }

    public async Task<RedirectDto?> GetByIdAsync(int id)
    {
        var redirect = await _unitOfWork.Redirects.GetByIdAsync(id);
        return redirect != null ? _mapper.Map<RedirectDto>(redirect) : null;
    }

    public async Task<RedirectDto?> GetBySourceUrlAsync(string sourceUrl, int? languageId = null)
    {
        var redirect = await _unitOfWork.Redirects.FirstOrDefaultAsync(
            r => r.SourceUrl == sourceUrl && r.IsActive && (!languageId.HasValue || r.LanguageId == languageId.Value || r.LanguageId == null));
        
        return redirect != null ? _mapper.Map<RedirectDto>(redirect) : null;
    }

    public async Task<IReadOnlyList<RedirectDto>> GetAllAsync(bool? isActive = null)
    {
        Expression<Func<Redirect, bool>> predicate = r => true;
        
        if (isActive.HasValue)
        {
            predicate = CombinePredicates(predicate, r => r.IsActive == isActive.Value);
        }

        var redirects = await _unitOfWork.Redirects.GetAllAsync(predicate, q => q.OrderByDescending(r => r.CreatedAt));
        return _mapper.Map<IReadOnlyList<RedirectDto>>(redirects);
    }

    public async Task<(IReadOnlyList<RedirectDto> Items, int TotalCount)> GetPagedAsync(RedirectFilterDto filter)
    {
        Expression<Func<Redirect, bool>> predicate = r => true;

        if (filter.IsActive.HasValue)
        {
            predicate = CombinePredicates(predicate, r => r.IsActive == filter.IsActive.Value);
        }

        if (filter.StatusCode.HasValue)
        {
            predicate = CombinePredicates(predicate, r => r.StatusCode == filter.StatusCode.Value);
        }

        if (!string.IsNullOrEmpty(filter.Search))
        {
            var search = filter.Search.ToLower();
            predicate = CombinePredicates(predicate, r => 
                r.SourceUrl.ToLower().Contains(search) || 
                r.TargetUrl.ToLower().Contains(search));
        }

        var (items, totalCount) = await _unitOfWork.Redirects.GetPagedAsync(
            filter.Page, 
            filter.PageSize, 
            predicate, 
            BuildOrderBy(filter.SortBy, filter.SortDirection));

        var dtos = _mapper.Map<IReadOnlyList<RedirectDto>>(items);
        return (dtos, totalCount);
    }

    public async Task<RedirectDto> CreateAsync(CreateRedirectDto dto)
    {
        var redirect = _mapper.Map<Redirect>(dto);
        redirect.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Redirects.AddAsync(redirect);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<RedirectDto>(redirect);
    }

    public async Task<RedirectDto> UpdateAsync(int id, UpdateRedirectDto dto)
    {
        var redirect = await _unitOfWork.Redirects.GetByIdAsync(id);
        if (redirect == null)
        {
            throw new KeyNotFoundException($"Redirect with id {id} not found.");
        }

        _mapper.Map(dto, redirect);
        redirect.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Redirects.UpdateAsync(redirect);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<RedirectDto>(redirect);
    }

    public async Task DeleteAsync(int id)
    {
        var redirect = await _unitOfWork.Redirects.GetByIdAsync(id);
        if (redirect == null)
        {
            throw new KeyNotFoundException($"Redirect with id {id} not found.");
        }

        await _unitOfWork.Redirects.DeleteAsync(redirect);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ProcessRedirectAsync(string sourceUrl, int? languageId)
    {
        var redirect = await GetBySourceUrlAsync(sourceUrl, languageId);
        if (redirect != null)
        {
            redirect.HitCount++;
            redirect.LastHitAt = DateTime.UtcNow;
            
            var entity = await _unitOfWork.Redirects.GetByIdAsync(redirect.Id);
            if (entity != null)
            {
                entity.HitCount = redirect.HitCount;
                entity.LastHitAt = redirect.LastHitAt;
                entity.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Redirects.UpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }

    public async Task<IReadOnlyList<RedirectDto>> GetBrokenLinksAsync()
    {
        var brokenLinks = await _unitOfWork.BrokenLinks.GetAllAsync(b => !b.IsFixed, q => q.OrderByDescending(b => b.HitCount));
        return _mapper.Map<IReadOnlyList<RedirectDto>>(brokenLinks);
    }

    public async Task FixBrokenLinkAsync(int id, string fixedUrl)
    {
        var brokenLink = await _unitOfWork.BrokenLinks.GetByIdAsync(id);
        if (brokenLink == null)
        {
            throw new KeyNotFoundException($"Broken link with id {id} not found.");
        }

        brokenLink.IsFixed = true;
        brokenLink.FixedUrl = fixedUrl;
        brokenLink.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.BrokenLinks.UpdateAsync(brokenLink);
        await _unitOfWork.SaveChangesAsync();
    }

    private Expression<Func<Redirect, bool>> CombinePredicates(Expression<Func<Redirect, bool>> first, Expression<Func<Redirect, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(Redirect));
        var firstBody = Expression.Invoke(first, parameter);
        var secondBody = Expression.Invoke(second, parameter);
        var combined = Expression.AndAlso(firstBody, secondBody);
        return Expression.Lambda<Func<Redirect, bool>>(combined, parameter);
    }

    private Func<IQueryable<Redirect>, IOrderedQueryable<Redirect>> BuildOrderBy(string? sortBy, string? sortDirection)
    {
        var isDesc = sortDirection?.ToLower() == "desc";
        
        return sortBy?.ToLower() switch
        {
            "sourceurl" => isDesc ? q => q.OrderByDescending(r => r.SourceUrl) : q => q.OrderBy(r => r.SourceUrl),
            "targeturl" => isDesc ? q => q.OrderByDescending(r => r.TargetUrl) : q => q.OrderBy(r => r.TargetUrl),
            "statuscode" => isDesc ? q => q.OrderByDescending(r => r.StatusCode) : q => q.OrderBy(r => r.StatusCode),
            "hitcount" => isDesc ? q => q.OrderByDescending(r => r.HitCount) : q => q.OrderBy(r => r.HitCount),
            "createdat" => isDesc ? q => q.OrderByDescending(r => r.CreatedAt) : q => q.OrderBy(r => r.CreatedAt),
            _ => isDesc ? q => q.OrderByDescending(r => r.CreatedAt) : q => q.OrderByDescending(r => r.CreatedAt)
        };
    }
}