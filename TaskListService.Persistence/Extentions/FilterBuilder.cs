using MongoDB.Driver;
using TaskService.Domain.Entities;

namespace TaskListService.Persistence.Extentions;

public static class FilterBuilder
{
    public static FilterDefinition<TaskList> CreateTaskListFilter(
        this IMongoCollection<TaskList> collection, 
        string taskListId, 
        string? targetUserId,
        string userId, 
        bool excludeTargetUser = false)
    {

        var filter = Builders<TaskList>.Filter.Eq(x => x.Id, taskListId);
        
        var ownerOrSharedWithFilter = Builders<TaskList>.Filter.Or(
            Builders<TaskList>.Filter.Eq(i => i.OwnerId, userId),
            Builders<TaskList>.Filter.AnyEq(i => i.SharedWith, userId)
        );
        
        filter = Builders<TaskList>.Filter.And(filter, ownerOrSharedWithFilter);
        
        if (excludeTargetUser && targetUserId != null)
        {
            filter = Builders<TaskList>.Filter.And(
                filter,
                Builders<TaskList>.Filter.Not(Builders<TaskList>.Filter.Eq(i => i.OwnerId, targetUserId)),
                Builders<TaskList>.Filter.Not(Builders<TaskList>.Filter.AnyEq(i => i.SharedWith, targetUserId))
            );
        }

        return filter;
    }


}