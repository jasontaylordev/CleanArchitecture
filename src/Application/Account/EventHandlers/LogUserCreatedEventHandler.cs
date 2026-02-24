using CleanArchitecture.Domain.Events;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Account.EventHandlers;

internal sealed class LogUserCreatedEventHandler(ILogger<LogUserCreatedEventHandler> logger) : INotificationHandler<UserCreatedEvent>
{
    public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("{userName} is created", notification.DisplayName);
        return Task.CompletedTask;
    }
}
