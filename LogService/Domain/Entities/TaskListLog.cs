namespace LogService.Domain.Entities;

public class TaskListLog
{
    public string Id { get; set; } 
    public string EventId { get; set; }
    public string EventType { get; set; } 
    public string TaskListId { get; set; }
    public string OwnerId { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
} 