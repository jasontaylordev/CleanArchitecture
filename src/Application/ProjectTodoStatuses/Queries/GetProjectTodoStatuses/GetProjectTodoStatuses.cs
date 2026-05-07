using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.ProjectTodoStatuses.Queries.GetProjectTodoStatuses;

public record ProjectTodoStatusDto
{
    public int Id { get; init; }

    public int ProjectId { get; init; }

    public string Name { get; init; } = string.Empty;

    public int SortOrder { get; init; }

    public bool IsDefault { get; init; }

    public bool IsTerminal { get; init; }
}

public record GetProjectTodoStatusesQuery(int ProjectId) : IRequest<IReadOnlyCollection<ProjectTodoStatusDto>>;

public class GetProjectTodoStatusesQueryHandler : IRequestHandler<GetProjectTodoStatusesQuery, IReadOnlyCollection<ProjectTodoStatusDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProjectTodoStatusesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<ProjectTodoStatusDto>> Handle(GetProjectTodoStatusesQuery request, CancellationToken cancellationToken)
    {
        return await _context.ProjectTodoStatuses
            .AsNoTracking()
            .Where(s => s.ProjectId == request.ProjectId)
            .OrderBy(s => s.SortOrder)
            .Select(s => new ProjectTodoStatusDto
            {
                Id = s.Id,
                ProjectId = s.ProjectId,
                Name = s.Name,
                SortOrder = s.SortOrder,
                IsDefault = s.IsDefault,
                IsTerminal = s.IsTerminal
            })
            .ToListAsync(cancellationToken);
    }
}
