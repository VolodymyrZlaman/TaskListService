namespace TaskService.Domain.Entities;

public class TaskList
{
    public string Id { get; set; } 
    public string Name { get; set; } 
    public string OwnerId { get; set; }
    public DateTime CreationDate { get; set; }
    public List<string> SharedWith { get; set; } = new();
}