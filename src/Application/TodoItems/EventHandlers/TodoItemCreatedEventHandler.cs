using CleanArchitecture.Domain.Events;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.TodoItems.EventHandlers;

public class TodoItemCreatedEventHandler(ILogger<TodoItemCreatedEventHandler> logger) : INotificationHandler<TodoItemCreatedEvent>
{
    private readonly ILogger<TodoItemCreatedEventHandler> _logger = logger;

    public Task Handle(TodoItemCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
