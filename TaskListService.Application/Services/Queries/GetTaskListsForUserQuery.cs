using TaskService.Domain.Common;

namespace TaskListService.Application.Services.Queries;

public record GetTaskListsForUserQuery(
    string UserId,
    int  Page,
    int PageSize);