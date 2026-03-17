const { env } = require('process');

const target =
  env["services__webapi__http__0"] ||
  "http://localhost:5000";

const PROXY_CONFIG = [
  {
    context: [
      "/api",
      "/openapi",
      "/scalar",
      "/weatherforecast",
      "/WeatherForecast"
    ],
    target: target,
    secure: env["NODE_ENV"] !== "development",
  }
];

module.exports = PROXY_CONFIG;
