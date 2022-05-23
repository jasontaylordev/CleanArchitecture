using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task DispatchEvents(IEnumerable<BaseEntity> entities);
}
