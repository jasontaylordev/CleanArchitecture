using CleanArchitecture.Application.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<UserModel> GetUserAsync(string userId);

        Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);

        Task<Result> DeleteUserAsync(string userId);
        Task<Result> UserIsInRoleAsync(string userId, List<string> roles);
    }
}
