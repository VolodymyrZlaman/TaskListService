using System.ComponentModel.DataAnnotations;

namespace TaskListService.Application.Services.Commands;

public record CreateTaskListCommand
    (string TaskListId , string Name, string OwnerId);
