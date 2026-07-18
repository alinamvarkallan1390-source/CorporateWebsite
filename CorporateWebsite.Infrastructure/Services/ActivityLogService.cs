using CorporateWebsite.Core.DTOs;
using CorporateWebsite.Core.Entities;
using CorporateWebsite.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CorporateWebsite.Infrastructure.Services;

public class ActivityLogService : IActivityLogService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ActivityLogService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task LogAsync(string userId, string userName, string action, string module, int? entityId = null, string? entityType = null, string? entityName = null, object? oldValues = null, object? newValues = null, string? ipAddress = null, string? userAgent = null, string? url = null, string? method = null, bool isSuccess = true, string? errorMessage = null, string? correlationId = null)
    {
        var log = new ActivityLog
        {
            UserId = userId,
            UserName = userName,
            Action = action,
            Module = module,
            EntityId = entityId,
            EntityType = entityType,
            EntityName = entityName,
            OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
            NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Url = url,
            Method = method,
            IsSuccess = isSuccess,
            ErrorMessage = errorMessage,
            CorrelationId = correlationId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.ActivityLogs.AddAsync(log);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<(IReadOnlyList<ActivityLogDto> Items, int TotalCount)> GetPagedAsync(ActivityLogFilterDto filter)
    {
        Expression<Func<ActivityLog, bool>> predicate = l => true;

        if (!string.IsNullOrEmpty(filter.UserId))
        {
            predicate = CombinePredicates(predicate, l => l.UserId == filter.UserId);
        }

        if (!string.IsNullOrEmpty(filter.Action))
        {
            predicate = CombinePredicates(predicate, l => l.Action == filter.Action);
        }

        if (!string.IsNullOrEmpty(filter.Module))
        {
            predicate = CombinePredicates(predicate, l => l.Module == filter.Module);
        }

        if (!string.IsNullOrEmpty(filter.EntityType))
        {
            predicate = CombinePredicates(predicate, l => l.EntityType == filter.EntityType);
        }

        if (filter.EntityId.HasValue)
        {
            predicate = CombinePredicates(predicate, l => l.EntityId == filter.EntityId.Value);
        }

        if (filter.IsSuccess.HasValue)
        {
            predicate = CombinePredicates(predicate, l => l.IsSuccess == filter.IsSuccess.Value);
        }

        if (filter.FromDate.HasValue)
        {
            predicate = CombinePredicates(predicate, l => l.CreatedAt >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            predicate = CombinePredicates(predicate, l => l.CreatedAt <= filter.ToDate.Value);
        }

        if (!string.IsNullOrEmpty(filter.IpAddress))
        {
            predicate = CombinePredicates(predicate, l => l.IpAddress == filter.IpAddress);
        }

        var (items, totalCount) = await _unitOfWork.ActivityLogs.GetPagedAsync(
            filter.Page, 
            filter.PageSize, 
            predicate, 
            BuildOrderBy(filter.SortBy, filter.SortDirection));

        var dtos = _mapper.Map<IReadOnlyList<ActivityLogDto>>(items);
        return (dtos, totalCount);
    }

    public async Task<IReadOnlyList<ActivityLogDto>> GetByEntityAsync(string entityType, int entityId)
    {
        var logs = await _unitOfWork.ActivityLogs.GetAllAsync(
            l => l.EntityType == entityType && l.EntityId == entityId,
            q => q.OrderByDescending(l => l.CreatedAt));
        
        return _mapper.Map<IReadOnlyList<ActivityLogDto>>(logs);
    }

    public async Task<IReadOnlyList<ActivityLogDto>> GetByUserAsync(string userId, int count = 50)
    {
        var logs = await _unitOfWork.ActivityLogs.GetAllAsync(
            l => l.UserId == userId,
            q => q.OrderByDescending(l => l.CreatedAt).Take(count));
        
        return _mapper.Map<IReadOnlyList<ActivityLogDto>>(logs);
    }

    public async Task ClearOldLogsAsync(int olderThanDays = 90)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-olderThanDays);
        var oldLogs = await _unitOfWork.ActivityLogs.GetAllAsync(l => l.CreatedAt < cutoffDate);

        await _unitOfWork.ActivityLogs.DeleteRangeAsync(oldLogs);
        await _unitOfWork.SaveChangesAsync();
    }

    private Expression<Func<ActivityLog, bool>> CombinePredicates(Expression<Func<ActivityLog, bool>> first, Expression<Func<ActivityLog, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(ActivityLog));
        var firstBody = Expression.Invoke(first, parameter);
        var secondBody = Expression.Invoke(second, parameter);
        var combined = Expression.AndAlso(firstBody, secondBody);
        return Expression.Lambda<Func<ActivityLog, bool>>(combined, parameter);
    }

    private Func<IQueryable<ActivityLog>, IOrderedQueryable<ActivityLog>> BuildOrderBy(string? sortBy, string? sortDirection)
    {
        var isDesc = sortDirection?.ToLower() == "desc";
        
        return sortBy?.ToLower() switch
        {
            "userid" => isDesc ? q => q.OrderByDescending(l => l.UserId) : q => q.OrderBy(l => l.UserId),
            "action" => isDesc ? q => q.OrderByDescending(l => l.Action) : q => q.OrderBy(l => l.Action),
            "module" => isDesc ? q => q.OrderByDescending(l => l.Module) : q => q.OrderBy(l => l.Module),
            "createdat" => isDesc ? q => q.OrderByDescending(l => l.CreatedAt) : q => q.OrderBy(l => l.CreatedAt),
            _ => isDesc ? q => q.OrderByDescending(l => l.CreatedAt) : q => q.OrderByDescending(l => l.CreatedAt)
        };
    }
}