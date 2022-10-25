using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebUI.OidcConfiguration;

public class OidcConfiguration : AbstractEndpoint
{
    public override void Map(WebApplication app)
    {
        MapGet(app, "/_configuration/{clientId}", (IClientRequestParametersProvider clientRequestParametersProvider, HttpContext context, string clientId) =>
        {
            var parameters = clientRequestParametersProvider.GetClientParameters(context, clientId);
            return Results.Ok(parameters);
        }, withDefaults: false).ExcludeFromDescription();
    }
}