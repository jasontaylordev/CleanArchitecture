namespace CleanArchitecture.Domain.Events;

public class TodoItemDeletedEvent(TodoItem item) : BaseEvent
{
    public TodoItem Item { get; } = item;
}
