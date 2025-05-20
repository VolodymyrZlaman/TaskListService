using System.Linq.Expressions;
using TaskListService.Application.Contracts.Persistence;
using TaskListService.Persistence.Context;
using TaskService.Domain.Common;

namespace TaskListService.Persistence.Repository;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    private readonly IDbContext _context;

    protected BaseRepository(IDbContext context)
    {
        _context = context;
    }

    public virtual async Task<Result<T>> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        return await _context.AddAsync(entity, cancellationToken);
    }

    public virtual async Task<Result<bool>> DeleteAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await _context.DeleteAsync(filter, cancellationToken);
    }

    public virtual async Task<Result<PagedResult<T>>> GetByFilterAsync(
        Expression<Func<T, bool>> filter,
        bool descending,
        int page,
        int pageSize,
        Expression<Func<T, object>>? sortBy = null,
        CancellationToken cancellationToken = default)
    {
        return await _context.GetPagedAsync(
            filter,
            page,
            pageSize,
            sortBy,
            descending,
            cancellationToken);
    }

    public virtual async Task<Result<T?>> GetOneByFilterAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        return await _context.GetOneAsync(filter, cancellationToken);
    }

    public virtual async Task<Result> UpdateAsync(
        Expression<Func<T, bool>> filter,
        object updateObject,
        CancellationToken cancellationToken = default)
    {
        if (updateObject is T entity)
        {
            return await _context.UpdateAsync(filter, entity, cancellationToken);
        }
        else
        {
            return Result.Failure("Something went wrong");
        }
    }
}
