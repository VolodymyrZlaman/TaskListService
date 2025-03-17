using System.Linq.Expressions;

namespace TaskListService.Application.Contracts.Persistence;

public interface IBaseRepository<T> where T : class
{
    Task<T?> GetOneByFilterAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> GetByFilterAsync(
        Expression<Func<T, bool>> filter,
        bool descending,
        int page,
        int pageSize,
        Expression<Func<T, object>>? sortBy = null,
        CancellationToken cancellationToken = default);

    Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Expression<Func<T, bool>> filter,
        object updateObject,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default);

    Task EnsureIndexAsync(
        Expression<Func<T, object>> field,
        bool unique = false,
        CancellationToken cancellationToken = default);
}