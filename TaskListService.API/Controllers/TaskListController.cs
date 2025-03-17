using Microsoft.AspNetCore.Mvc;
using TaskListService.Application.Contracts.Aplication;
using TaskListService.Application.Services.Commands;
using TaskListService.Application.Services.Vm;

namespace TaskListService.API.Controllers
{
    [ApiController]
    [Route("api/task-lists")]
    public class TaskListController(ITaskListService taskListService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateTaskList([FromBody] CreateTaskListCommand command)
        {
            var result = await taskListService.CreateTaskListAsync(command);
            return Ok(result);
        }

        [HttpGet("{taskListId}")]
        public async Task<IActionResult> GetTaskList(string taskListId, [FromQuery] string userId)
        {
            var result = await taskListService.GetTaskListItemAsync(taskListId, userId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTaskListsForUser([FromQuery] string userId, [FromQuery] int page,
            [FromQuery] int pageSize)
        {
            var result = await taskListService.GetTaskListsForUserAsync(userId, page, pageSize);
            return Ok(result);
        }

        [HttpPut("{taskListId}")]
        public async Task<IActionResult> UpdateTaskList(string taskListId, [FromQuery] string userId,
            [FromBody] UpdateTaskListCommand updateTaskListCommand)
        {
            await taskListService.UpdateTaskListAsync(taskListId, userId, updateTaskListCommand);
            return NoContent();
        }

        [HttpDelete("{taskListId}")]
        public async Task<IActionResult> DeleteTaskList(string taskListId, [FromQuery] string userId)
        {
            await taskListService.DeleteTaskListAsync(taskListId, userId);
            return NoContent();
        }

        [HttpPost("{taskListId}/share")]
        public async Task<IActionResult> ShareTaskList(string taskListId, [FromQuery] string targetUserId,
            [FromQuery] string? userId)
        {
            await taskListService.ShareTaskListAsync(taskListId, targetUserId, userId);
            return NoContent();
        }

        [HttpGet("{taskListId}/shared-users")]
        public async Task<IActionResult> GetSharedUsers(string taskListId, [FromQuery] string userId)
        {
            var result = await taskListService.GetSharedUsersAsync(taskListId, userId);
            return Ok(result);
        }

        [HttpDelete("{taskListId}/unshared")]
        public async Task<IActionResult> UnshareTaskList(string taskListId, [FromQuery] string targetUserId,
            [FromQuery] string userId)
        {
            await taskListService.UnshareTaskListAsync(taskListId, targetUserId, userId);
            return NoContent();
        }
    }
}