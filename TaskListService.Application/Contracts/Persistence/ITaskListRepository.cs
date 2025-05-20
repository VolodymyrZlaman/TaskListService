using TaskService.Domain.Common;
using TaskService.Domain.Entities;

namespace TaskListService.Application.Contracts.Persistence;

public interface ITaskListRepository : IBaseRepository<TaskList>
{
    Task<Result> ShareTaskListAsync(string taskListId, string targetUserId, string userId);
    Task<Result<IEnumerable<string>>> GetSharedUsersAsync(string taskListId, string userId);
    Task<Result> UnshareTaskListAsync(string taskListId, string targetUserId, string userId);
    
    
}