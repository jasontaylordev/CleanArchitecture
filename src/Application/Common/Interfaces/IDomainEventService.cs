using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task DispatchEvents(IEnumerable<BaseEntity> entities);
}
