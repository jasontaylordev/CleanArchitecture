using AutoMapper;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Queries;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsWithPagination;

public class GetTodoItemsWithPaginationQuery : PageEntityBaseQuery<TodoItemBriefDto>
{
    public int ListId { get; set; }

}

public class GetTodoItemsWithPaginationQueryHandler : PageEntityBaseQueryHandler<GetTodoItemsWithPaginationQuery, TodoItem, TodoItemBriefDto>
{

    public GetTodoItemsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    protected override IQueryable<TodoItem> Query(GetTodoItemsWithPaginationQuery request)
    {
        return Entity.Where(x => x.ListId == request.ListId)
        .OrderBy(x => x.Title);
    }
}
