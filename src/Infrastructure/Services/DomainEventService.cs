using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Common;
using MediatR;

namespace CleanArchitecture.Infrastructure.Services;

public class DomainEventService : IDomainEventService
{
    private readonly IPublisher _publisher;

    public DomainEventService(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task DispatchEvents(IEnumerable<BaseEntity> entities)
    {
        foreach (var entity in entities)
        {
            var domainEvents = entity.DomainEvents.ToArray();

            entity.DomainEvents.Clear();

            foreach (var domainEvent in domainEvents)
            {
                await _publisher.Publish(domainEvent);
            }
        }
    }
}
