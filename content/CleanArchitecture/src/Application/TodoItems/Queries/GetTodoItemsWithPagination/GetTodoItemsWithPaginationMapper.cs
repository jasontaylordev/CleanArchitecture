using Cubido.Template.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using Cubido.Template.Domain.Entities;

namespace Cubido.Template.Application.Queries.GetTodoItemsWithPagination;

[Mapper]
public static partial class GetTodoItemsWithPaginationMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial TodoItemBriefDto Map(this TodoItem item);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial IQueryable<TodoItemBriefDto> ProjectTo(this IQueryable<TodoItem> item);
}
