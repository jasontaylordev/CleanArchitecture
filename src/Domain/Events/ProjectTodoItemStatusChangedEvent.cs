namespace CleanArchitecture.Domain.Events;

public class ProjectTodoItemStatusChangedEvent : BaseEvent
{
    public ProjectTodoItemStatusChangedEvent(ProjectTodoItem item)
    {
        Item = item;
    }

    public ProjectTodoItem Item { get; }
}
