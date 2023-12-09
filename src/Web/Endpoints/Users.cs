#if (UseApiOnly)
using CleanArchitecture.Infrastructure.Identity;

namespace CleanArchitecture.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public Users(ISender sender):base(sender) { }

    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapIdentityApi<ApplicationUser>();
    }
}
#endif
