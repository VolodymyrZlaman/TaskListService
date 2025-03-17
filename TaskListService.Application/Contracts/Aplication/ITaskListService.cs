using TaskListService.Application.Services.Commands;
using TaskListService.Application.Services.Vm;

namespace TaskListService.Application.Contracts.Aplication;

public interface ITaskListService
{
    Task<TaskListItemVm> CreateTaskListAsync(CreateTaskListCommand command);
    Task<TaskListItemVm> GetTaskListItemAsync(string taskListId, string userId);
    Task<IEnumerable<ShortTaskListItemVm>> GetTaskListsForUserAsync(string userId, int page, int pageSize);
    Task UpdateTaskListAsync(string taskListId, string userId , UpdateTaskListCommand taskListItemVm);
    Task DeleteTaskListAsync(string taskListId, string userId);
    Task ShareTaskListAsync(string taskListId, string targetUserId, string userId);
    Task<IEnumerable<string>> GetSharedUsersAsync(string taskListId, string userId);
    Task UnshareTaskListAsync(string taskListId, string targetUserId, string userId);
}