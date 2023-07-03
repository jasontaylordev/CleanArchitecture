# Clean Architecture Solution Template

[![Build](https://github.com/jasontaylordev/CleanArchitecture/actions/workflows/dotnet-build.yml/badge.svg)](https://github.com/jasontaylordev/CleanArchitecture/actions/workflows/dotnet-build.yml)
[![CodeQL](https://github.com/jasontaylordev/CleanArchitecture/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/jasontaylordev/CleanArchitecture/actions/workflows/codeql-analysis.yml)
[![Nuget](https://img.shields.io/nuget/v/Clean.Architecture.Solution.Template?label=NuGet)](https://www.nuget.org/packages/Clean.Architecture.Solution.Template)
[![Nuget](https://img.shields.io/nuget/dt/Clean.Architecture.Solution.Template?label=Downloads)](https://www.nuget.org/packages/Clean.Architecture.Solution.Template)
[![Discord](https://img.shields.io/discord/893301913662148658?label=Discord)](https://discord.gg/p9YtBjfgGe)
![Twitter Follow](https://img.shields.io/twitter/follow/jasontaylordev?label=Follow&style=social)

The goal of this template is to provide a straightforward and efficient approach to enterprise application development, leveraging the power of Clean Architecture and ASP.NET Core. Using this template, you can effortlessly create a Single Page App (SPA) with ASP.NET Core and Angular or React, while adhering to the principles of Clean Architecture. Getting started is easy - simply install the **.NET template** (see below for full details).

## Technologies

* [ASP.NET Core 8](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core)
* [Entity Framework Core 8](https://docs.microsoft.com/en-us/ef/core/)
* [Angular 15](https://angular.io/) or [React 18](https://react.dev/)
* [MediatR](https://github.com/jbogard/MediatR)
* [AutoMapper](https://automapper.org/)
* [FluentValidation](https://fluentvalidation.net/)
* [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq) & [Respawn](https://github.com/jbogard/Respawn)

## Dependencies
The template depends on the latest versions of:

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [Node.js LTS](https://nodejs.org/en/)

## Getting Started

The easiest way to get started is to install the [.NET template](https://www.nuget.org/packages/Clean.Architecture.Solution.Template):
```
dotnet new install Clean.Architecture.Solution.Template::8.0.0-preview.5.2
````

Once installed, create a new solution using the template:
```
dotnet new ca-sln -c <Angular|React> --output <YourProjectName>
```

The above command creates a SPA with Angular or React on ASP.NET Core. Start the application by navigating to `./src/WebUI` and running:
```
dotnet run
```

Create use cases (commands or queries) by navigating to `./src/Application`, and running:
```
dotnet new ca-usecase --feature TodoLists --name CreateTodoList --useCaseType command --returnType int
```

To learn more, run the following command:

```
dotnet new ca-usecase --help
```

## Database

The template is configured to use an in-memory database by default. This ensures that all users will be able to run the solution without needing to set up additional infrastructure (e.g. SQL Server).

If you would like to use SQL Server, you will need to update **WebUI/appsettings.json** as follows:

```json
  "UseInMemoryDatabase": false,
```

Verify that the **DefaultConnection** connection string within **appsettings.json** points to a valid SQL Server instance. 

When you run the application the database will be automatically created (if necessary) and the latest migrations will be applied.

### Database Migrations

To use `dotnet-ef` for your migrations first ensure that "UseInMemoryDatabase" is disabled, as described within previous section.
Then, add the following flags to your command (values assume you are executing from repository root)

* `--project src/Infrastructure` (optional if in this folder)
* `--startup-project src/WebUI`
* `--output-dir Persistence/Migrations`

For example, to add a new migration from the root folder:

 `dotnet ef migrations add "SampleMigration" --project src\Infrastructure --startup-project src\WebUI --output-dir Persistence\Migrations`

## Versions
The main branch is now on .NET 8.0. The following previous versions are available:

* [7.0](https://github.com/jasontaylordev/CleanArchitecture/tree/net7.0)
* [6.0](https://github.com/jasontaylordev/CleanArchitecture/tree/net6.0)
* [5.0](https://github.com/jasontaylordev/CleanArchitecture/tree/net5.0)
* [3.1](https://github.com/jasontaylordev/CleanArchitecture/tree/netcore3.1)

## Learn More

* [Clean Architecture with ASP.NET Core 3.0 (GOTO 2019)](https://youtu.be/dK4Yb6-LxAk)
* [Clean Architecture with .NET Core: Getting Started](https://jasontaylor.dev/clean-architecture-getting-started/)

## Support

If you are having problems, please let me know by [raising a new issue](https://github.com/jasontaylordev/CleanArchitecture/issues/new/choose).

## License

This project is licensed with the [MIT license](LICENSE).
