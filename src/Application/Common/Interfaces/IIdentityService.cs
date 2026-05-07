using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.Users.Queries.GetAssignableUsers;

namespace CleanArchitecture.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);

    Task<IReadOnlyCollection<AssignableUserDto>> GetAssignableUsersAsync(CancellationToken cancellationToken);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);

    Task<Result> DeleteUserAsync(string userId);
}
