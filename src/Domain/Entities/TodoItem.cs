namespace CleanArchitecture.Domain.Entities;

public class TodoItem : BaseAuditableEntity
{
    private TodoItem()
    {
            
    }
    public int ListId { get; private set; }

    public string? Title { get; private set; }

    public string? Note { get; private set; }

    public PriorityLevel Priority { get; private set; }

    public DateTime? Reminder { get; private set; }

    private bool _done;
    public bool Done
    {
        get => _done;
        private set
        {
            if (value && !_done)
            {
                AddDomainEvent(new TodoItemCompletedEvent(this));
            }

            _done = value;
        }
    }

    public TodoList List { get; private set; } = null!;

    public TodoItem(int listId, string? title, bool done)
    {
        ListId = listId;
        Title = title;
        Done = done;
    }

    public void Update(int listId, PriorityLevel priority, string? note)
    {
        ListId = listId;
        Priority = priority;
        Note = note;
    }

    public void Update(string? title, bool done)
    {
        Title = title;
        Done = done;
    }
}
