using CleanArchitecture.Domain.Common.Events;

namespace CleanArchitecture.Domain.Events;

public class CompletedEvent<T> : BaseEvent<T>
{
    public CompletedEvent(T item) : base(item)
    {
    }
}
