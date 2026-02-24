import { ChangeDetectorRef, Component } from '@angular/core';
import { WeatherForecastsClient, WeatherForecast } from '../web-api-client';

@Component({
  standalone: false,
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public forecasts: WeatherForecast[] = [];

  constructor(private client: WeatherForecastsClient, private cdr: ChangeDetectorRef) {
      client.getWeatherForecasts().subscribe({
          next: result => {
              this.forecasts = result;
              this.cdr.detectChanges();
          },
          error: error => console.error(error)
      });
  }
}
