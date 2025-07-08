namespace CleanArchitecture.Domain.Events;

public record TodoItemCreatedEvent(TodoItem Item) : BaseEvent;
