import { useState, useEffect } from 'react';
import { WeatherForecastsClient } from '../web-api-client.ts';

export function Weather() {
  const [forecasts, setForecasts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    async function fetchData() {
      try {
        const client = new WeatherForecastsClient();
        const data = await client.getWeatherForecasts();
        setForecasts(data);
      } catch (e) {
        setError('Unable to load weather forecasts. Please try again later.');
      } finally {
        setLoading(false);
      }
    }
    fetchData();
  }, []);

  return (
    <div>
      <h1>Weather</h1>
      <p>This component demonstrates fetching data from the server.</p>
      {loading && <span aria-busy="true">Fetching your weather forecast...</span>}
      {error && <p className="error">{error}</p>}
      {!loading && !error && (
        <table>
          <thead>
            <tr>
              <th>Date</th>
              <th>Temp. (C)</th>
              <th>Temp. (F)</th>
              <th>Summary</th>
            </tr>
          </thead>
          <tbody>
            {forecasts.map(forecast =>
              <tr key={forecast.date}>
                <td>{new Date(forecast.date).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })}</td>
                <td>{forecast.temperatureC}</td>
                <td>{forecast.temperatureF}</td>
                <td>{forecast.summary}</td>
              </tr>
            )}
          </tbody>
        </table>
      )}
    </div>
  );
}
