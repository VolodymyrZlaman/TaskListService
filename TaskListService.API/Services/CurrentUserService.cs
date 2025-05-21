using TaskListService.Application.Contracts.Infrastructure;

namespace TaskListService.API.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private const string UserIdHeader = "X-UserId";

    public string UserId => httpContextAccessor.HttpContext?.Request.Headers[UserIdHeader].FirstOrDefault() ?? string.Empty;

    public bool IsAuthenticated => !string.IsNullOrEmpty(UserId);
} 