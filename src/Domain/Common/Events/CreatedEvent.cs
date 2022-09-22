using CleanArchitecture.Domain.Common.Events;

namespace CleanArchitecture.Domain.Events;

public class CreatedEvent<T> : BaseEvent<T>
{
    public CreatedEvent(T item) : base(item)
    {
    }

}
