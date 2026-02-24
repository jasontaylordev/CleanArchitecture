using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Domain.Entities;

// https://learn.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-10.0#add-all-navigation-properties
public class UserRole : IdentityUserRole<string>
{
    public virtual User User { get; set; } = null!;
    public virtual ApplicationRole Role { get; set; } = null!;
}
