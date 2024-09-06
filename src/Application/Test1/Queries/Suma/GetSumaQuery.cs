using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Application.WeatherForecasts.Queries.GetWeatherForecasts;

namespace CleanArchitecture.Application.Test1.Queries.Suma;

public record GetSumaQuery : IRequest<int>;


public class GetSumaHandler : IRequestHandler<GetSumaQuery, int>
{

    public Task<int> Handle(GetSumaQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(1+2);
    }
}
