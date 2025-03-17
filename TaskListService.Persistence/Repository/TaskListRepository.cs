using MongoDB.Driver;
using TaskListService.Application.Contracts.Persistence;
using TaskListService.Application.Exceptions;
using TaskListService.Persistence.Extentions;
using TaskService.Domain.Entities;

namespace TaskListService.Persistence.Repository;

public class TaskListRepository(IMongoDatabase database) : BaseRepository<TaskList>(database), ITaskListRepository
{
    private readonly IMongoCollection<TaskList> _collection = database.GetCollection<TaskList>("TaskList");
    
    public async Task ShareTaskListAsync(string taskListId, string? targetUserId, string userId)
    {
        var filter = _collection.CreateTaskListFilter(taskListId,  targetUserId, userId, true);
        var builder = Builders<TaskList>.Update.AddToSet(d => d.SharedWith, targetUserId);
        
        var result = await _collection.UpdateOneAsync(filter, builder);

        if (result.ModifiedCount == 0)
        {
            throw new NotFoundException("Unable to share the task list.", taskListId);
        }
    }

    public async Task<IEnumerable<string>> GetSharedUsersAsync(string taskListId, string userId)
    {
        var filter = _collection.CreateTaskListFilter(taskListId, null, userId);
        var result = await _collection.Find(filter).FirstOrDefaultAsync();

        if (result == null)
        {
            throw new NotFoundException("Unable to get the task list.", taskListId);
        }

        return result.SharedWith;
    }

    public async Task UnshareTaskListAsync(string taskListId, string targetUserId, string userId)
    {
        var filter = _collection.CreateTaskListFilter(taskListId, targetUserId,userId);
        var builder = Builders<TaskList>.Update.Pull(d => d.SharedWith, targetUserId);
        
        var result = await _collection.UpdateOneAsync(filter, builder);

        if (result.ModifiedCount == 0)
        {
            throw new NotFoundException("Unable to unshare the task list.", taskListId);
        }
    }
}
