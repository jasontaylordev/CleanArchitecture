using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Domain.Entities;

public class UserRole : IdentityUserRole<string>
{
    public virtual User User { get; set; } = null!;
    public virtual ApplicationRole Role { get; set; } = null!;
}
