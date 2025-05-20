using FluentValidation;
using TaskListService.Application.Services.Queries;

namespace TaskListService.Application.Validators;

public class GetTaskListsForUserQueryValidator : AbstractValidator<GetTaskListsForUserQuery>
{
    private const int MinPageNumber = 1;
    private const int MaxPageNumber = 1000;
    private const int MinPageSize = 1;
    private const int MaxPageSize = 100;

    public GetTaskListsForUserQueryValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("The 'UserId' field is required and cannot be empty.")
            .NotNull().WithMessage("The 'UserId' field cannot be null.")
            .MaximumLength(255).WithMessage("The 'UserId' field must not exceed 255 characters.")
            .MinimumLength(3).WithMessage("The 'UserId' field must be at least 3 characters long.")
            .Matches("^[a-zA-Z0-9-_]+$").WithMessage("The 'UserId' field can only contain letters, numbers, hyphens, and underscores.")
            .Must(BeValidUserId).WithMessage("The 'UserId' field must be a valid identifier.");

        RuleFor(c => c.Page)
            .NotEmpty().WithMessage("The 'Page' field is required.")
            .NotNull().WithMessage("The 'Page' field cannot be null.")
            .GreaterThanOrEqualTo(MinPageNumber)
                .WithMessage($"The 'Page' field must be greater than or equal to {MinPageNumber}.")
            .LessThanOrEqualTo(MaxPageNumber)
                .WithMessage($"The 'Page' field must be less than or equal to {MaxPageNumber}.")
            .Must(BeValidPageNumber)
                .WithMessage("The 'Page' field must be a valid positive integer.");

        RuleFor(c => c.PageSize)
            .NotEmpty().WithMessage("The 'PageSize' field is required.")
            .NotNull().WithMessage("The 'PageSize' field cannot be null.")
            .GreaterThanOrEqualTo(MinPageSize)
                .WithMessage($"The 'PageSize' field must be greater than or equal to {MinPageSize}.")
            .LessThanOrEqualTo(MaxPageSize)
                .WithMessage($"The 'PageSize' field must be less than or equal to {MaxPageSize}.")
            .Must(BeValidPageSize)
                .WithMessage($"The 'PageSize' field must be a valid positive integer not exceeding {MaxPageSize}.");

        // Composite rules
        RuleFor(q => q)
            .Must(q => IsValidPagination(q.Page, q.PageSize))
            .WithMessage($"The total number of items requested (Page * PageSize) cannot exceed {MaxPageNumber * MaxPageSize}.");
    }

    private static bool BeValidUserId(string userId)
    {
        // Add additional user ID validation logic if needed
        return !string.IsNullOrWhiteSpace(userId) && 
               userId.Length >= 3 && 
               userId.Length <= 255;
    }

    private static bool BeValidPageNumber(int page)
    {
        return page >= MinPageNumber && page <= MaxPageNumber;
    }

    private static bool BeValidPageSize(int pageSize)
    {
        return pageSize >= MinPageSize && pageSize <= MaxPageSize;
    }

    private static bool IsValidPagination(int page, int pageSize)
    {
        // Prevent potential integer overflow and excessive resource usage
        try
        {
            var totalItems = checked(page * pageSize);
            return totalItems <= MaxPageNumber * MaxPageSize;
        }
        catch (OverflowException)
        {
            return false;
        }
    }
}