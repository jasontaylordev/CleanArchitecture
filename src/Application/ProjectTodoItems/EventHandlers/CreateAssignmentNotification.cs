using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Events;

namespace CleanArchitecture.Application.ProjectTodoItems.EventHandlers;

public class CreateAssignmentNotification : INotificationHandler<ProjectTodoItemAssignedEvent>
{
    private readonly IApplicationDbContext _context;

    public CreateAssignmentNotification(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task Handle(ProjectTodoItemAssignedEvent notification, CancellationToken cancellationToken)
    {
        var item = notification.Item;

        if (string.IsNullOrWhiteSpace(item.AssigneeUserId))
        {
            return Task.CompletedTask;
        }

        _context.UserNotifications.Add(new UserNotification
        {
            UserId = item.AssigneeUserId,
            Title = "Project to-do assigned",
            Message = $"You have been assigned to '{item.Title}'.",
            LinkUrl = $"/projects/{item.ProjectId}/todos/{item.Id}"
        });

        return Task.CompletedTask;
    }
}
