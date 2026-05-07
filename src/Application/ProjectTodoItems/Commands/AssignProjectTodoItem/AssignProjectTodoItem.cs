using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Notifications.Queries.GetNotifications;
using CleanArchitecture.Application.ProjectTodoItems.Queries.GetProjectTodoItems;

namespace CleanArchitecture.Application.ProjectTodoItems.Commands.AssignProjectTodoItem;

public record AssignProjectTodoItemCommand : IRequest
{
    public int Id { get; init; }

    public int ProjectId { get; init; }

    public string? AssigneeUserId { get; init; }
}

public class AssignProjectTodoItemCommandValidator : AbstractValidator<AssignProjectTodoItemCommand>
{
    public AssignProjectTodoItemCommandValidator()
    {
        RuleFor(v => v.Id).GreaterThan(0);
        RuleFor(v => v.ProjectId).GreaterThan(0);
    }
}

public class AssignProjectTodoItemCommandHandler : IRequestHandler<AssignProjectTodoItemCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IProjectTodoRealtimeNotifier _notifier;
    private readonly IEmailService _emailService;

    public AssignProjectTodoItemCommandHandler(IApplicationDbContext context, IIdentityService identityService, IProjectTodoRealtimeNotifier notifier, IEmailService emailService)
    {
        _context = context;
        _identityService = identityService;
        _notifier = notifier;
        _emailService = emailService;
    }

    public async Task Handle(AssignProjectTodoItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.ProjectTodoItems
            .Include(i => i.Status)
            .FirstOrDefaultAsync(i => i.Id == request.Id && i.ProjectId == request.ProjectId, cancellationToken);

        Guard.Against.NotFound(request.Id, item);

        var previousAssigneeUserId = item.AssigneeUserId;
        var assignmentChanged = item.AssignTo(request.AssigneeUserId);

        if (!assignmentChanged)
        {
            return;
        }

        await _context.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(previousAssigneeUserId))
        {
            await _emailService.SendAsync(
                previousAssigneeUserId,
                $"Project to-do unassigned: {item.Title}",
                $"You are no longer assigned to '{item.Title}'. Open /projects/{item.ProjectId}/todos/{item.Id} to review it.",
                cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(request.AssigneeUserId))
        {
            await _emailService.SendAsync(
                request.AssigneeUserId,
                $"Project to-do assigned: {item.Title}",
                $"You have been assigned to '{item.Title}'. Open /projects/{item.ProjectId}/todos/{item.Id} to review it.",
                cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _notifier.ProjectTodoItemUpdatedAsync(await ProjectTodoItemDtoFactory.CreateAsync(item, _identityService), cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.AssigneeUserId))
        {
            var linkUrl = $"/projects/{item.ProjectId}/todos/{item.Id}";
            var notification = await _context.UserNotifications
                .AsNoTracking()
                .Where(n => n.UserId == request.AssigneeUserId && n.LinkUrl == linkUrl)
                .OrderByDescending(n => n.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (notification is not null)
            {
                await _notifier.UserNotificationCreatedAsync(new NotificationDto
                {
                    Id = notification.Id,
                    UserId = notification.UserId,
                    Title = notification.Title,
                    Message = notification.Message,
                    LinkUrl = notification.LinkUrl,
                    IsRead = notification.IsRead,
                    Created = notification.Created
                }, cancellationToken);
            }
        }
    }
}
