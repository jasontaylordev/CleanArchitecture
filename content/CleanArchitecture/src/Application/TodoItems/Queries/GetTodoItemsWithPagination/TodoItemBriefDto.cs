using Cubido.Template.Domain.Entities;

namespace Cubido.Template.Application.TodoItems.Queries.GetTodoItemsWithPagination;

public class TodoItemBriefDto
{
    [JsonSqid<TodoItem>]
    public int Id { get; init; }

    public int ListId { get; init; }

    public string? Title { get; init; }

    public bool Done { get; init; }
}
