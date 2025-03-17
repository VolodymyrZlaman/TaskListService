using System.Linq.Expressions;
using MongoDB.Driver;
using TaskListService.Application.Contracts.Persistence;
using TaskService.Domain.Entities;

namespace TaskListService.Persistence.Repository;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    private readonly IMongoCollection<T> _collection;

    protected BaseRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<T>(typeof(T).Name);
    }

    public async Task<IEnumerable<T>> GetByFilterAsync(
        Expression<Func<T, bool>> filter,
        bool descending,
        int page,
        int pageSize,
        Expression<Func<T, object>>? sortBy = null,
        CancellationToken cancellationToken = default)
    {
        var query = _collection.Find(filter);
        if (sortBy != null)
        {
            query = descending ? query.SortByDescending(sortBy) : query.SortBy(sortBy);
        }

        return await query
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<T?> GetOneByFilterAsync(
        Expression<Func<T, bool>> filter, 
        CancellationToken cancellationToken = default)
    {
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(
        Expression<Func<T, bool>> filter,
        object updateObject,
        CancellationToken cancellationToken = default)
    {
        var properties = updateObject.GetType().GetProperties()
            .Where(p => p.GetValue(updateObject) != null)
            .ToList();

        if (!properties.Any()) return;

        var updateDefinitions = properties
            .Select(p => Builders<T>.Update.Set(p.Name, p.GetValue(updateObject)!))
            .ToList();

        var updateDefinition = Builders<T>.Update.Combine(updateDefinitions);
        var result = await _collection.UpdateOneAsync(filter, updateDefinition, cancellationToken: cancellationToken);

        if (result.MatchedCount == 0)
        {
            throw new UnauthorizedAccessException("You don't have permission to update this item.");
        }
    }

    public async Task DeleteAsync(
        Expression<Func<T, bool>> filter, 
        CancellationToken cancellationToken = default)
    {
        var result = await _collection.DeleteOneAsync(filter, cancellationToken);
        if (result.DeletedCount == 0)
        {
            throw new UnauthorizedAccessException("You don't have permission to delete this item.");
        }
    }

    public async Task EnsureIndexAsync(
        Expression<Func<T, object>> field,
        bool unique = false,
        CancellationToken cancellationToken = default)
    {
        var indexKeys = Builders<T>.IndexKeys.Ascending(field);
        var indexOptions = new CreateIndexOptions { Unique = unique };
        var indexModel = new CreateIndexModel<T>(indexKeys, indexOptions);
        await _collection.Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);
    }
}
