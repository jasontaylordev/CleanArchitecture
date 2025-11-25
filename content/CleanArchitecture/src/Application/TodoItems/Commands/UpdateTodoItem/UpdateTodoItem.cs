using Cubido.Template.Application.Common.Interfaces;
using Cubido.Template.Domain.Entities;

namespace Cubido.Template.Application.TodoItems.Commands.UpdateTodoItem;

public record UpdateTodoItemCommand : ICommand
{
    [JsonSqid<TodoItem>]
    public required int Id { get; set; }

    public string? Title { get; init; }

    public bool Done { get; init; }
}

public class UpdateTodoItemCommandHandler : ICommandHandler<UpdateTodoItemCommand>
{
    private readonly IApplicationDbContext context;

    public UpdateTodoItemCommandHandler(IApplicationDbContext context)
    {
        this.context = context;
    }

    public async ValueTask<Unit> Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.TodoItems
            .FindAsync([request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        entity.Title = request.Title;
        entity.Done = request.Done;

        await context.SaveChangesAsync(cancellationToken);
        return default;
    }
}
