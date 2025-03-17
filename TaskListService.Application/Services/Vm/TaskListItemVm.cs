namespace TaskListService.Application.Services.Vm;

public class TaskListItemVm
{
    public string Id { get; set; } 
    public string Name { get; set; } 
    public string OwnerId { get; set; }
    public DateTime CreationDate { get; set; }
    public List<string> SharedUsersIdList { get; set; } = [];
}