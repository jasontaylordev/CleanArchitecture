# CleanArchitecture

The project was generated using the [Clean.Architecture.Solution.Template](caRepositoryUrl) version caPackageVersion.

## Prerequisites

- The [.NET SDK](https://dotnet.microsoft.com/download) for `net10.0` (`global.json` sets the minimum version)
- [Node.js](https://nodejs.org/) (LTS), if the solution includes an Angular or React client
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) or [Podman](https://podman.io/), if the solution uses SQL Server or PostgreSQL. Aspire runs the database in a container, for local development and the functional tests.
- The [Aspire CLI](https://aspire.dev/get-started/install-cli/) (recommended)

Alternatively, open the solution in the included [Dev Container](#dev-container), which has everything installed.

## Build

Run `dotnet build` to build the solution.

## Run

To run the application:

Using the [Aspire CLI](https://aspire.dev/get-started/install-cli/) (recommended):

```bash
aspire run
```

Or using the .NET CLI:

```bash
dotnet run --project ./src/AppHost
```

The Aspire dashboard will open automatically, showing the application URLs and logs.

## Code Styles & Formatting

The template includes [EditorConfig](https://editorconfig.org/) support to help maintain consistent coding styles for multiple developers working on the same project across various editors and IDEs. The **.editorconfig** file defines the coding styles applicable to this solution.

## Code Scaffolding

The template includes support to scaffold new commands and queries.

Start in the `./src/Application/` folder.

Create a new command:

```
dotnet new ca-usecase --name CreateTodoList --feature-name TodoLists --usecase-type command --return-type int
```

Create a new query:

```
dotnet new ca-usecase -n GetTodos -fn TodoLists -ut query -rt TodosVm
```

If you encounter the error *"No templates or subcommands found matching: 'ca-usecase'."*, install the template and try again:

```bash
dotnet new install Clean.Architecture.Solution.Template::caPackageVersion
```

## Test

The solution contains unit, integration, and functional tests.

To run the tests:
```bash
dotnet test
```

## Dev Container

The `.devcontainer` folder configures a [Dev Container](https://containers.dev/) with the .NET SDK, the Aspire CLI, Docker and (for Angular or React solutions) Node.js. In VS Code, install the Dev Containers extension and select **Reopen in Container**, or open the repository in GitHub Codespaces. See [.devcontainer/README.md](.devcontainer/README.md) for details, including the certificate warning you may see in your browser.

## Deploy

To deploy the solution to Azure, see the [deployment guide](caDocsUrl/docs/getting-started/deployment/).

## Architectural Decisions

Key design decisions are documented as [Architecture Decision Records](docs/decisions/).

## Help
To learn more about the template go to the [project website](caDocsUrl). Here you can find additional guidance, request new features, report a bug, and discuss the template with other users.