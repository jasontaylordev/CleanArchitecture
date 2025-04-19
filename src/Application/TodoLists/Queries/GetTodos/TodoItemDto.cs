using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.TodoLists.Queries.GetTodos;

public class TodoItemDto
{
    public int Id { get; init; }

    public int ListId { get; init; }

    public string? Title { get; init; }

    public bool Done { get; init; }

    public int Priority { get; init; }

    public string? Note { get; init; }
}


public static class TodoItemDtoMapper
{
    public static TodoItemDto FromEntity(TodoItem item) => new()
    {
        Id = item.Id,
        ListId = item.ListId,
        Title = item.Title,
        Done = item.Done,
        Priority = (int)item.Priority,
        Note = item.Note
    };
}
