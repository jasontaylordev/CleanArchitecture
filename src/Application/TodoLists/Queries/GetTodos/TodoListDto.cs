using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.TodoLists.Queries.GetTodos;

public class TodoListDto
{
    public TodoListDto()
    {
        Items = Array.Empty<TodoItemDto>();
    }

    public int Id { get; init; }

    public string? Title { get; init; }

    public string? Colour { get; init; }

    public IReadOnlyCollection<TodoItemDto> Items { get; init; }
}

public static class TodoListDtoMapper
{
    public static TodoListDto FromEntity(TodoList list) => new()
    {
        Id = list.Id,
        Title = list.Title,
        Colour = list.Colour,
        Items = list.Items?
            .Select(TodoItemDtoMapper.FromEntity)
            .ToList()
            ?? new List<TodoItemDto>()
    };
}
