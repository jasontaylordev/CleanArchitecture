using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.Users.Queries.GetAssignableUsers;

public record AssignableUserDto
{
    public string Id { get; init; } = string.Empty;

    public string UserName { get; init; } = string.Empty;
}

public record GetAssignableUsersQuery : IRequest<IReadOnlyCollection<AssignableUserDto>>;

public class GetAssignableUsersQueryHandler : IRequestHandler<GetAssignableUsersQuery, IReadOnlyCollection<AssignableUserDto>>
{
    private readonly IIdentityService _identityService;

    public GetAssignableUsersQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<IReadOnlyCollection<AssignableUserDto>> Handle(GetAssignableUsersQuery request, CancellationToken cancellationToken)
    {
        return await _identityService.GetAssignableUsersAsync(cancellationToken);
    }
}
