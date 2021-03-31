using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUI.ViewModels
{
    public class CounterViewModel : Screen
    {
        private int _counterValue = 0;

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            CounterValue = 0;
        }

        public int CounterValue 
        { 
            get => _counterValue; 
            set
            {
                if (!Equals(_counterValue, value))
                {
                    _counterValue = value;
                    NotifyOfPropertyChange(() => CounterValue);
                }
            }
        }

        public void Increment()
        {
            CounterValue += 1;
        }
    }
}
