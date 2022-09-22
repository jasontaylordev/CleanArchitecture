using CleanArchitecture.Domain.Common.Events;

namespace CleanArchitecture.Domain.Events;

public class DeletedEvent<T> : BaseEvent<T>
{
    public DeletedEvent(T item) : base(item)
    {
    }

}
