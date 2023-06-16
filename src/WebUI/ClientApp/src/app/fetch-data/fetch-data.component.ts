import { Component } from '@angular/core';
import { WeatherForecastClient, WeatherForecast } from '../web-api-client';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public forecasts: WeatherForecast[] = [];

  constructor(private client: WeatherForecastClient) {
    client.get().subscribe({
      next: result => this.forecasts = result,
      error: error => console.error(error)
    });
  }
}
