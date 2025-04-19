using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsWithPagination;

public class TodoItemBriefDto
{
    public int Id { get; init; }

    public int ListId { get; init; }

    public string? Title { get; init; }

    public bool Done { get; init; }
}

public static class TodoItemBriefDtoMapper
{
    public static TodoItemBriefDto FromEntity(TodoItem item) => new()
    {
        Id = item.Id,
        ListId = item.ListId,
        Title = item.Title,
        Done = item.Done
    };
}
