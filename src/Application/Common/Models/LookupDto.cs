using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Common.Models;

public class LookupDto
{
    public int Id { get; init; }

    public string? Title { get; init; }
}

public static class LookupDtoMapper
{
    public static LookupDto FromTodoList(TodoList list) => new()
    {
        Id = list.Id,
        Title = list.Title
    };

    public static LookupDto FromTodoItem(TodoItem item) => new()
    {
        Id = item.Id,
        Title = item.Title
    };
}
