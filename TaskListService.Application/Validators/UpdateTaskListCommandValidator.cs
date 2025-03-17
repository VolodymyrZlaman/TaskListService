using FluentValidation;
using TaskListService.Application.Services.Commands;

namespace TaskListService.Application.Validators;

public class UpdateTaskListCommandValidator : AbstractValidator<UpdateTaskListCommand>
{
    public UpdateTaskListCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("The 'Name' field is required and cannot be empty.")
            .NotNull().WithMessage("The 'Name' field cannot be null.")
            .MaximumLength(255).WithMessage("The 'Name' field must be between 1 and 255 characters long.");
    }
}