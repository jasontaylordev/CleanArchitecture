using System.Collections.Generic;

namespace CleanArchitecture.Web.Endpoints;

public class UserProfileDto
{
    public string? Id { get; init; }
    public string? Email { get; init; }
    public List<string> Roles { get; init; } = [];
}
