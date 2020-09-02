namespace CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsWithPagination
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using MediatR;

    using Common.Interfaces;
    using Common.Mappings;
    using Common.Models;
    using TodoLists.Queries.GetTodos;

    public class GetTodoItemsWithPaginationQuery : PaginationQuery, IRequest<PaginationResponse<TodoItemDto>>
    {
        public int ListId { get; set; }
    }

    public class GetTodoItemsWithPaginationQueryHandler : IRequestHandler<GetTodoItemsWithPaginationQuery, PaginationResponse<TodoItemDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetTodoItemsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaginationResponse<TodoItemDto>> Handle(GetTodoItemsWithPaginationQuery request, CancellationToken cancellationToken)
        {
            PaginatedList<TodoItemDto> list = await _context.TodoItems
                .Where(x => x.ListId == request.ListId)
                .OrderBy(x => x.Title)
                .ProjectTo<TodoItemDto>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize);


            return new PaginationResponse<TodoItemDto>
            {
                Items = list,
                PageIndex = list.PageIndex,
                TotalPages = list.TotalPages,
                TotalCount = list.TotalCount,
            };
        }
    }
}
