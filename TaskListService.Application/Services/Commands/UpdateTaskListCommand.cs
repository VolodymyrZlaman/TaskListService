using System.ComponentModel.DataAnnotations;

namespace TaskListService.Application.Services.Commands;

public class UpdateTaskListCommand
{
    [Required]
    public string Name {get; set; }
}