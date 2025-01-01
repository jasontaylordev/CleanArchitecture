
namespace CleanArchitecture.Domain.Entities;

public class TodoList : BaseAuditableEntity
{
    private TodoList()
    {

    }

    public string? Title { get; private set; }

    public Colour Colour { get; private set; } = Colour.White;

    public IList<TodoItem> Items { get; private set; } = new List<TodoItem>();

    public TodoList(string? title)
    {
        Title = title;
    }
    public TodoList(string? title, IList<TodoItem> items)
    {
        Title = title;
        Items = items;
    }
    public TodoList(string? title, Colour colour, IList<TodoItem> items)
    {
        Title = title;
        Colour = colour;
        Items = items;
    }

    public void UpdateTitle(string? title)
    {
        Title = title;
    }
}
