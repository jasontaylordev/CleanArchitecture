const { env } = require('process');

const target =
  env["services__webapi__https__0"] ||
  env["services__webapi__http__0"];

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
