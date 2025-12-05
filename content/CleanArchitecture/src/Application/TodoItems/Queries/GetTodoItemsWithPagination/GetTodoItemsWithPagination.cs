using Cubido.Template.Application.Common.Interfaces;
using Cubido.Template.Application.Common.Mappings;
using Cubido.Template.Application.Common.Models;

namespace Cubido.Template.Application.TodoItems.Queries.GetTodoItemsWithPagination;

public record GetTodoItemsWithPaginationQuery : IRequest<PaginatedList<TodoItemBriefDto>>
{
    public int ListId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetTodoItemsWithPaginationQueryHandler : IRequestHandler<GetTodoItemsWithPaginationQuery, PaginatedList<TodoItemBriefDto>>
{
    private readonly IApplicationDbContext context;

    public GetTodoItemsWithPaginationQueryHandler(IApplicationDbContext context)
    {
        this.context = context;
    }

    public async ValueTask<PaginatedList<TodoItemBriefDto>> Handle(GetTodoItemsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return await context.TodoItems
            .Where(x => x.ListId == request.ListId)
            .OrderBy(x => x.Title)
            .ProjectTo()
            .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
