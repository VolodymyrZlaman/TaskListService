namespace TaskListService.Application.Contracts.Infrastructure;

/// <summary>
/// Service for accessing current user information
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user ID
    /// </summary>
    string UserId { get; }
    
    /// <summary>
    /// Checks if the current user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }
} 