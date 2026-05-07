using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.ProjectTodoItems.Queries.GetProjectTodoItems;

namespace CleanArchitecture.Application.ProjectTodoItems.Commands.ChangeProjectTodoItemStatus;

public record ChangeProjectTodoItemStatusCommand : IRequest
{
    public int Id { get; init; }

    public int ProjectId { get; init; }

    public int StatusId { get; init; }
}

public class ChangeProjectTodoItemStatusCommandValidator : AbstractValidator<ChangeProjectTodoItemStatusCommand>
{
    public ChangeProjectTodoItemStatusCommandValidator()
    {
        RuleFor(v => v.Id).GreaterThan(0);
        RuleFor(v => v.ProjectId).GreaterThan(0);
        RuleFor(v => v.StatusId).GreaterThan(0);
    }
}

public class ChangeProjectTodoItemStatusCommandHandler : IRequestHandler<ChangeProjectTodoItemStatusCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IProjectTodoRealtimeNotifier _notifier;

    public ChangeProjectTodoItemStatusCommandHandler(IApplicationDbContext context, IIdentityService identityService, IProjectTodoRealtimeNotifier notifier)
    {
        _context = context;
        _identityService = identityService;
        _notifier = notifier;
    }

    public async Task Handle(ChangeProjectTodoItemStatusCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.ProjectTodoItems
            .Include(i => i.Status)
            .FirstOrDefaultAsync(i => i.Id == request.Id && i.ProjectId == request.ProjectId, cancellationToken);

        Guard.Against.NotFound(request.Id, item);

        var newStatus = await _context.ProjectTodoStatuses
            .FirstOrDefaultAsync(s => s.Id == request.StatusId && s.ProjectId == request.ProjectId, cancellationToken);

        Guard.Against.NotFound(request.StatusId, newStatus);

        var statusChanged = item.ChangeStatus(request.StatusId);

        if (!statusChanged)
        {
            return;
        }

        item.Status = newStatus;

        await _context.SaveChangesAsync(cancellationToken);

        await _notifier.ProjectTodoItemUpdatedAsync(await ProjectTodoItemDtoFactory.CreateAsync(item, _identityService), cancellationToken);
    }
}
