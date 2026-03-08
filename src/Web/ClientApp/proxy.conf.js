const { env } = require('process');

const target =
  env["services__web__https__0"] ||
  env["services__web__http__0"];

const PROXY_CONFIG = [
  {
    context: [
      "/api",
      "/openapi",
      "/Identity",
      "/weatherforecast",
      "/WeatherForecast"
    ],
    target: target,
    secure: env["NODE_ENV"] !== "development",
  }
];

module.exports = PROXY_CONFIG;
