using Microsoft.AspNetCore.Mvc;
using TaskListService.Application.Contracts.Aplication;
using TaskListService.Application.Services.Commands;
using TaskListService.Application.Services.Queries;
using TaskListService.Application.Services.Vm;
using TaskService.Domain.Common;

namespace TaskListService.API.Controllers;

/// <summary>
/// Controller for managing task lists
/// </summary>
[ApiController]
[Route("api/task-lists")]
public class TaskListController(ITaskListService taskListService) : ControllerBase
{

    [HttpPost]
    [ProducesResponseType(typeof(Result<TaskListItemVm>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<TaskListItemVm>>> CreateTaskList([FromBody] CreateTaskListCommand command)
    {
        var result = await taskListService.CreateTaskListAsync(command);
        
        if (result.IsFailure)
            return BadRequest(Result.Failure(result.Error));

        return CreatedAtAction(
            nameof(GetTaskList),
            new { taskListId = result.Value.Id },
            Result<TaskListItemVm>.Success(result.Value));
    }
    
    [HttpGet("{taskListId}")]
    [ProducesResponseType(typeof(Result<TaskListItemVm>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<TaskListItemVm>>> GetTaskList(string taskListId, [FromQuery] string userId)
    {
        var result = await taskListService.GetTaskListItemAsync(taskListId, userId);
        
        if (result.IsFailure)
            return NotFound(Result.Failure(result.Error));

        return Ok(Result<TaskListItemVm>.Success(result.Value));
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResult<ShortTaskListItemVm>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<PagedResult<ShortTaskListItemVm>>>> GetTaskListsForUser(
        [FromQuery] string userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await taskListService.GetTaskListsForUserAsync(new GetTaskListsForUserQuery(userId, page, pageSize));
        
        if (result.IsFailure)
            return BadRequest(Result.Failure(result.Error));

        return Ok(Result<PagedResult<ShortTaskListItemVm>>.Success(result.Value));
    }
    
    [HttpPut("{taskListId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateTaskList(
        string taskListId,
        [FromQuery] string userId,
        [FromBody] UpdateTaskListCommand command)
    {
        var result = await taskListService.UpdateTaskListAsync(taskListId, userId, command);

        if (!result.IsFailure) return NoContent();
        if (result.Error != null && result.Error.Contains("not found"))
            return NotFound(Result.Failure(result.Error));
                
        return BadRequest(Result.Failure(result.Error));

    }
    
    [HttpDelete("{taskListId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteTaskList(string taskListId, [FromQuery] string userId)
    {
        var result = await taskListService.DeleteTaskListAsync(taskListId, userId);

        if (!result.IsFailure) return NoContent();
        if (result.Error != null && result.Error.Contains("not found"))
            return NotFound(Result.Failure(result.Error));
                
        return BadRequest(Result.Failure(result.Error));

    }
    
    [HttpPost("{taskListId}/share")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ShareTaskList(
        string taskListId,
        [FromQuery] string targetUserId,
        [FromQuery] string userId)
    {
        var result = await taskListService.ShareTaskListAsync(taskListId, targetUserId, userId);

        if (!result.IsFailure) return NoContent();
        if (result.Error != null && result.Error.Contains("not found"))
            return NotFound(Result.Failure(result.Error));
                
        return BadRequest(Result.Failure(result.Error));

    }
    
    [HttpGet("{taskListId}/shared-users")]
    [ProducesResponseType(typeof(Result<IEnumerable<string>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<IEnumerable<string>>>> GetSharedUsers(
        string taskListId,
        [FromQuery] string userId)
    {
        var result = await taskListService.GetSharedUsersAsync(taskListId, userId);

        if (result.IsFailure)
            return NotFound(Result.Failure(result.Error));

        return Ok(Result<IEnumerable<string>>.Success(result.Value));
    }
    
    
    [HttpDelete("{taskListId}/unshare")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UnshareTaskList(
        string taskListId,
        [FromQuery] string targetUserId,
        [FromQuery] string userId)
    {
        var result = await taskListService.UnshareTaskListAsync(taskListId, targetUserId, userId);

        if (!result.IsFailure) return NoContent();
        if (result.Error != null && result.Error.Contains("not found"))
            return NotFound(Result.Failure(result.Error));
                
        return BadRequest(Result.Failure(result.Error));

    }
}