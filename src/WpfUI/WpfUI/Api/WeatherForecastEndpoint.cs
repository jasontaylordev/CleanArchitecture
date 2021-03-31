using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Models;

namespace WpfUI.Api
{
    public class WeatherForecastEndpoint : IWeatherForecastEndpoint
    {
        private IAPIHelper _apiHelper;

        public WeatherForecastEndpoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<IEnumerable<WeatherForecastDisplayModel>> GetAll()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/WeatherForecast"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<IEnumerable<WeatherForecastDisplayModel>>(
                        await response.Content.ReadAsStringAsync());
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
