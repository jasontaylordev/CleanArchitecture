using Caliburn.Micro;
using System;
using System.Threading;


namespace WpfUI.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        public ShellViewModel()
        {
            ActivateHomeView();
        }

        public void ActivateHomeView()
        {
            ActivateItemAsync(IoC.Get<HomeViewModel>(), new CancellationToken());
        }

        public void ActivateCounterView()
        {
            ActivateItemAsync(IoC.Get<NotImplementedViewModel>(), new CancellationToken());
        }

        public void ActivateWeatherForecastView()
        {
            ActivateItemAsync(IoC.Get<WeatherForecastViewModel>(), new CancellationToken());
        }

        public void ActivateTodoView()
        {
            ActivateItemAsync(IoC.Get<NotImplementedViewModel>(), new CancellationToken());
        }

        public void ActivateSwaggerApiView()
        {
            ActivateItemAsync(IoC.Get<NotImplementedViewModel>(), new CancellationToken());
        }

        public void ActivateRegisterView()
        {
            ActivateItemAsync(IoC.Get<NotImplementedViewModel>(), new CancellationToken());
        }

        public void ActivateLoginView()
        {
            ActivateItemAsync(IoC.Get<NotImplementedViewModel>(), new CancellationToken());
        }
    } 
}