using CleanArchitecture.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Common.Behaviours;

public class DomainEventBehavior : INotificationHandler<BaseEvent>
{
    private readonly ILogger<DomainEventBehavior> _logger;

    public DomainEventBehavior(ILogger<DomainEventBehavior> logger)
    {
        _logger = logger;
    }

    public Task Handle(BaseEvent notification, CancellationToken cancellationToken)
    {
        _logger.Log(notification.Level, notification.LogMessage);
        return Task.CompletedTask;
    }
}