namespace TaskService.Domain.Common;

/// <summary>
/// Class for handling operation results without value
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates if operation was successful
    /// </summary>
    protected bool IsSuccess { get; }

    /// <summary>
    /// Indicates if operation failed
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Error message
    /// </summary>
    public string? Error { get; } 

    protected Result(bool isSuccess, string? error = null)
    {
        switch (isSuccess)
        {
            case true when !string.IsNullOrEmpty(error):
                throw new InvalidOperationException("Success result cannot contain an error");
            case false when string.IsNullOrEmpty(error):
                throw new InvalidOperationException("Failure result must contain an error");
            default:
                IsSuccess = isSuccess;
                Error = error;
                break;
        }
    }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    public static Result Success() => new(true);

    /// <summary>
    /// Creates a failure result with error message
    /// </summary>
    public static Result Failure(string? error) => new(false, error);

    /// <summary>
    /// Creates a failure result from exception
    /// </summary>
    public static Result Failure(Exception exception) => 
        new(false, exception.Message);

    /// <summary>
    /// Creates a successful result with value
    /// </summary>
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    /// <summary>
    /// Creates a failure result with value and error message
    /// </summary>
    public static Result<T> Failure<T>(string error) => Result<T>.Failure(error);
    
    /// <summary>
    /// Creates a failure result with value from exception
    /// </summary>
    public static Result<T> Failure<T>(Exception exception) => 
        Result<T>.Failure(exception.Message);
    
}

/// <summary>
/// Class for handling operation results with value
/// </summary>
/// <typeparam name="T">Value type</typeparam>
public class Result<T>(T? value, bool isSuccess, string? error) : Result(isSuccess, error)
{
    /// <summary>
    /// Result value
    /// </summary>
    public T Value
    {
        get
        {
            if (!IsSuccess)
                throw new InvalidOperationException("Cannot access value of a failed result");

            return value;
        }
    }

    /// <summary>
    /// Creates a successful result with value
    /// </summary>
    public static Result<T> Success(T value) => new(value, true, null);

    /// <summary>
    /// Creates a failure result with error message
    /// </summary>
    public new static Result<T> Failure(string error) => new(default, false, error);
    
    /// <summary>
    /// Creates a failure result from exception
    /// </summary>
    public new static Result<T> Failure(Exception exception) => 
        new(default, false, exception.Message);
    
} 