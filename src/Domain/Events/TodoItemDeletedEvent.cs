namespace CleanArchitecture.Domain.Events;

public record TodoItemDeletedEvent(TodoItem Item) : BaseEvent;
