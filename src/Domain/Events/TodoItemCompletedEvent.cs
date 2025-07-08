namespace CleanArchitecture.Domain.Events;

public record TodoItemCompletedEvent(TodoItem Item) : BaseEvent;
