namespace Cubido.Template.Domain.Entities;

public class TodoItem : BaseAuditableEntity
{
    public int ListId { get; set; }

    public string? Title { get; set; }

    public string? Note { get; set; }

    public PriorityLevel Priority { get; set; }

    public DateTime? Reminder { get; set; }

    public bool Done
    {
        get;
        set
        {
            if (value && !field)
            {
                AddDomainEvent(new TodoItemCompletedEvent(this));
            }

            field = value;
        }
    }

    public TodoList List { get; set; } = null!;
}
