using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Events;

namespace CleanArchitecture.Application.ProjectTodoItems.EventHandlers;

public class QueueReporterEmailOnStatusChanged : INotificationHandler<ProjectTodoItemStatusChangedEvent>
{
    private readonly IEmailService _emailService;

    public QueueReporterEmailOnStatusChanged(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Handle(ProjectTodoItemStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        var item = notification.Item;
        var statusName = item.Status?.Name ?? "the selected status";

        await _emailService.SendAsync(
            item.ReporterUserId,
            $"Project to-do status changed: {item.Title}",
            $"The status for '{item.Title}' changed to '{statusName}'. Open /projects/{item.ProjectId}/todos/{item.Id} to review it.",
            cancellationToken);
    }
}
