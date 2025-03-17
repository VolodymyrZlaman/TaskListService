using TaskService.Domain.Entities;

namespace TaskListService.Application.Contracts.Persistence;

public interface ITaskListRepository : IBaseRepository<TaskList>
{
    Task ShareTaskListAsync(string taskListId, string targetUserId, string userId);
    Task<IEnumerable<string>> GetSharedUsersAsync(string taskListId, string userId);
    Task UnshareTaskListAsync(string taskListId, string targetUserId, string userId);
    
    
}