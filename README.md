# Clean Architecture Solution Template


This is a solution template for creating a Blazor WASM App and ASP.NET Core following the principles of Clean Architecture. 
It is a fork of the [Clean Architecture repository by Jason Taylor](https://github.com/jasontaylordev/CleanArchitecture) where we removed his Angular SPA and added a Blazor WASM app.

## Learn about Clean Architecture

[![Clean Architecture with ASP.NET Core 3.0 • Jason Taylor • GOTO 2019](https://img.youtube.com/vi/dK4Yb6-LxAk/0.jpg)](https://www.youtube.com/watch?v=dK4Yb6-LxAk)

## Technologies

* ASP.NET Core 5
* [Entity Framework Core 5](https://docs.microsoft.com/en-us/ef/core/)
* [MediatR](https://github.com/jbogard/MediatR)
* [AutoMapper](https://automapper.org/)
* [FluentValidation](https://fluentvalidation.net/)
* [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq) & [Respawn](https://github.com/jbogard/Respawn)
* [BlazorFluentUI](https://github.com/BlazorFluentUI/BlazorFluentUI)
* [Docker](https://www.docker.com/) (not fully tested yet)

## Getting Started


### Docker Configuration

In order to get Docker working, you will need to add a temporary SSL cert and mount a volume to hold that cert.
You can find [Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.1) that describe the steps required for Windows, macOS, and Linux.

For Windows:
The following will need to be executed from your terminal to create a cert
`dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p Your_password123`
`dotnet dev-certs https --trust`

NOTE: When using PowerShell, replace %USERPROFILE% with $env:USERPROFILE.

FOR macOS:
`dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123`
`dotnet dev-certs https --trust`

FOR Linux:
`dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123`

In order to build and run the docker containers, execute `docker-compose -f 'docker-compose.yml' up --build` from the root of the solution where you find the docker-compose.yml file.  You can also use "Docker Compose" from Visual Studio for Debugging purposes.
Then open http://localhost:5000 on your browser.

To disable Docker in Visual Studio, right-click on the **docker-compose** file in the **Solution Explorer** and select **Unload Project**.

### Database Configuration

The template is configured to use an in-memory database by default. This ensures that all users will be able to run the solution without needing to set up additional infrastructure (e.g. SQL Server).

If you would like to use SQL Server, you will need to update **WebUI/appsettings.json** as follows:

```json
  "UseInMemoryDatabase": false,
```

Verify that the **DefaultConnection** connection string within **appsettings.json** points to a valid SQL Server instance. 

When you run the application the database will be automatically created (if necessary) and the latest migrations will be applied.

### Database Migrations

To use `dotnet-ef` for your migrations please add the following flags to your command (values assume you are executing from repository root)

* `--project src/Infrastructure` (optional if in this folder)
* `--startup-project src/WebUI`
* `--output-dir Persistence/Migrations`

For example, to add a new migration from the root folder:

 `dotnet ef migrations add "SampleMigration" --project src\Infrastructure --startup-project src\WebUI --output-dir Persistence\Migrations`

## Overview

### Domain

This will contain all entities, enums, exceptions, interfaces, types and logic specific to the domain layer.

### Application

This layer contains all application logic. It is dependent on the domain layer, but has no dependencies on any other layer or project. This layer defines interfaces that are implemented by outside layers. For example, if the application need to access a notification service, a new interface would be added to application and an implementation would be created within infrastructure.

### Infrastructure

This layer contains classes for accessing external resources such as file systems, web services, smtp, and so on. These classes should be based on interfaces defined within the application layer.

### WebUI

This layer depends on both the Application and Infrastructure layers, however, the dependency on Infrastructure is only to support dependency injection. Therefore only *Startup.cs* should reference Infrastructure.

### BlazorClient

This is the Blazor WASM client. The app is served by WebUI and communicates with the WebUI API.

## Support

If you are having problems, please let us know by [raising a new issue](https://github.com/JohnMidity/CleanArchitecture/issues/new/choose).

## License

This project is licensed with the [MIT license](LICENSE).
