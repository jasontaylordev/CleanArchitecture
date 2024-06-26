using System.Security.Claims;

using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Web.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
