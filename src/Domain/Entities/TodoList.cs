

namespace CleanArchitecture.Domain.Entities;

public class TodoList : BaseAuditableEntity
{
    [MaxLength(200)]
    public string Title { get; set; } = default!;

    public Colour Colour { get; set; } = Colour.White;

    public IList<TodoItem> Items { get; private set; } = new List<TodoItem>();
}
