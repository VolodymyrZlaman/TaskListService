using System.ComponentModel.DataAnnotations;

namespace TaskListService.Application.Services.Commands;

public class CreateTaskListCommand
{
    [Required]
    public string TaskListId { get; set; }
    [Required]
    public string Name {get; set; }
    [Required]
    public string OwnerId { get; set; }
}