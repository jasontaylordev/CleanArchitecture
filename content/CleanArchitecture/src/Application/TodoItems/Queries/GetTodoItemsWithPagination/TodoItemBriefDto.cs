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

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal static partial class TodoItemBriefDtoMapper
{
    private static partial TodoItemBriefDto Map(this TodoItem item);

    public static partial IQueryable<TodoItemBriefDto> ProjectTo(this IQueryable<TodoItem> item);
}
