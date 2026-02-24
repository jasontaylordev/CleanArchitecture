using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;

namespace CleanArchitecture.Application.Account.Queries.UserProfile;

public record UserProfileQuery : IRequest<UserDto?>;

public class UserProfileQueryValidator : AbstractValidator<UserProfileQuery>
{
    public UserProfileQueryValidator()
    {
    }
}

public class UserProfileQueryHandler(IIdentityService identityService, IUser user, IMapper mapper) : IRequestHandler<UserProfileQuery, UserDto?>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly IUser _currentUser = user;
    private readonly IMapper _mapper = mapper;

    public async Task<UserDto?> Handle(UserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = string.IsNullOrEmpty(_currentUser.Id)
            ? throw new UnauthorizedAccessException("User not logged in.")
            : await _identityService.GetUserByIdAsync(_currentUser.Id);

        return _mapper.Map<UserDto?>(user);
    }
}
