using CleanArchitecture.Application.WeatherForecasts.Queries.GetWeatherForecasts;

namespace CleanArchitecture.Web.Endpoints;

public class WeatherForecasts: EndpointGroupBase
{
    public WeatherForecasts(ISender sender) : base(sender) { }

    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetWeatherForecasts);
    }

    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecasts()
    {
        return await _sender.Send(new GetWeatherForecastsQuery());
    }
}
