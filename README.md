 <img align="left" width="116" height="116" src="https://raw.githubusercontent.com/JasonGT/CleanArchitecture/master/.github/icon.png" />
 
 # Clean Architecture Solution Template
[![Clean.Architecture.Solution.Template NuGet Package](https://img.shields.io/badge/nuget-1.0.5-blue)](https://www.nuget.org/packages/Clean.Architecture.Solution.Template)
[![Twitter Follow](https://img.shields.io/twitter/follow/jasongtau.svg?style=social&label=Follow)](https://twitter.com/jasongtau)

<br/>

This is a solution template for creating a Single Page App (SPA) with Angular 8 and ASP.NET Core 3 following the principles of Clean Architecture. Create a new project based on this template by clicking the above **Use this template** button or by installing and running the associated NuGet package (see Getting Started for full details). 


## Technologies
* .NET Core 3.1
* ASP .NET Core 3.1
* Entity Framework Core 3.1
* Angular 8

## Getting Started

The easiest way to get started is to install the [NuGet package](https://www.nuget.org/packages/Clean.Architecture.Solution.Template) and run `dotnet new ca-sln`:

1. Install the latest [.NET Core SDK](https://dotnet.microsoft.com/download)
2. Run `dotnet new --install Clean.Architecture.Solution.Template` to install the project template
3. Run `dotnet new ca-sln` to create a new project
4. Navigate to `src/WebUI` and run `dotnet run` to launch the project

## Overview

### Domain

This will contain all entities, enums, exceptions, interfaces, types and logic specific to the domain layer.


### Application

This layer contains all application logic. It is dependent on the domain layer, but has no dependencies on any other layer or project. This layer defines interfaces that are implemented by outside layers. For example, if the application need to access a notification service, a new interface would be added to application and an implementation would be created within infrastructure.


### Infrastructure

This layer contains classes for accessing external resources such as file systems, web services, smtp, and so on. These classes should be based on interfaces defined within the application layer.

### WebUI

This layer is a single page application based on Angular 8 and ASP.NET Core 3. This layer depends on both the Application and Infrastructure layers, however, the dependency on Infrastructure is only to support dependency injection. Therefore only *Startup.cs* should reference Infrastructure.

## Support

If you are having problems, please let us know by [raising a new issue](https://github.com/JasonGT/CleanArchitecture/issues/new/choose).

## License

This project is licensed with the [MIT license](LICENSE).
