using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Identity
{
    public class UserClaimPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        public UserClaimPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
        {

        }

        public async override Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var principal = await base.CreateAsync(user);
            var identity = (ClaimsIdentity)principal.Identity;
            var userRoles = await UserManager.GetRolesAsync(user);

            var role = userRoles.FirstOrDefault();
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Role, role),
                new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber),
                new Claim(JwtClaimTypes.Picture, !string.IsNullOrEmpty(user.UserPhoto) ? user.UserPhoto : ""),
                new Claim(JwtClaimTypes.Email, user.Email),
            };
            identity.AddClaims(claims);
            return principal;
        }

        //protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        //{
        //    var userRoles = await UserManager.GetRolesAsync(user);
        //    var role = userRoles.FirstOrDefault();
        //    var identity = await base.GenerateClaimsAsync(user);
        //    identity.AddClaim(new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber ?? "[Click to edit profile]"));
        //    identity.AddClaim(new Claim(JwtClaimTypes.Role, role));
        //    return identity;
        //}
    }
}
