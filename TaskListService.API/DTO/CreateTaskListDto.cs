using System.ComponentModel.DataAnnotations;

namespace TaskListService.API.DTO;

public record CreateTaskListDto(string TaskListId, string Name);
