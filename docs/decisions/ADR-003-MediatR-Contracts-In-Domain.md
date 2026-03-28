# ADR-003: MediatR.Contracts Reference in Domain

## Status

Accepted

## Date

2026-03-16

## Context

Domain events need to integrate with MediatR's pub/sub system, which is used in the Application layer to dispatch events after `SaveChangesAsync`. `BaseEvent` must satisfy MediatR's `INotification` marker interface for this to work. Clean Architecture requires the Domain layer to be free of external dependencies so that the domain model can evolve independently of frameworks and infrastructure.

## Decision

Reference `MediatR.Contracts` in the Domain project. `BaseEvent` implements `INotification` directly, making domain events first-class MediatR notifications without any adapter or mapping layer in the Application layer.

## Rationale

The alternative is to define a local marker interface (e.g. `IDomainEvent`) in Domain and adapt it to `INotification` in the Application layer before publishing through MediatR. This preserves the "zero external dependencies in Domain" ideal but adds an adapter that exists solely to paper over a seam between two interfaces with identical semantics.

`MediatR.Contracts` contains only interface definitions — no implementation, no transitive dependencies, stable across MediatR major versions. The coupling is real but minimal, and it eliminates an entire conversion layer that would otherwise need to be written and maintained.

## Consequences

**Easier:**
- Domain events are directly publishable via `IMediator.Publish()` with no conversion step.
- If MediatR is ever replaced, `BaseEvent` and one package reference change — a single, contained update.

**Harder:**
- Domain.csproj has one NuGet dependency (`MediatR.Contracts`). The "zero NuGet dependencies" ideal is intentionally relaxed.
