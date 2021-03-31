using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Api;
using WpfUI.Models;

namespace WpfUI.ViewModels
{
    public class WeatherForecastViewModel : Screen
    {
        private ObservableCollection<WeatherForecastDisplayModel> _forecasts = new ObservableCollection<WeatherForecastDisplayModel>();
        private bool _isLoading;
        private IWeatherForecastEndpoint _weatherForecastEndpoint;

        public WeatherForecastViewModel(IWeatherForecastEndpoint weatherForecastEndpoint)
        {
            _weatherForecastEndpoint = weatherForecastEndpoint;
            IsLoading = true;
        }

        protected async override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            await LoadWeatherForecast();
        }

        public async Task LoadWeatherForecast()
        {
            try
            {
                IsLoading = true;

                var forecasts = await _weatherForecastEndpoint.GetAll();
                Forecasts = new ObservableCollection<WeatherForecastDisplayModel>(forecasts);

                IsLoading = (Forecasts.Count > 0 ? false : true);
            }
            catch (Exception)
            {
                IsLoading = true;
                Forecasts.Clear();
            }
        }

        public ObservableCollection<WeatherForecastDisplayModel> Forecasts
        {
            get => _forecasts;
            set 
            {
                if (!Equals(_forecasts, value))
                {
                    _forecasts = value;
                    NotifyOfPropertyChange(() => Forecasts);
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set 
            { 
                if (!Equals(_isLoading, value))
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }
    }
}