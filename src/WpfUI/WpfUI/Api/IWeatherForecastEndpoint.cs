using System.Collections.Generic;
using System.Threading.Tasks;
using WpfUI.Models;

namespace WpfUI.Api
{
    public interface IWeatherForecastEndpoint
    {
        Task<IEnumerable<WeatherForecastDisplayModel>> GetAll();
    }
}