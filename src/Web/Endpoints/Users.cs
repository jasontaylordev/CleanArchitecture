#if (UseApiOnly)
using CleanArchitecture.Infrastructure.Identity;

namespace CleanArchitecture.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapIdentityApi<ApplicationUser>();
    }
}
#endif
