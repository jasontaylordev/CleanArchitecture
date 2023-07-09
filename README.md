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
dotnet new install Clean.Architecture.Solution.Template::8.0.0-preview.5.13
```

Once installed, create a new solution using the template. You can choose to use Angular, React, or create a Web API-only solution. Specify the client framework using the `-cf` or `--client-framework` option, and provide the output directory where your project will be created. Here are some examples:

To create a SPA with Angular:
```
dotnet new ca-sln --client-framework Angular --output YourProjectName
```

To create a SPA with React:
```
dotnet new ca-sln -cf React -o YourProjectName
```

To create a Web API-only solution:
```
dotnet new ca-sln -cf None -o YourProjectName
```

The above commands will create a Single-Page Application (SPA) with Angular or React on top of ASP.NET Core, or a Web API-only solution.

Start the application by navigating to ``./src/WebUI`` (SPA) or ``./src/WebApi`` and running:
```
dotnet run
```

To learn more, run the following command:
```
dotnet new ca-sln --help
```

You can create use cases (commands or queries) by navigating to `./src/Application` and running `dotnet new ca-usecase`. Here are some examples:

To create a new command:
```
dotnet new ca-usecase --name CreateTodoList --feature-name TodoLists --usecase-type command --return-type int
```

To create a query:
```
dotnet new ca-usecase -n GetTodos -fn TodoLists -ut query -rt TodosVm
```

To learn more, run the following command:
```
dotnet new ca-usecase --help
```

## Database

The template is configured to use SQL Server by default. If you would prefer to use SQLite, create your solution using the following command:

```bash
dotnet new ca-sln --use-sqlite
```

When you run the application the database will be automatically created (if necessary) and the latest migrations will be applied.

### Database Migrations

Running database migrations is easy. Ensure you add the following flags to your command (values assume you are executing from repository root)

* `--project src/Infrastructure` (optional if in this folder)
* `--startup-project src/WebUI`
* `--output-dir Data/Migrations`

For example, to add a new migration from the root folder:

 `dotnet ef migrations add "SampleMigration" --project src\Infrastructure --startup-project src\WebUI --output-dir Data\Migrations`

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
