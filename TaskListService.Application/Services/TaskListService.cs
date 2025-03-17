using AutoMapper;
using TaskListService.Application.Contracts.Aplication;
using TaskListService.Application.Contracts.Persistence;
using TaskListService.Application.Exceptions;
using TaskListService.Application.Services.Commands;
using TaskListService.Application.Services.Vm;
using TaskListService.Application.Validators;
using TaskService.Domain.Entities;

namespace TaskListService.Application.Services;

public class TaskListService(ITaskListRepository repository, IMapper mapper ) : ITaskListService
{
    public async Task<TaskListItemVm> CreateTaskListAsync(CreateTaskListCommand command)
    {
        var validator = new CreateTaskListCommandValidator();
        var validationResult = await validator.ValidateAsync(command);
        
        if (validationResult.Errors.Count > 0)
            throw new ValidationException(validationResult);
        

        var taskList = mapper.Map<TaskList>(command);
        taskList.CreationDate = DateTime.Now;
        var response = await repository.CreateAsync(taskList);
        return mapper.Map<TaskListItemVm>(response);
    }

    public async Task<TaskListItemVm> GetTaskListItemAsync(string taskListId, string userId)
    {
        var taskList = await repository.GetOneByFilterAsync( x => x.Id == taskListId && x.OwnerId == userId || x.SharedWith.Contains(userId));
        return mapper.Map<TaskListItemVm>(taskList);
    }

    public async Task<IEnumerable<ShortTaskListItemVm>> GetTaskListsForUserAsync(string userId, int page, int pageSize)
    {
        var taskLists = await repository.GetByFilterAsync(
            x => x.OwnerId == userId || x.SharedWith.Contains(userId),
            page: page,
            pageSize: pageSize,
            sortBy: x => x.CreationDate,
            descending: true
        );
        return mapper.Map<IEnumerable<ShortTaskListItemVm>>(taskLists);
    }

    public async Task UpdateTaskListAsync(string taskListId, string userId, UpdateTaskListCommand updateTaskListCommand)
    {
        var taskList = await repository.GetOneByFilterAsync(x => x.Id == taskListId && (x.OwnerId == userId || x.SharedWith.Contains(userId)));

        if (taskList != null)
        {
            await repository.UpdateAsync(
                x => x.Id == taskListId && (x.OwnerId == userId || x.SharedWith.Contains(userId)),
                updateTaskListCommand
            );
        }
        else
        {
            throw new NotFoundException("TaskList not found", taskListId);
        }
    }

    public async Task DeleteTaskListAsync(string taskListId, string userId)
    {
        await repository.DeleteAsync(x => x.Id == taskListId && x.OwnerId == userId);
    }

    public async Task ShareTaskListAsync(string taskListId, string targetUserId, string userId)
    {
        await repository.ShareTaskListAsync(taskListId, targetUserId, userId);
    }

    public async Task<IEnumerable<string>> GetSharedUsersAsync(string taskListId, string userId)
    {
       return await repository.GetSharedUsersAsync(taskListId, userId);
    }

    public async Task UnshareTaskListAsync(string taskListId, string targetUserId, string userId)
    {
        await repository.UnshareTaskListAsync(taskListId, targetUserId, userId);
    }
}