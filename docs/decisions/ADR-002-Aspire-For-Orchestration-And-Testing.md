# ADR-002: Aspire for Orchestration and Testing

## Status

Accepted

## Date

2026-03-12

## Context

Before Aspire, the template had no built-in solution for orchestrating the full application stack. Running `Application.FunctionalTests` required a database server to be installed and a connection string manually configured. Running `Web.AcceptanceTests` required the developer to manually launch the database, the backend, and the frontend dev server before tests could run. The template also provided no opinionated baseline for observability, service discovery, or HTTP resilience.

## Decision

[Aspire](https://aspire.dev) is included by default in all solution variants as the orchestration layer for local development and testing. Two projects are provided: `AppHost`, which orchestrates the full stack for local development and acceptance testing, and `TestAppHost`, which orchestrates only the database for use by `Application.FunctionalTests`.

## Rationale

The alternatives considered were:

- **No orchestration** — the previous approach. Each developer installs and configures dependencies manually. Tests require external setup before they can run. This was the single biggest source of "getting started" friction.
- **Docker Compose** — handles container orchestration but provides no observability, no service discovery, no health check integration, and no programmatic control from test code. It also requires maintaining a separate YAML configuration outside the .NET solution.
- **Testcontainers directly** — good for spinning up database containers in tests, but limited to test scenarios. It does not help with local development orchestration, observability, or service defaults.

Aspire covers all of these: it orchestrates the full stack for local development, provides programmatic control for test setup, and includes production-ready defaults for observability (OpenTelemetry), service discovery, HTTP resilience, and health checks — all configured automatically via the `ServiceDefaults` project.

## Consequences

**Easier:**
- `dotnet test` is self-contained — no database installation, no manual connection strings, no external processes to launch.
- Local development gets a dashboard, structured logging, distributed tracing, and health checks out of the box.
- Services reference each other by name rather than hardcoded URLs or ports.

**Harder:**
- Aspire is a significant dependency that couples the template to a Microsoft-specific orchestration framework.
- Docker (or Podman) is required for the PostgreSQL and SQL Server variants. The SQLite variant (the default) does not require Docker.
