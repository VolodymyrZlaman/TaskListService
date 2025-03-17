using FluentValidation;
using TaskListService.Application.Services.Commands;

namespace TaskListService.Application.Validators;

public class CreateTaskListCommandValidator : AbstractValidator<CreateTaskListCommand>
{
    public CreateTaskListCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("The 'Name' field is required and cannot be empty.")
            .NotNull().WithMessage("The 'Name' field cannot be null.")
            .MaximumLength(255).WithMessage("The 'Name' field must be between 1 and 255 characters long.");

        RuleFor(c => c.OwnerId)
            .NotEmpty().WithMessage("The 'OwnerId' field is required and cannot be empty.")
            .NotNull().WithMessage("The 'OwnerId' field cannot be null.");

        RuleFor(c => c.TaskListId)
            .NotEmpty().WithMessage("The 'TaskListId' field is required and cannot be empty.")
            .NotNull().WithMessage("The 'TaskListId' field cannot be null.");
    }

}