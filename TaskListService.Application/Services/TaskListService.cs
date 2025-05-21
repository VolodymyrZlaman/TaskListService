using AutoMapper;
using TaskListService.Application.Contracts.Aplication;
using TaskListService.Application.Contracts.Persistence;
using TaskListService.Application.Exceptions;
using TaskListService.Application.Services.Commands;
using TaskListService.Application.Services.Queries;
using TaskListService.Application.Services.Vm;
using TaskListService.Application.Validators;
using TaskService.Domain.Common;
using TaskService.Domain.Entities;

namespace TaskListService.Application.Services;

public class TaskListService(ITaskListRepository repository, IMapper mapper ) : ITaskListService
{
    public async Task<Result<TaskListItemVm>> CreateTaskListAsync(CreateTaskListCommand command)
    {
        var validator = new CreateTaskListCommandValidator();
        var validationResult = await validator.ValidateAsync(command);
        
        if (validationResult.Errors.Count > 0)
            return Result<TaskListItemVm>.Failure(new ValidationException(validationResult));
        

        var taskList = mapper.Map<TaskList>(command);
        taskList.CreationDate = DateTime.Now;
        var response = await repository.CreateAsync(taskList);
        
        return response.IsFailure ? Result<TaskListItemVm>.Failure("Internal server error") : Result<TaskListItemVm>.Success(mapper.Map<TaskListItemVm>(response.Value));
    }

    public async Task<Result<TaskListItemVm>> GetTaskListItemAsync(string taskListId, string userId)
    {
        var taskList = await repository.GetOneByFilterAsync( x => x.Id == taskListId && x.OwnerId == userId || x.SharedWith.Contains(userId));
        return Result<TaskListItemVm>.Success(mapper.Map<TaskListItemVm>(taskList.Value));
    }

    public async Task<Result<PagedResult<ShortTaskListItemVm>>> GetTaskListsForUserAsync(GetTaskListsForUserQuery query)
    {
        var validator = new GetTaskListsForUserQueryValidator();
        var validationResult = await validator.ValidateAsync(query);
        
        if (validationResult.Errors.Count > 0)
            return Result<PagedResult<ShortTaskListItemVm>>.Failure(new ValidationException(validationResult));
        
        var taskLists = await repository.GetByFilterAsync(
            x => x.OwnerId == query.UserId || x.SharedWith.Contains(query.UserId),
            page: query.Page,
            pageSize: query.PageSize,
            sortBy: x => x.CreationDate,
            descending: true
        );
        return taskLists.IsFailure ? Result<PagedResult<ShortTaskListItemVm>>.Failure("Internal server error") : Result<PagedResult<ShortTaskListItemVm>>.Success(PagedResult<ShortTaskListItemVm>.Create(mapper.Map<IEnumerable<ShortTaskListItemVm>>(taskLists.Value.Items) , taskLists.Value.TotalCount, taskLists.Value.PageNumber , taskLists.Value.PageSize ));
    }

    public async Task<Result> UpdateTaskListAsync(string taskListId, string userId, UpdateTaskListCommand updateTaskListCommand)
    {
        var validator = new UpdateTaskListCommandValidator();
        var validationResult = await validator.ValidateAsync(updateTaskListCommand);
        
        if (validationResult.Errors.Count > 0)
            return Result.Failure(new ValidationException(validationResult));
        
        var taskList = await repository.GetOneByFilterAsync(x => x.Id == taskListId && (x.OwnerId == userId || x.SharedWith.Contains(userId)));

        if (!taskList.IsFailure)
        {
            return await repository.UpdateAsync(
                x => x.Id == taskListId && (x.OwnerId == userId || x.SharedWith.Contains(userId)),
                updateTaskListCommand
            );
        }

        return Result.Failure(new NotFoundException("TaskList not found", taskListId));
    }

    public async Task<Result<bool>> DeleteTaskListAsync(string taskListId, string userId)
    {
        return await repository.DeleteAsync(x => x.Id == taskListId && x.OwnerId == userId);
    }

    public async Task<Result> ShareTaskListAsync(string taskListId, string targetUserId, string userId)
    {
        return await repository.ShareTaskListAsync(taskListId, targetUserId, userId);
    }

    public async Task<Result<IEnumerable<string>>> GetSharedUsersAsync(string taskListId, string userId)
    {
       return await repository.GetSharedUsersAsync(taskListId, userId);
    }

    public async Task<Result> UnshareTaskListAsync(string taskListId, string targetUserId, string userId)
    {
        return await repository.UnshareTaskListAsync(taskListId, targetUserId, userId);
    }
}