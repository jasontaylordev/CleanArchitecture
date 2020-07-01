using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserModel> GetUserAsync(string userId)
        {
            var user = await _userManager.Users.FirstAsync(u => u.Id == userId);
            var userRole = await _userManager.GetRolesAsync(user);
            return new UserModel { UserName = user.UserName, UserEmail = user.Email, UserId = user.Id, PhoneNumber = user.PhoneNumber, Role = userRole.FirstOrDefault() ?? "User"  };
        }

        public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = userName,
            };

            var result = await _userManager.CreateAsync(user, password);

            return (result.ToApplicationResult(), user.Id);
        }

        public async Task<Result> DeleteUserAsync(string userId)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            if (user != null)
            {
                return await DeleteUserAsync(user);
            }

            return Result.Success();
        }

        public async Task<Result> DeleteUserAsync(ApplicationUser user)
        {
            var result = await _userManager.DeleteAsync(user);

            return result.ToApplicationResult();
        }

        public async Task<Result> UserIsInRoleAsync(string userId, List<string> roles)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);
            var userInRole = false;

            foreach (var r in roles)
            {
                userInRole = await _userManager.IsInRoleAsync(user, r);
                if (userInRole)
                {
                    break;
                }
            }

            if (user == null || !userInRole)
            {
                return Result.Failure(new string[] { "User not found or is not in role." });
            }

            return Result.Success();
        }
      
    }
}
