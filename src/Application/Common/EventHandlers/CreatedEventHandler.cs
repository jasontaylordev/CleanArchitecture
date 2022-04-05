using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Common.EventHandlers;

public class CreatedEventHandler<T> : INotificationHandler<DomainEventNotification<CreatedEvent<T>>> where T : IHasDomainEvent
{
    private readonly ILogger<CreatedEventHandler<T>> _logger;

    public CreatedEventHandler(ILogger<CreatedEventHandler<T>> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<CreatedEvent<T>> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", domainEvent.GetType().Name);

        return Task.CompletedTask;
    }
}
