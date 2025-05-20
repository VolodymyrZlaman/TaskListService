using System.Linq.Expressions;
using TaskService.Domain.Common;

namespace TaskListService.Application.Contracts.Persistence;

public interface IBaseRepository<T> where T : class
{
    Task<Result<T?>> GetOneByFilterAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default);

    Task<Result<PagedResult<T>>> GetByFilterAsync(
        Expression<Func<T, bool>> filter,
        bool descending,
        int page,
        int pageSize,
        Expression<Func<T, object>>? sortBy = null,
        CancellationToken cancellationToken = default);

    Task<Result<T>> CreateAsync(T entity, CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(
        Expression<Func<T, bool>> filter,
        object updateObject,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> DeleteAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default);
}