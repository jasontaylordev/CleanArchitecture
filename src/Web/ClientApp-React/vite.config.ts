import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import fs from 'fs';
import path from 'path';

const baseFolder =
  process.env.APPDATA !== undefined && process.env.APPDATA !== ''
    ? `${process.env.APPDATA}/ASP.NET/https`
    : `${process.env.HOME}/.aspnet/https`;

const certificateName = 'cleanarchitecture.web';
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

const target = process.env.ASPNETCORE_HTTPS_PORT
  ? `https://localhost:${process.env.ASPNETCORE_HTTPS_PORT}`
  : process.env.ASPNETCORE_URLS
    ? process.env.ASPNETCORE_URLS.split(';')[0]
    : 'https://localhost:5001';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: 44447,
    https: fs.existsSync(certFilePath) && fs.existsSync(keyFilePath)
      ? {
          cert: fs.readFileSync(certFilePath),
          key: fs.readFileSync(keyFilePath),
        }
      : undefined,
    proxy: {
      '/api': {
        target,
        secure: false,
        changeOrigin: true,
      },
      '/scalar': {
        target,
        secure: false,
        changeOrigin: true,
      },
      '/openapi': {
        target,
        secure: false,
        changeOrigin: true,
      },
      '/Identity': {
        target,
        secure: false,
        changeOrigin: true,
      },
      '/weatherforecast': {
        target,
        secure: false,
        changeOrigin: true,
      },
      '/WeatherForecast': {
        target,
        secure: false,
        changeOrigin: true,
      },
    },
  },
  build: {
    outDir: 'build',
  },
});
