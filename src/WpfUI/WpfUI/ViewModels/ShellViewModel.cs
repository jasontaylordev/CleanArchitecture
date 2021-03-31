using Caliburn.Micro;
using System;
using System.Threading;


namespace WpfUI.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        public ShellViewModel()
        {
            ActivateItemAsync(IoC.Get<HomeViewModel>(), new CancellationToken());
        }

        public void ActivateHomeView()
        {
            ActivateItemAsync(IoC.Get<HomeViewModel>(), new CancellationToken());
        }

        public void ActivateWeatherForecastView()
        {
            ActivateItemAsync(IoC.Get<WeatherForecastViewModel>(), new CancellationToken());
        }
    }

    
}
