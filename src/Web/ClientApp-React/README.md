# CleanArchitecture React Client

This project uses [Vite](https://vitejs.dev/) with React 19 and TypeScript.

## Available Scripts

### `npm start`

Runs the app in development mode with hot module replacement.
Opens at [https://localhost:44447](https://localhost:44447).

The development server proxies API requests to the ASP.NET Core backend.

### `npm run build`

Builds the app for production to the `build` folder.
Optimizes the build for best performance.

### `npm run preview`

Previews the production build locally.

### `npm run lint`

Runs ESLint on the src directory.

## Project Structure

- `src/` - React source code
- `src/main.jsx` - Application entry point
- `src/App.js` - Root component
- `src/components/` - React components
- `public/` - Static assets (favicon, manifest)
- `vite.config.ts` - Vite configuration with proxy settings
- `index.html` - HTML template

## Environment Variables

Vite environment variables must be prefixed with `VITE_` to be exposed to client code.

Example:
```
VITE_API_URL=https://api.example.com
```

Access in code:
```javascript
const apiUrl = import.meta.env.VITE_API_URL;
```

## HTTPS Configuration

The development server uses ASP.NET Core development certificates for HTTPS.
Run `npm start` to automatically set up certificates via `aspnetcore-https.js`.

## Learn More

- [Vite Documentation](https://vitejs.dev/)
- [React Documentation](https://react.dev/)
