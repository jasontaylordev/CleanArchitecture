using CleanArchitecture.Application.WeatherForecasts.Queries.GetWeatherForecasts;

namespace CleanArchitecture.Web.Endpoints;

public class WeatherForecasts : EndpointGroupBase
{
    public override void Map(WebApplication app) => app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetWeatherForecasts);

    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecasts(ISender sender) =>
        await sender.Send(new GetWeatherForecastsQuery());
}
