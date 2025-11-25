using Cubido.Template.Infrastructure.Identity;

namespace Cubido.Template.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder route)
    {
        route
            .MapIdentityApi<ApplicationUser>();
    }
}
