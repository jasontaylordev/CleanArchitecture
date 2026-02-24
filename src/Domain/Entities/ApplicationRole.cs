using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Domain.Entities;

public class ApplicationRole : IdentityRole
{
    public virtual ICollection<UserRole> UserRoles { get; set; } = [];
    public ApplicationRole() { }
    public ApplicationRole(string roleName) : base(roleName) { }
}
