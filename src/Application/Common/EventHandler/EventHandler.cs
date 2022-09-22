using CleanArchitecture.Domain.Common.Events;
using CleanArchitecture.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.EventHandlers;

public class EventHandler<T> : INotificationHandler<BaseEvent<T>>
{
    private readonly ILogger<EventHandler<T>> _logger;

    public EventHandler(ILogger<EventHandler<T>> logger)
    {
        _logger = logger;
    }

    public Task Handle(BaseEvent<T> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", notification.GetType().Name);
        return Task.CompletedTask;
    }
}
