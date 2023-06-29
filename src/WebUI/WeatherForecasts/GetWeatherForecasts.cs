using CleanArchitecture.Application.WeatherForecasts.Queries.GetWeatherForecasts;

namespace CleanArchitecture.WebUI.WeatherForecasts;

public class GetWeatherForecasts : EndpointBase
{
    public override void Map(WebApplication app)
    {
        MapGet(app,
            async (ISender sender) => await sender.Send(new GetWeatherForecastsQuery()));
    }
}