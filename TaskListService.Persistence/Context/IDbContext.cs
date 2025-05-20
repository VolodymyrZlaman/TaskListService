using System.Linq.Expressions;
using TaskService.Domain.Common;

namespace TaskListService.Persistence.Context;

/// <summary>
/// Generic database context interface that can work with any database type
/// </summary>
public interface IDbContext
{
    /// <summary>
    /// Gets the collection/table for a specific entity type
    /// </summary>
    IQueryable<T> GetCollection<T>() where T : class;
    
    /// <summary>
    /// Adds a new entity to the database
    /// </summary>
    Task<Result<T>> AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class;
    
    /// <summary>
    /// Updates an existing entity in the database
    /// </summary>
    Task<Result> UpdateAsync<T>(Expression<Func<T, bool>> filter, T entity, CancellationToken cancellationToken = default) where T : class;
    
    /// <summary>
    /// Deletes an entity from the database
    /// </summary>
    Task<Result<bool>> DeleteAsync<T>(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default) where T : class;
    
    /// <summary>
    /// Gets a single entity by filter
    /// </summary>
    Task<Result<T?>> GetOneAsync<T>(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default) where T : class;
    
    /// <summary>
    /// Gets a paged list of entities by filter
    /// </summary>
    Task<Result<PagedResult<T>>> GetPagedAsync<T>(
        Expression<Func<T, bool>> filter,
        int pageNumber,
        int pageSize,
        Expression<Func<T, object>>? orderBy = null,
        bool descending = false,
        CancellationToken cancellationToken = default) where T : class;
} 