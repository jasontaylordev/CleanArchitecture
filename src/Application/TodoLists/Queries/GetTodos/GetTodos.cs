using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Application.Common.Specifications;
using CleanArchitecture.Application.TodoLists.Specifications;
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
    private readonly ICacheService _cacheService;

    public GetTodosQueryHandler(IApplicationDbContext context, IMapper mapper, ICacheService cacheService)
    {
        _context = context;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<TodosVm> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = Math.Max(request.PageNumber, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);
        var search = request.Search?.Trim() ?? string.Empty;
        var colour = request.Colour?.Trim() ?? string.Empty;

        var cacheKey = $"GetTodos:{pageNumber}:{pageSize}:{search}:{colour}:{request.Priority}";

        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            var specification = new TodoListFilterSpecification(search, colour, request.Priority);
            
            var countSpecification = new TodoListFilterSpecification(search, colour, request.Priority);
            var totalQuery = SpecificationEvaluator<Domain.Entities.TodoList>.GetQuery(_context.TodoLists.AsNoTracking(), countSpecification);
            var totalCount = await totalQuery.CountAsync(cancellationToken);

            var skip = (pageNumber - 1) * pageSize;
            specification.ApplyPaging(skip, pageSize);

            var query = SpecificationEvaluator<Domain.Entities.TodoList>.GetQuery(_context.TodoLists.AsNoTracking(), specification);
            var pagedLists = await query
                .ProjectTo<TodoListDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

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

                Lists = pagedLists,
                Pagination = new PaginationDto
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    HasPreviousPage = pageNumber > 1,
                    HasNextPage = pageNumber < totalPages
                }
            };
        }, TimeSpan.FromMinutes(1));
    }
}
