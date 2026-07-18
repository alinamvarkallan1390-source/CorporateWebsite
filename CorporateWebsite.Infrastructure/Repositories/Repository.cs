using CorporateWebsite.Core.Entities;
using CorporateWebsite.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CorporateWebsite.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet.Where(predicate);
        if (orderBy != null)
        {
            query = orderBy(query);
        }
        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet.Where(predicate);
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        if (orderBy != null)
        {
            query = orderBy(query);
        }
        return await query.ToListAsync();
    }

    public virtual async Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        
        var totalCount = await query.CountAsync();
        
        if (orderBy != null)
        {
            query = orderBy(query);
        }
        
        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (items, totalCount);
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public virtual async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.UpdateRange(entities);
        await Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public virtual async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }
        _dbSet.UpdateRange(entities);
        await Task.CompletedTask;
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
        {
            return await _dbSet.CountAsync(cancellationToken);
        }
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return await query.FirstOrDefaultAsync(predicate);
    }

    public virtual IQueryable<T> Query()
    {
        return _dbSet;
    }

    public virtual IQueryable<T> QueryNoTracking()
    {
        return _dbSet.AsNoTracking();
    }
}