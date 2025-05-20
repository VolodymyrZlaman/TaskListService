using System.Collections;
using System.Linq.Expressions;
using TaskListService.Application.Contracts.Persistence;
using TaskListService.Application.Exceptions;
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
        var taskList = await _context.GetOneAsync<TaskList>(
            x => x.Id == taskListId && 
                 (x.OwnerId == userId || x.SharedWith.Contains(userId)) &&
                 x.OwnerId != targetUserId &&
                 !x.SharedWith.Contains(targetUserId));

        if (taskList.IsFailure)
        {
            Result.Failure(new NotFoundException("Unable to share the task list.", taskListId));
        }

        // Add target user to shared list
        taskList.Value?.SharedWith.Add(targetUserId);

        // Update the task list
        return await _context.UpdateAsync(
            x => x.Value != null && x.Value.Id == taskListId,
            taskList);
    }

    public async Task<Result<IEnumerable<string>>> GetSharedUsersAsync(string taskListId, string userId)
    {
        var taskList = await _context.GetOneAsync<TaskList>(
            x => x.Id == taskListId && 
                 (x.OwnerId == userId || x.SharedWith.Contains(userId)));

        if (taskList.IsFailure)
        {
            return Result<IEnumerable<string>>.Failure(new NotFoundException("Unable to get the task list.", taskListId));
        }

        return Result<IEnumerable<string>>.Success(taskList.Value.SharedWith);
    }

    public async Task<Result> UnshareTaskListAsync(string taskListId, string targetUserId, string userId)
    {
        // Get the task list
        var taskList = await _context.GetOneAsync<TaskList>(
            x => x.Id == taskListId && 
                 (x.OwnerId == userId || x.SharedWith.Contains(userId)));

        if (taskList.IsFailure)
        {
            return Result.Failure(new NotFoundException("Unable to unshare the task list.", taskListId));
        }

        // Remove target user from shared list
        if (taskList.Value.SharedWith.Remove(targetUserId))
        {
            // Update the task list only if the user was actually removed
            return await _context.UpdateAsync<TaskList>(
                x => x.Id == taskListId,
                taskList.Value);
        }

        return Result.Failure(new NotFoundException("User was not shared with this task list.", taskListId));
    }
}
