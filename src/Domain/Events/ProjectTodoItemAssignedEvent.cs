namespace CleanArchitecture.Domain.Events;

public class ProjectTodoItemAssignedEvent : BaseEvent
{
    public ProjectTodoItemAssignedEvent(ProjectTodoItem item)
    {
        Item = item;
    }

    public ProjectTodoItem Item { get; }
}
