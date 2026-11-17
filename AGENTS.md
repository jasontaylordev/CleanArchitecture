# Clean Architecture Agent Guide

Use this guide when changing either this template repository or a solution generated from it. Prefer existing patterns over introducing new abstractions, and keep changes focused on the requested behaviour.

## Solution context

This guide is shared by the `ca-sln` template source and the solutions generated from it.

- If `.template.config` exists, you are working in the template source. Changes can affect multiple generated variants.
- If `.template.config` does not exist, you are working in one generated solution. Follow its selected client framework and database provider, and ignore template-maintenance steps.

The template supports:

- Angular, React, or Web API only
- SQLite, PostgreSQL, or SQL Server

In the template source, files containing directives such as `#if (UseApiOnly)` are not dead code. Preserve the directive comments and consider every affected template variant when editing them.

## Project boundaries

Keep dependencies pointing inward:

- `src/Domain` contains entities, value objects, enums, exceptions, and domain events. It must not depend on Application, Infrastructure, or Web. Its intentional `MediatR.Contracts` dependency is documented in ADR-003.
- `src/Application` contains use cases, validation, authorization metadata, behaviours, models, and interfaces owned by the application. It depends on Domain, never on Infrastructure or Web.
- `src/Infrastructure` implements Application interfaces and contains EF Core persistence, identity, and external-service concerns. It may depend on Application and Shared.
- `src/Web` is the HTTP/UI boundary and composition root. Keep endpoints thin: translate HTTP input/output and dispatch Application requests through `ISender`.
- `src/AppHost` orchestrates the application with Aspire for local development and acceptance tests.
- `src/ServiceDefaults` contains shared observability, health-check, service-discovery, and resilience defaults.
- `src/Shared` contains contracts needed across orchestration boundaries. Do not move domain or application business rules here.

Before changing an architectural boundary, read the accepted records in `docs/decisions`. Add a new ADR from `ADR-000-template.md` when making a significant new architectural decision, and update `docs/decisions/README.md`.

## Application patterns

- Organize use cases by feature, then by command or query, following `src/Application/TodoLists` and `src/Application/TodoItems`.
- Represent requests as MediatR records and keep their handlers in the same feature folder.
- Commands change state; queries read and project state. Keep request-specific validators next to the request.
- Use `IApplicationDbContext` directly in handlers when data access is required. Do not add a generic repository layer merely to hide EF Core; ADR-001 explicitly accepts EF Core abstractions in Application.
- Put business invariants that belong to entities or value objects in Domain. Put use-case orchestration in Application, concrete I/O in Infrastructure, and transport concerns in Web.
- Implement Application-owned interfaces in Infrastructure and register implementations in the relevant `DependencyInjection.cs` file.
- Derive domain events from `BaseEvent`; they are directly publishable through MediatR as described in ADR-003.
- Use the `ca-usecase` item template for new commands and queries when practical. From `src/Application`, for example:

  ```bash
  dotnet new ca-usecase --name CreateTodoList --feature-name TodoLists --usecase-type command --return-type int
  ```

## Template-specific changes

This section applies only when `.template.config` exists.

- The root template is configured in `.template.config/template.json`; the use-case item template is under `templates/ca-use-case`.
- Template source contains all supported alternatives at once. Generated solutions contain only the selected client framework and database provider.
- In this repository, Angular lives in `src/Web/ClientApp` and React in `src/Web/ClientApp-React`; the React directory is renamed to `ClientApp` in generated React solutions.
- Keep template condition markers balanced and in their existing comment form. When adding, removing, or renaming variant-specific files, update the include, exclude, and rename rules in `.template.config/template.json`.
- Edit template source, not files under a temporary generated-output directory. Generated output is disposable validation data.
- For changes to template selection or conditional files, generate and validate every affected client/database combination. The full CI matrix is defined in `.github/workflows/test-templates.yml`.

## Coding conventions

- Follow `.editorconfig`. C# uses four-space indentation, file-scoped namespaces, sorted `System` directives, and braces.
- Nullable reference types and implicit usings are enabled. All build warnings are treated as errors.
- Keep public names in PascalCase, interfaces prefixed with `I`, parameters and locals in camelCase, and private instance fields in `_camelCase`.
- Prefer the established result, exception, mapping, validation, and endpoint patterns over new framework choices.
- Never commit secrets, connection credentials, generated build artifacts, or untracked machine-specific configuration.

## Tests

Place tests according to the behaviour being changed:

- `tests/Domain.UnitTests`: pure domain behaviour and value objects
- `tests/Application.UnitTests`: isolated Application behaviour, mappings, and pipeline behaviours
- `tests/Application.FunctionalTests`: use cases that exercise persistence and the application stack
- `tests/Infrastructure.IntegrationTests`: concrete infrastructure integrations
- `tests/Web.AcceptanceTests`: end-to-end UI behaviour through Aspire and Playwright

Functional tests use Aspire with a real database rather than mocked repositories. SQLite is the default; PostgreSQL and SQL Server variants require Docker, Podman, or another compatible container runtime.

Run the smallest relevant test project while iterating, then validate the solution before handing off:

```bash
dotnet restore
dotnet build --no-restore --configuration Release
dotnet test --no-build --configuration Release
```

For frontend changes, use the package lock and run the relevant client build:

```bash
npm ci
npm run build
```

For acceptance tests, build first and install the Playwright Chromium browser using the generated `playwright.ps1` script, matching `.github/workflows/build.yml`.

## Completion checklist

- The change respects the project dependency boundaries and accepted ADRs.
- New behaviour has tests at the appropriate level.
- Template directives and all affected variants remain valid.
- Formatting, build, and relevant tests pass without introducing warnings.
- Documentation and ADRs are updated when behaviour or architecture changes.
