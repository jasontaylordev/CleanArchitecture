using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Common.EventHandlers;

public class DeletedEventHandler<T> : INotificationHandler<DomainEventNotification<DeletedEvent<T>>> where T : IHasDomainEvent
{
    private readonly ILogger<DeletedEventHandler<T>> _logger;

    public DeletedEventHandler(ILogger<DeletedEventHandler<T>> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<DeletedEvent<T>> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", domainEvent.GetType().Name);

        return Task.CompletedTask;
    }
}
