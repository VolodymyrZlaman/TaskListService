using TaskListService.Application.Services.Commands;
using TaskListService.Application.Services.Queries;
using TaskListService.Application.Services.Vm;
using TaskService.Domain.Common;

namespace TaskListService.Application.Contracts.Aplication;

public interface ITaskListService
{
    Task<Result<TaskListItemVm>> CreateTaskListAsync(CreateTaskListCommand command);
    Task<Result<TaskListItemVm>> GetTaskListItemAsync(string taskListId, string userId);
    Task<Result<PagedResult<ShortTaskListItemVm>>> GetTaskListsForUserAsync(GetTaskListsForUserQuery query);
    Task<Result> UpdateTaskListAsync(string taskListId, string userId , UpdateTaskListCommand taskListItemVm);
    Task<Result<bool>> DeleteTaskListAsync(string taskListId, string userId);
    Task<Result> ShareTaskListAsync(string taskListId, string targetUserId, string userId);
    Task<Result<IEnumerable<string>>> GetSharedUsersAsync(string taskListId, string userId);
    Task<Result> UnshareTaskListAsync(string taskListId, string targetUserId, string userId);
}