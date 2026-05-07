using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Notifications.Queries.GetNotifications;
using CleanArchitecture.Application.ProjectTodoItems.Queries.GetProjectTodoItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CleanArchitecture.Web.Hubs;

[Authorize]
public class ProjectTodoHub : Hub
{
    public Task JoinProject(int projectId)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, ProjectGroup(projectId));
    }

    public Task LeaveProject(int projectId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, ProjectGroup(projectId));
    }

    public static string ProjectGroup(int projectId) => $"project-{projectId}";
}

public class ProjectTodoRealtimeNotifier : IProjectTodoRealtimeNotifier
{
    private readonly IHubContext<ProjectTodoHub> _hubContext;

    public ProjectTodoRealtimeNotifier(IHubContext<ProjectTodoHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task ProjectTodoItemCreatedAsync(ProjectTodoItemDto item, CancellationToken cancellationToken)
    {
        return _hubContext.Clients.Group(ProjectTodoHub.ProjectGroup(item.ProjectId)).SendAsync("ProjectTodoItemCreated", item, cancellationToken);
    }

    public Task ProjectTodoItemUpdatedAsync(ProjectTodoItemDto item, CancellationToken cancellationToken)
    {
        return _hubContext.Clients.Group(ProjectTodoHub.ProjectGroup(item.ProjectId)).SendAsync("ProjectTodoItemUpdated", item, cancellationToken);
    }

    public Task ProjectTodoItemDeletedAsync(int projectId, int itemId, CancellationToken cancellationToken)
    {
        return _hubContext.Clients.Group(ProjectTodoHub.ProjectGroup(projectId)).SendAsync("ProjectTodoItemDeleted", new { projectId, itemId }, cancellationToken);
    }

    public Task UserNotificationCreatedAsync(NotificationDto notification, CancellationToken cancellationToken)
    {
        return _hubContext.Clients.User(notification.UserId).SendAsync("UserNotificationCreated", notification, cancellationToken);
    }
}
