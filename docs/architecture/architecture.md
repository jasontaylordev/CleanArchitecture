# Architecture

> **Authority:** This file is the authoritative, human-reviewed architecture record.
>
> **State semantics:** Baseline is currently implemented or operating.
> Transition is approved temporary or intermediate architecture. Target is
> approved future intent. Implementation alone is not evidence of target intent.
> LITTLE UPDATE

## Source repositories

| Source ID | Repository | Role | Configured ref | Resolved commit |
|---|---|---|---|---|
| SRC-001 | EdgarAlvarez10/CleanArchitecture | primary | main | 959071aeff638120d77bc4dd79f421e6510b1aca |

## Drivers and constraints

### ARC-DRV-001 — .NET 10 clean architecture baseline

- Description: The solution is a .NET 10 Clean Architecture template with centralized package management, nullable enabled, and warnings treated as errors.
- Evidence: `global.json`, `Directory.Build.props`, `Directory.Packages.props`, `README.md`.

### ARC-DRV-002 — Template-gated deployment shape

- Description: Aspire orchestration, database provider selection, and frontend selection are all compile-time/template gated rather than fixed by the collected evidence.
- Evidence: `CleanArchitecture.slnx`, `src/AppHost/Program.cs`, `src/Web/Web.csproj`, `src/Infrastructure/Infrastructure.csproj`.

## Solution context

### ARC-CTX-001 — Layered multi-project solution

- Description: The solution is split into AppHost, ServiceDefaults, Application, Domain, Infrastructure, Shared, and Web projects, with test projects listed in the solution but excluded from the bootstrap scope.
- Evidence: `CleanArchitecture.slnx`.

## Components and responsibilities

### ARC-CMP-001 — AppHost orchestrates the runtime

- Responsibility: Build the Aspire application, configure the database resource, reference the Web project, set external HTTP endpoints, and optionally launch a JavaScript frontend in run mode.
- Owning repository: EdgarAlvarez10/CleanArchitecture
- Evidence: `src/AppHost/Program.cs`, `src/AppHost/AppHost.csproj`, `src/Shared/Services.cs`.

### ARC-CMP-002 — Web exposes the HTTP boundary

- Responsibility: Host Minimal API endpoint groups, ASP.NET Core Identity endpoints, OpenAPI/Scalar, CORS, file serving, and exception handling.
- Owning repository: EdgarAlvarez10/CleanArchitecture
- Evidence: `src/Web/Program.cs`, `src/Web/DependencyInjection.cs`, `src/Web/Endpoints/TodoLists.cs`, `src/Web/Endpoints/TodoItems.cs`, `src/Web/Endpoints/Users.cs`, `src/Web/Infrastructure/*.cs`.

### ARC-CMP-003 — Application implements use cases

- Responsibility: Contain MediatR handlers, validators, mapping profiles, and request pipeline behaviors for logging, authorization, validation, performance, and exception logging.
- Owning repository: EdgarAlvarez10/CleanArchitecture
- Evidence: `src/Application/DependencyInjection.cs`, `src/Application/Common/Behaviours/*.cs`, `src/Application/TodoLists/**/*.cs`, `src/Application/TodoItems/**/*.cs`, `src/Application/WeatherForecasts/**/*.cs`.

### ARC-CMP-004 — Infrastructure owns persistence and identity

- Responsibility: Implement the DbContext, EF Core entity configuration, save-change interceptors, database initialization and seeding, and identity service adapters.
- Owning repository: EdgarAlvarez10/CleanArchitecture
- Evidence: `src/Infrastructure/DependencyInjection.cs`, `src/Infrastructure/Data/*.cs`, `src/Infrastructure/Identity/*.cs`.

### ARC-CMP-005 — Domain holds the core model

- Responsibility: Define the todo aggregates, value objects, domain event, role constants, and entity base types.
- Owning repository: EdgarAlvarez10/CleanArchitecture
- Evidence: `src/Domain/**/*.cs`.

### ARC-CMP-006 — ServiceDefaults centralizes Aspire defaults

- Responsibility: Configure service discovery, HTTP resilience, OpenTelemetry, and development health checks.
- Owning repository: EdgarAlvarez10/CleanArchitecture
- Evidence: `src/ServiceDefaults/Extensions.cs`.

### ARC-CMP-007 — Shared holds resource names

- Responsibility: Provide shared service and database identifiers used by AppHost and dependent projects.
- Owning repository: EdgarAlvarez10/CleanArchitecture
- Evidence: `src/Shared/Services.cs`.

## Repository ownership of components

| Component ID | Repository | Ownership |
|---|---|---|
| ARC-CMP-001 | EdgarAlvarez10/CleanArchitecture | Implementation owner |
| ARC-CMP-002 | EdgarAlvarez10/CleanArchitecture | Implementation owner |
| ARC-CMP-003 | EdgarAlvarez10/CleanArchitecture | Implementation owner |
| ARC-CMP-004 | EdgarAlvarez10/CleanArchitecture | Implementation owner |
| ARC-CMP-005 | EdgarAlvarez10/CleanArchitecture | Implementation owner |
| ARC-CMP-006 | EdgarAlvarez10/CleanArchitecture | Implementation owner |
| ARC-CMP-007 | EdgarAlvarez10/CleanArchitecture | Implementation owner |

## Interfaces and integrations

### ARC-INT-001 — MediatR request and notification boundary

- Producer: Web endpoint handlers and other request senders.
- Consumer: Application handlers and notification handlers.
- Contract: HTTP handlers call `ISender.Send`; domain events are published through MediatR notifications.
- Evidence: `src/Web/Endpoints/*.cs`, `src/Application/DependencyInjection.cs`, `src/Infrastructure/Data/Interceptors/DispatchDomainEventsInterceptor.cs`.

### ARC-INT-002 — Endpoint-group route discovery

- Producer: `IEndpointGroup` implementations in Web.
- Consumer: `WebApplicationExtensions.MapEndpoints`.
- Contract: route groups are discovered by reflection and mapped with names derived from handler methods.
- Evidence: `src/Web/Infrastructure/IEndpointGroup.cs`, `src/Web/Infrastructure/WebApplicationExtensions.cs`, `src/Web/Infrastructure/EndpointRouteBuilderExtensions.cs`.

### ARC-INT-003 — Identity and OpenAPI integration

- Producer: ASP.NET Core Identity APIs and Web endpoint metadata.
- Consumer: Browser or API clients through Scalar/OpenAPI.
- Contract: identity routes are generated by `MapIdentityApi`, documented by OpenAPI transformers, and protected by auth metadata.
- Evidence: `src/Web/Endpoints/Users.cs`, `src/Web/Infrastructure/IdentityApiOperationTransformer.cs`, `src/Web/Infrastructure/BearerSecuritySchemeTransformer.cs`.

### ARC-INT-004 — EF Core context contract

- Producer: Infrastructure implements `ApplicationDbContext`.
- Consumer: Application code depends on `IApplicationDbContext`.
- Contract: Application uses the abstraction for todo and identity data access; Infrastructure supplies the concrete DbContext and provider-specific configuration.
- Evidence: `src/Application/Common/Interfaces/IApplicationDbContext.cs`, `src/Infrastructure/Data/ApplicationDbContext.cs`, `src/Infrastructure/DependencyInjection.cs`.

### ARC-INT-005 — Aspire defaults and service discovery

- Producer: ServiceDefaults.
- Consumer: AppHost and Web projects.
- Contract: OpenTelemetry, health checks, service discovery, and HTTP resilience are attached to host builders by convention.
- Evidence: `src/ServiceDefaults/Extensions.cs`, `src/AppHost/Program.cs`, `src/Web/Program.cs`.

## Cross-repository relationships

This bootstrap manifest includes one repository only, so cross-repository relationships are not evidenced. Internal project-to-project relationships are:

- AppHost composes Web and database resources.
- Web references Application, Infrastructure, and ServiceDefaults.
- Application references Domain.
- Infrastructure references Application and Shared.
- Domain references only MediatR.Contracts.

## Data ownership

### ARC-DATA-001 — Todo and identity data are owned by Infrastructure

- Data: Todo lists, todo items, application users, roles, and audit fields.
- Owner: Infrastructure via `ApplicationDbContext` and `IdentityDbContext<ApplicationUser>`.
- Readers: Application handlers, Web endpoints, identity services, and seed/initialization logic.
- Evidence: `src/Infrastructure/Data/ApplicationDbContext.cs`, `src/Infrastructure/Data/Configurations/TodoListConfiguration.cs`, `src/Infrastructure/Data/Configurations/TodoItemConfiguration.cs`, `src/Infrastructure/Data/ApplicationDbContextInitialiser.cs`, `src/Application/Common/Interfaces/IApplicationDbContext.cs`.

## Security and trust boundaries

### ARC-SEC-001 — Identity-based authentication and authorization

- Description: Web derives the current user from `HttpContext`, Application enforces request authorization with a custom `[Authorize]` attribute and MediatR pipeline behavior, and Infrastructure configures cookie or bearer authentication depending on template mode.
- Evidence: `src/Web/Services/CurrentUser.cs`, `src/Application/Common/Security/AuthorizeAttribute.cs`, `src/Application/Common/Behaviours/AuthorizationBehaviour.cs`, `src/Infrastructure/DependencyInjection.cs`.

### ARC-SEC-002 — Optional secret store integration

- Description: Web can add Azure Key Vault configuration when `AZURE_KEY_VAULT_ENDPOINT` is present.
- Evidence: `src/Web/DependencyInjection.cs`.

## Deployment architecture

### ARC-DEP-001 — Aspire-hosted web and database topology

- Description: AppHost runs the Web API plus a database resource, can attach an Azure Container App environment, and can launch a JavaScript frontend in run mode. The Web project is configured for external HTTP endpoints.
- Evidence: `src/AppHost/Program.cs`, `src/AppHost/AppHost.csproj`, `src/Web/Program.cs`.

## Operational architecture

### ARC-OPS-001 — Startup initialization and seeding

- Description: In Development, Web initializes the database by deleting and recreating it, then seeds default roles, an administrator user, and starter todo data.
- Evidence: `src/Web/Program.cs`, `src/Infrastructure/Data/ApplicationDbContextInitialiser.cs`.

### ARC-OPS-002 — Observability, error handling, and performance logging

- Description: Service defaults enable OpenTelemetry, service discovery, and health checks; Web maps RFC 9110 ProblemDetails responses for known exceptions; Application logs requests, long-running operations, and unhandled exceptions; infrastructure dispatches domain events on save changes.
- Evidence: `src/ServiceDefaults/Extensions.cs`, `src/Web/Infrastructure/ProblemDetailsExceptionHandler.cs`, `src/Application/Common/Behaviours/LoggingBehaviour.cs`, `src/Application/Common/Behaviours/PerformanceBehaviour.cs`, `src/Application/Common/Behaviours/UnhandledExceptionBehaviour.cs`, `src/Infrastructure/Data/Interceptors/DispatchDomainEventsInterceptor.cs`.

## Baseline architecture

### ARC-BASE-001 — Current implemented state

Status: implemented

Description: The current implemented state is a single-repository Clean Architecture solution with Aspire orchestration, a Web HTTP boundary, an Application use-case layer, Infrastructure persistence and identity, a Domain model for todo behavior, and shared host identifiers. The collected evidence also shows both Angular and React client assets, but only the Angular ClientApp path is wired into AppHost run mode.

Evidence: `CleanArchitecture.slnx`, `src/AppHost/Program.cs`, `src/Web/Program.cs`, `src/Application/DependencyInjection.cs`, `src/Infrastructure/DependencyInjection.cs`, `src/Domain/**/*.cs`.

## Transition architecture

### ARC-TRANS-001 — No approved transition currently evidenced

Status: unknown

Do not replace this with inferred implementation intent.

## Target architecture

### ARC-TARGET-001 — No approved target currently evidenced

Status: unknown

Do not infer a target from current implementation.

## Architecture decisions

### ARC-DEC-001 — Keep HTTP composition in Web and use-case logic in Application

- Status: proposed
- Context: Web endpoint groups are thin, and Application owns handlers, validation, authorization, and mapping.
- Decision: Continue to use endpoint-group discovery and MediatR handlers as the primary composition pattern for requests.
- Consequences: HTTP wiring remains isolated, cross-cutting behaviors stay centralized, and use cases remain testable without the HTTP stack.
- Approval evidence: none collected; this is a proposed synthesis from implementation evidence only.

### ARC-DEC-002 — Keep Aspire as the runtime orchestrator

- Status: proposed
- Context: AppHost already composes the Web project, database resource, service discovery, and optional frontend startup.
- Decision: Treat AppHost as the canonical local/runtime composition entrypoint.
- Consequences: Local execution, resource wiring, and external endpoint exposure stay centralized in one executable host.
- Approval evidence: none collected; this is a proposed synthesis from implementation evidence only.

## Contradictions

### ARC-CON-001 — Frontend runtime path is ambiguous

The collected evidence shows an Angular ClientApp path launched by AppHost, but the repository also contains a React ClientApp-React tree. The evidence does not show the React tree wired into the current runtime path.

Affected repositories: EdgarAlvarez10/CleanArchitecture

## Unknowns

### ARC-UNK-001 — Active database provider

Which database provider is active in this checkout: SQLite, PostgreSQL, or SQL Server?

### ARC-UNK-002 — React client intent

Is the React client tree an alternate generated frontend, a dormant artifact, or intended future runtime input?

### ARC-UNK-003 — Additional repositories

Are there any supporting repositories or external services beyond the single primary repository in the manifest?
