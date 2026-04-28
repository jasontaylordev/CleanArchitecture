using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Application.TodoLists.Queries.GetTodos;

[Authorize]
public sealed class GetTodosQuery : IRequest<TodosVm>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Search { get; init; }
    public string? Colour { get; init; }
    public int? Priority { get; init; }
}

public class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, TodosVm>
{
    private const int MaxPageSize = 50;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTodosQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TodosVm> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = Math.Max(request.PageNumber, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);

        var query = _context.TodoLists.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(list => list.Title != null && list.Title.Contains(request.Search));
        }

        if (!string.IsNullOrWhiteSpace(request.Colour))
        {
            query = query.Where(list => list.Colour.Code == request.Colour);
        }

        if (request.Priority.HasValue)
        {
            query = query.Where(list => list.Items.Any(item => (int)item.Priority == request.Priority.Value));
        }

        var pagedLists = await query
            .OrderBy(list => list.Title)
            .ProjectTo<TodoListDto>(_mapper.ConfigurationProvider)
            .ToPaginatedListAsync(pageNumber, pageSize, cancellationToken);

        return new TodosVm
        {
            PriorityLevels = Enum.GetValues(typeof(PriorityLevel))
                .Cast<PriorityLevel>()
                .Select(p => new LookupDto { Id = (int)p, Title = p.ToString() })
                .ToList(),

            Colours =
            [
                new ColourDto { Code = Colour.Grey, Name = nameof(Colour.Grey) },
                new ColourDto { Code = Colour.Purple, Name = nameof(Colour.Purple) },
                new ColourDto { Code = Colour.Blue, Name = nameof(Colour.Blue) },
                new ColourDto { Code = Colour.Teal, Name = nameof(Colour.Teal) },
                new ColourDto { Code = Colour.Green, Name = nameof(Colour.Green) },
                new ColourDto { Code = Colour.Orange, Name = nameof(Colour.Orange) },
                new ColourDto { Code = Colour.Red, Name = nameof(Colour.Red) },
            ],

            Lists = pagedLists.Items,
            Pagination = new PaginationDto
            {
                PageNumber = pagedLists.PageIndex,
                PageSize = pagedLists.PageSize,
                TotalCount = pagedLists.TotalCount,
                TotalPages = pagedLists.TotalPages,
                HasPreviousPage = pagedLists.HasPreviousPage,
                HasNextPage = pagedLists.HasNextPage
            }
        };
    }
}
