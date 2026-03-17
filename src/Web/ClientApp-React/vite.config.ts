import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

const target =
  process.env['services__webapi__http__0'] ||
  "http://localhost:5000";

const proxyOptions = target
  ? { target, secure: false, changeOrigin: true }
  : undefined;

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: process.env.PORT ? parseInt(process.env.PORT) : 5173,
    proxy: proxyOptions
      ? {
          '/api': proxyOptions,
          '/openapi': proxyOptions,
          '/scalar': proxyOptions,
          '/weatherforecast': proxyOptions,
          '/WeatherForecast': proxyOptions,
        }
      : undefined,
  },
  build: {
    outDir: 'build',
  },
});
