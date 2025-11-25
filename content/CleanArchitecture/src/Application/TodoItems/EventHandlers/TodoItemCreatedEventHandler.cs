using Cubido.Template.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Cubido.Template.Application.TodoItems.EventHandlers;

public class TodoItemCreatedEventHandler : INotificationHandler<TodoItemCreatedEvent>
{
    private readonly ILogger<TodoItemCreatedEventHandler> logger;

    public TodoItemCreatedEventHandler(ILogger<TodoItemCreatedEventHandler> logger)
    {
        this.logger = logger;
    }

    public ValueTask Handle(TodoItemCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Cubido.Template Domain Event: {DomainEvent}", notification.GetType().Name);

        return default;
    }
}
