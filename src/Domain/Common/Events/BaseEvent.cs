using MediatR;

namespace CleanArchitecture.Domain.Common.Events;

public class BaseEvent<T> : BaseEvent
{
    public T Item { get; }
    public BaseEvent(T item)
    {
        Item = item;
    }
}

public class BaseEvent : INotification
{
}
