import { ChangeDetectorRef, Component, ChangeDetectionStrategy } from '@angular/core';
import { WeatherForecastsClient, WeatherForecast } from '../web-api-client';

@Component({
  standalone: false,
  selector: 'app-weather',
  changeDetection: ChangeDetectionStrategy.Eager,
  templateUrl: './weather.component.html'
})
export class WeatherComponent {
  forecasts: WeatherForecast[] = [];
  loading = true;
  error: string | null = null;

  constructor(private client: WeatherForecastsClient, private cdr: ChangeDetectorRef) {
    client.getWeatherForecasts().subscribe({
      next: result => {
        this.forecasts = result;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.error = 'Unable to load weather forecasts. Please try again later.';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }
}
