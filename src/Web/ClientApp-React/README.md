# CleanArchitecture React Client

This project uses [Vite](https://vitejs.dev/) with React 19 and TypeScript.

## Available Scripts

### `npm start`

Runs the app in development mode with hot module replacement.
Opens at [http://localhost:5173](http://localhost:5173) when run manually.

Also runs `npm run generate-api` before starting to keep the API client up to date.

### `npm run build`

Builds the app for production to the `build` folder.
Optimizes the build for best performance.

Also runs `npm run generate-api` before building.

### `npm run preview`

Previews the production build locally.

### `npm run lint`

Runs ESLint on the src directory.

### `npm run generate-api`

Generates the TypeScript API client from the OpenAPI spec using NSwag.

## Project Structure

- `src/` - React source code
- `src/main.jsx` - Application entry point
- `src/App.js` - Root component
- `src/components/` - React components
- `public/` - Static assets (favicon, manifest)
- `vite.config.ts` - Vite configuration with proxy settings
- `index.html` - HTML template

## Aspire Integration

When running via .NET Aspire, the `PORT` environment variable is set automatically and the dev server proxies API requests to the ASP.NET Core backend via the `services__webapi__https__0` / `services__webapi__http__0` environment variables.

## Learn More

- [Vite Documentation](https://vitejs.dev/)
- [React Documentation](https://react.dev/)
