using System.Linq.Expressions;
using MongoDB.Driver;
using TaskService.Domain.Common;

namespace TaskListService.Persistence.Context;

public class MongoDbContext(IMongoDatabase database) : IDbContext
{
    public IQueryable<T> GetCollection<T>() where T : class
    {
        return database.GetCollection<T>(typeof(T).Name).AsQueryable();
    }

    public async Task<Result<T>> AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
    {
        var collection = database.GetCollection<T>(typeof(T).Name);
        try
        {
            await collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
            return Result<T>.Success(entity);
        }
        catch (MongoWriteException ex)
        {
            return Result<T>.Failure(ex.WriteError.Message);
        }
        catch (Exception e)
        {
            return Result<T>.Failure(e.Message);
        }
    }

    public async Task<Result> UpdateAsync<T>(Expression<Func<T, bool>> filter, T entity, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var collection = database.GetCollection<T>(typeof(T).Name);
            var result = await collection.ReplaceOneAsync(filter, entity, new ReplaceOptions { IsUpsert = false }, cancellationToken);
            return result.IsModifiedCountAvailable ? Result.Success() : Result.Failure(result.ToString());
        }
        catch (Exception e)
        {
            return Result.Failure(e.Message);
        }
    }

    public async Task<Result<bool>> DeleteAsync<T>(Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default) where T : class
    {
        var collection = database.GetCollection<T>(typeof(T).Name);
        var result = await collection.DeleteOneAsync(filter, cancellationToken);
        if (result.DeletedCount == 1)
            return Result.Success(true);
        else
        {
            return Result.Failure<bool>($"Was deleted{
                result.DeletedCount.ToString()}");
        }
        
    }

    public async Task<Result<T?>> GetOneAsync<T>(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var collection = database.GetCollection<T>(typeof(T).Name);
        
            var result =  await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
            
            return result == null ? Result<T?>.Failure("Item was not found") : Result<T?>.Success(result);
        }
        catch (Exception e)
        {
            return Result<T?>.Failure(e.Message);
        }

    }

    public async Task<Result<PagedResult<T>>> GetPagedAsync<T>(
        Expression<Func<T, bool>> filter,
        int pageNumber,
        int pageSize,
        Expression<Func<T, object>>? orderBy = null,
        bool descending = false,
        CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var collection = database.GetCollection<T>(typeof(T).Name);
            var query = collection.Find(filter);

            if (orderBy != null)
            {
                query = descending 
                    ? query.SortByDescending(orderBy) 
                    : query.SortBy(orderBy);
            }
            
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize + 1)
                .ToListAsync(cancellationToken);

            var hasNextPage = items.Count > pageSize;
            var actualItems = hasNextPage ? items.Take(pageSize) : items;
            
            int totalCount;
            if (!hasNextPage)
            {
                totalCount = (pageNumber - 1) * pageSize + items.Count;
            }
            else
            {
                totalCount = pageNumber * pageSize + 1;
            }

            return Result<PagedResult<T>>.Success(PagedResult<T>.Create(actualItems, totalCount, pageNumber, pageSize));
        }
        catch (Exception e)
        {
            return Result<PagedResult<T>>.Failure(e.Message);
        }
       
    }
} 