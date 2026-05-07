using CleanArchitecture.Application.Notifications.Queries.GetNotifications;
using CleanArchitecture.Application.ProjectTodoItems.Queries.GetProjectTodoItems;

namespace CleanArchitecture.Application.Common.Interfaces;

public interface IProjectTodoRealtimeNotifier
{
    Task ProjectTodoItemCreatedAsync(ProjectTodoItemDto item, CancellationToken cancellationToken);

    Task ProjectTodoItemUpdatedAsync(ProjectTodoItemDto item, CancellationToken cancellationToken);

    Task ProjectTodoItemDeletedAsync(int projectId, int itemId, CancellationToken cancellationToken);

    Task UserNotificationCreatedAsync(NotificationDto notification, CancellationToken cancellationToken);
}
