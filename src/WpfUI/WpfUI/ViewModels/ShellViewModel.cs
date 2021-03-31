using Caliburn.Micro;
using System;
using System.Threading;


namespace WpfUI.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        private WeatherForecastViewModel _weatherForecastVM;



        public ShellViewModel(WeatherForecastViewModel weatherForecastVM)
        {
            _weatherForecastVM = weatherForecastVM;
            ActivateHomeView();
        }

        public void ActivateHomeView()
        {
            ActivateItemAsync(IoC.Get<HomeViewModel>(), new CancellationToken());
        }

        public void ActivateCounterView()
        {
            ActivateItemAsync(IoC.Get<CounterViewModel>(), new CancellationToken());
        }

        public void ActivateWeatherForecastView()
        {
            ActivateItemAsync(_weatherForecastVM, new CancellationToken());
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