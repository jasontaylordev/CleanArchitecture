
using CleanArchitecture.Application.Test1.Queries.Suma;
using CleanArchitecture.Application.WeatherForecasts.Queries.GetWeatherForecasts;

namespace CleanArchitecture.Web.Endpoints;

public class Test1 : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetSuma);
    }

    public async Task<int> GetSuma(ISender sender)
    {
        return await sender.Send(new GetSumaQuery());
    }
}
