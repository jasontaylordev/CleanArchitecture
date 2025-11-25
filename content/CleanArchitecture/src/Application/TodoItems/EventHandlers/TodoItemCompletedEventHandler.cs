using Cubido.Template.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Cubido.Template.Application.TodoItems.EventHandlers;

public class TodoItemCompletedEventHandler : INotificationHandler<TodoItemCompletedEvent>
{
    private readonly ILogger<TodoItemCompletedEventHandler> logger;

    public TodoItemCompletedEventHandler(ILogger<TodoItemCompletedEventHandler> logger)
    {
        this.logger = logger;
    }

    public ValueTask Handle(TodoItemCompletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Cubido.Template Domain Event: {DomainEvent}", notification.GetType().Name);
        return ValueTask.CompletedTask;
    }
}
