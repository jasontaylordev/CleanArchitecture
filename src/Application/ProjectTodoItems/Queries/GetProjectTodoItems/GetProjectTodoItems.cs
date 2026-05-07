using System.Globalization;
using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.ProjectTodoItems.Queries.GetProjectTodoItems;

public record ProjectTodoItemDto
{
    public int Id { get; init; }

    public int ProjectId { get; init; }

    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public string? DueDate { get; init; }

    public string? AssigneeUserId { get; init; }

    public string? AssigneeUserName { get; init; }

    public string ReporterUserId { get; init; } = string.Empty;

    public string? ReporterUserName { get; init; }

    public int StatusId { get; init; }

    public string StatusName { get; init; } = string.Empty;
}

public record GetProjectTodoItemsQuery(int ProjectId) : IRequest<IReadOnlyCollection<ProjectTodoItemDto>>;

public class GetProjectTodoItemsQueryHandler : IRequestHandler<GetProjectTodoItemsQuery, IReadOnlyCollection<ProjectTodoItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public GetProjectTodoItemsQueryHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<IReadOnlyCollection<ProjectTodoItemDto>> Handle(GetProjectTodoItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await _context.ProjectTodoItems
            .AsNoTracking()
            .Include(i => i.Status)
            .Where(i => i.ProjectId == request.ProjectId)
            .OrderBy(i => i.DueDate)
            .ThenBy(i => i.Title)
            .ToListAsync(cancellationToken);

        return await ProjectTodoItemDtoFactory.CreateAsync(items, _identityService);
    }
}

public static class ProjectTodoItemDtoFactory
{
    public static async Task<IReadOnlyCollection<ProjectTodoItemDto>> CreateAsync(IEnumerable<Domain.Entities.ProjectTodoItem> items, IIdentityService identityService)
    {
        var result = new List<ProjectTodoItemDto>();

        foreach (var item in items)
        {
            result.Add(await CreateAsync(item, identityService));
        }

        return result;
    }

    public static async Task<ProjectTodoItemDto> CreateAsync(Domain.Entities.ProjectTodoItem item, IIdentityService identityService)
    {
        return new ProjectTodoItemDto
        {
            Id = item.Id,
            ProjectId = item.ProjectId,
            Title = item.Title,
            Description = item.Description,
            DueDate = item.DueDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            AssigneeUserId = item.AssigneeUserId,
            AssigneeUserName = string.IsNullOrWhiteSpace(item.AssigneeUserId) ? null : await identityService.GetUserNameAsync(item.AssigneeUserId),
            ReporterUserId = item.ReporterUserId,
            ReporterUserName = await identityService.GetUserNameAsync(item.ReporterUserId),
            StatusId = item.StatusId,
            StatusName = item.Status.Name
        };
    }
}
