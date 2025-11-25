using Cubido.Template.Application.Common.Interfaces;
using Cubido.Template.Domain.Entities;
using Cubido.Template.Domain.Events;

namespace Cubido.Template.Application.TodoItems.Commands.CreateTodoItem;

public record CreateTodoItemCommand : ICommand<int>
{
    public int ListId { get; init; }

    public string? Title { get; init; }
}

public class CreateTodoItemCommandHandler : ICommandHandler<CreateTodoItemCommand, int>
{
    private readonly IApplicationDbContext context;

    public CreateTodoItemCommandHandler(IApplicationDbContext context)
    {
        this.context = context;
    }

    public async ValueTask<int> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoItem
        {
            ListId = request.ListId,
            Title = request.Title,
            Done = false
        };

        entity.AddDomainEvent(new TodoItemCreatedEvent(entity));

        context.TodoItems.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
