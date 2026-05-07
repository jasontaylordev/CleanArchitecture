using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.ProjectTodoItems.Commands.DeleteProjectTodoItem;

public record DeleteProjectTodoItemCommand(int ProjectId, int Id) : IRequest;

public class DeleteProjectTodoItemCommandHandler : IRequestHandler<DeleteProjectTodoItemCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IProjectTodoRealtimeNotifier _notifier;

    public DeleteProjectTodoItemCommandHandler(IApplicationDbContext context, IProjectTodoRealtimeNotifier notifier)
    {
        _context = context;
        _notifier = notifier;
    }

    public async Task Handle(DeleteProjectTodoItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.ProjectTodoItems
            .FirstOrDefaultAsync(i => i.Id == request.Id && i.ProjectId == request.ProjectId, cancellationToken);

        Guard.Against.NotFound(request.Id, item);

        _context.ProjectTodoItems.Remove(item);

        await _context.SaveChangesAsync(cancellationToken);

        await _notifier.ProjectTodoItemDeletedAsync(request.ProjectId, request.Id, cancellationToken);
    }
}
