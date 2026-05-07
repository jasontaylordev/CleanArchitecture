using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.ProjectTodoItems.Queries.GetProjectTodoItems;

namespace CleanArchitecture.Application.ProjectTodoItems.Queries.GetProjectKanbanBoard;

public record ProjectKanbanColumnDto
{
    public int StatusId { get; init; }

    public string StatusName { get; init; } = string.Empty;

    public int SortOrder { get; init; }

    public IReadOnlyCollection<ProjectTodoItemDto> Items { get; init; } = Array.Empty<ProjectTodoItemDto>();
}

public record ProjectKanbanBoardVm
{
    public int ProjectId { get; init; }

    public IReadOnlyCollection<ProjectKanbanColumnDto> Columns { get; init; } = Array.Empty<ProjectKanbanColumnDto>();
}

public record GetProjectKanbanBoardQuery(int ProjectId) : IRequest<ProjectKanbanBoardVm>;

public class GetProjectKanbanBoardQueryHandler : IRequestHandler<GetProjectKanbanBoardQuery, ProjectKanbanBoardVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public GetProjectKanbanBoardQueryHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<ProjectKanbanBoardVm> Handle(GetProjectKanbanBoardQuery request, CancellationToken cancellationToken)
    {
        var statuses = await _context.ProjectTodoStatuses
            .AsNoTracking()
            .Where(s => s.ProjectId == request.ProjectId)
            .OrderBy(s => s.SortOrder)
            .ToListAsync(cancellationToken);

        var items = await _context.ProjectTodoItems
            .AsNoTracking()
            .Include(i => i.Status)
            .Where(i => i.ProjectId == request.ProjectId)
            .OrderBy(i => i.DueDate)
            .ThenBy(i => i.Title)
            .ToListAsync(cancellationToken);

        var itemDtos = await ProjectTodoItemDtoFactory.CreateAsync(items, _identityService);

        return new ProjectKanbanBoardVm
        {
            ProjectId = request.ProjectId,
            Columns = statuses.Select(status => new ProjectKanbanColumnDto
            {
                StatusId = status.Id,
                StatusName = status.Name,
                SortOrder = status.SortOrder,
                Items = itemDtos.Where(i => i.StatusId == status.Id).ToList()
            }).ToList()
        };
    }
}
