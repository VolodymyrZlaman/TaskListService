using TaskListService.Application.Contracts.Persistence;
using TaskListService.Persistence.Context;
using TaskService.Domain.Common;
using TaskService.Domain.Entities;

namespace TaskListService.Persistence.Repository;

public class TaskListRepository(IDbContext context) : BaseRepository<TaskList>(context), ITaskListRepository
{
    private readonly IDbContext _context = context;

    public async Task<Result> ShareTaskListAsync(string taskListId, string targetUserId, string userId)
    {
        // Get the task list
        var taskList = await _context.GetOneAsync<TaskList>(x => x.Id == taskListId);
        if (taskList.IsFailure || taskList.Value == null)
            return Result.Failure("TaskList not found");
        if (taskList.Value.OwnerId != userId && !taskList.Value.SharedWith.Contains(userId))
            return Result.Failure("You don't have access to this TaskList");
        if (taskList.Value.OwnerId == targetUserId)
            return Result.Failure("Cannot share TaskList with its owner");
        if (taskList.Value.SharedWith.Contains(targetUserId))
            return Result.Failure("User already has access to this TaskList");
        
        // Add target user to shared list
        taskList.Value?.SharedWith.Add(targetUserId);

        // Update the task list
        return await _context.UpdateAsync(
            x => x != null && x.Id == taskListId,
            taskList.Value);
    }

    public async Task<Result<IEnumerable<string>>> GetSharedUsersAsync(string taskListId, string userId)
    {
        var taskList = await _context.GetOneAsync<TaskList>(x => x.Id == taskListId);
        if (taskList.IsFailure || taskList.Value == null)
            return Result<IEnumerable<string>>.Failure("TaskList not found");
        if (taskList.Value.OwnerId != userId && !taskList.Value.SharedWith.Contains(userId))
            return Result<IEnumerable<string>>.Failure("You don't have access to this TaskList");

        return Result<IEnumerable<string>>.Success(taskList.Value.SharedWith);
    }

    public async Task<Result> UnshareTaskListAsync(string taskListId, string targetUserId, string userId)
    {
        var taskList = await _context.GetOneAsync<TaskList>(x => x.Id == taskListId);
        if (taskList.IsFailure || taskList.Value == null)
            return Result.Failure("TaskList not found");
        if (taskList.Value.OwnerId != userId && !taskList.Value.SharedWith.Contains(userId))
            return Result.Failure("You don't have access to this TaskList");
        
        if (taskList.Value.SharedWith.Remove(targetUserId))
        {
            return await _context.UpdateAsync(
                x => x.Id == taskListId,
                taskList.Value);
        }
        else
        {
            return Result.Failure("You don't have access to this TaskList");
        }
    }
}
