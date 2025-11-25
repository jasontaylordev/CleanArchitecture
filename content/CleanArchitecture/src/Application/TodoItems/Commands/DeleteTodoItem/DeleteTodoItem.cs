using Cubido.Template.Application.Common.Interfaces;
using Cubido.Template.Domain.Entities;

namespace Cubido.Template.Application.TodoItems.Commands.DeleteTodoItem;

public record DeleteTodoItemCommand : ICommand
{
    [JsonSqid<TodoItem>]
    public required int Id { get; set; }
}

public class DeleteTodoItemCommandHandler : ICommandHandler<DeleteTodoItemCommand>
{
    private readonly IApplicationDbContext context;

    public DeleteTodoItemCommandHandler(IApplicationDbContext context)
    {
        this.context = context;
    }

    public async ValueTask<Unit> Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.TodoItems
            .FindAsync([request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        context.TodoItems.Remove(entity);

        await context.SaveChangesAsync(cancellationToken);
        return default;
    }

}
