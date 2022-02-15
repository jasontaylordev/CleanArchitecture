using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}
