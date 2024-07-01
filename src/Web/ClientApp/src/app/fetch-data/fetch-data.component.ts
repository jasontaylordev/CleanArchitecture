import { Component } from '@angular/core';
import { WeatherForecastsClient, WeatherForecast } from '../web-api-client';
import { NgIf, NgFor, DatePipe } from '@angular/common';

@Component({
    selector: 'app-fetch-data',
    templateUrl: './fetch-data.component.html',
    standalone: true,
    imports: [NgIf, NgFor, DatePipe]
})
export class FetchDataComponent {
  public forecasts: WeatherForecast[] = [];

  constructor(private client: WeatherForecastsClient) {
      client.getWeatherForecasts().subscribe({
          next: result => this.forecasts = result,
          error: error => console.error(error)
      });
  }
}
