using FluentValidation;
using TaskListService.Application.Services.Commands;

namespace TaskListService.Application.Validators;

public class UpdateTaskListCommandValidator : AbstractValidator<UpdateTaskListCommand>
{
    private const int MinNameLength = 3;
    private const int MaxNameLength = 255;

    public UpdateTaskListCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("The 'Name' field is required and cannot be empty.")
            .NotNull().WithMessage("The 'Name' field cannot be null.")
            .MinimumLength(MinNameLength).WithMessage($"The 'Name' field must be at least {MinNameLength} characters long.")
            .MaximumLength(MaxNameLength).WithMessage($"The 'Name' field must not exceed {MaxNameLength} characters.")
            .Matches("^[a-zA-Z0-9\\s-_]+$").WithMessage("The 'Name' field can only contain letters, numbers, spaces, hyphens, and underscores.")
            .Must(BeValidName).WithMessage("The 'Name' field contains invalid characters or format.");
    }

    private static bool BeValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        
        // Check for leading/trailing spaces
        if (name.Trim() != name) return false;
        
        // Check for multiple consecutive spaces
        if (name.Contains("  ")) return false;
        
        // Check for special characters
        var validChars = new[] { ' ', '-', '_' };
        return name.All(c => char.IsLetterOrDigit(c) || validChars.Contains(c));
    }
}