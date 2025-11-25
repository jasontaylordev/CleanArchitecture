using Cubido.Template.Application.Common.Interfaces;
using System.Security.Claims;

namespace Cubido.Template.Web.Services;

public class CurrentUser : IUser
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public string? Id => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
