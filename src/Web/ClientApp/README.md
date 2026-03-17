# CleanArchitecture Angular Client

This project uses [Angular CLI](https://github.com/angular/angular-cli) version 21.1.5.

## Available Scripts

### `npm run dev`

Runs the app in development mode with hot module replacement.
Opens at [http://localhost:4200](http://localhost:4200) when run manually.

### `npm start`

Used by .NET Aspire to start the app. Requires the `PORT` environment variable to be set.
Also runs `npm run generate-api` before starting to keep the API client up to date.

### `npm run build`

Builds the app for production to the `dist/` folder.
Also runs `npm run generate-api` before building.

### `npm test`

Runs unit tests via [Karma](https://karma-runner.github.io).

### `npm run generate-api`

Generates the TypeScript API client from the OpenAPI spec using NSwag.

## Project Structure

- `src/` - Angular source code
- `src/app/` - Root module and components
- `proxy.conf.js` - Dev server proxy configuration
- `angular.json` - Angular CLI configuration

## Aspire Integration

When running via .NET Aspire, the `PORT` environment variable is set automatically and the dev server proxies API requests to the ASP.NET Core backend via the `services__webapi__http__0` environment variable.

## Learn More

- [Angular Documentation](https://angular.dev/)
- [Angular CLI Overview](https://angular.dev/tools/cli)
