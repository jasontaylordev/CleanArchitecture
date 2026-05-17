namespace CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;

public class CreateTodoItemCommandValidator : AbstractValidator<CreateTodoItemCommand>
{
    public CreateTodoItemCommandValidator()
    {
        RuleFor(v => v.Title)
            .MaximumLength(200)
            .NotEmpty();

        RuleFor(v => v.DueDate)
            .Must(date => date == null || date > DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Due date must be a future date.");
    }
}
