# ADR-001: Use EF Core in the Application Layer

## Status

Accepted

## Date

2024-02-29

## Context

The Application layer needs to read and write data. Clean Architecture restricts inner layers from depending on outer layers, and frameworks like ORMs are typically placed in the Infrastructure layer. The question is whether the Application layer should access EF Core directly through an interface it owns, or whether a repository layer should sit between them.

## Decision

The Application layer defines `IApplicationDbContext` — an interface exposing `DbSet<T>` properties — and uses it directly in command and query handlers. `ApplicationDbContext` in the Infrastructure layer implements this interface. No repository layer sits between the Application layer and EF Core.

## Rationale

### Dependency inversion is satisfied

`IApplicationDbContext` is defined in the Application layer. `ApplicationDbContext` in Infrastructure implements it. The dependency arrow points inward — Infrastructure depends on Application, not the other way around.

The Application layer takes a compile-time dependency on EF Core's abstractions (`DbSet<T>`, `IQueryable<T>`) but has no knowledge of the concrete `DbContext`, the database provider, or any Infrastructure type. A reference to an assembly (`Microsoft.EntityFrameworkCore`) is not the same as a dependency on a concrete implementation. Dependency inversion is about the direction of the dependency arrow, not about achieving zero framework references in inner layers.

### Repositories add indirection without meaningful abstraction

The alternative is to define repository interfaces in the Application layer and implement them in Infrastructure. In practice, this does not remove the coupling to EF Core — it hides it. Repository implementations still use EF Core internally, and the Application layer still needs to express intent in ORM-relevant terms: which related entities to load, how to filter, how to project. These concerns surface through the repository interface as custom methods (`GetByIdWithItems`, `GetActiveOrderedByName`, etc.) that mirror EF Core query patterns without using EF Core syntax.

The result is an extra layer of indirection that increases complexity and reduces discoverability. `IApplicationDbContext` with `DbSet<T>` is more honest about what is actually happening.

### Testing against a real database is preferred

Functional tests use Aspire to spin up a real database and Respawn to reset state between tests. Testing against the actual database provider catches issues that in-memory fakes and mocked repositories cannot: provider-specific query behaviour, index violations, transaction semantics, and constraint enforcement. This is the [approach recommended by Microsoft](https://learn.microsoft.com/en-us/ef/core/testing/choosing-a-testing-strategy).

Unit tests for pure domain logic and Application-layer validation do not require database access at all.

### ORM replaceability is YAGNI

A common argument for abstracting EF Core is that it keeps the Application layer portable — if you need to swap the ORM later, only the Infrastructure layer changes. In practice, projects that adopt EF Core for greenfield development do not replace it. Designing the entire Application layer around a scenario that almost never occurs adds real complexity now to solve a theoretical problem later.

## Consequences

**Easier:**
- Application handlers can use the full expressiveness of EF Core — projections, includes, raw SQL, compiled queries — without workarounds or leaking intent through a repository interface.
- There is no repository layer to design, implement, or keep in sync as handler requirements evolve.
- The data access path is straightforward to follow: handler -> `IApplicationDbContext` -> `ApplicationDbContext`.

**Harder:**
- Functional tests must run against a real database. There is no lightweight substitute.
- Switching the ORM would require changes to Application handlers, not just Infrastructure. This is an accepted tradeoff given how rarely it occurs in practice.

## When this decision does not apply

This ADR applies to the template's default approach, which does not prescribe DDD. If you are combining Clean Architecture with Domain-Driven Design, the repository pattern is the right choice — but for different reasons than persistence abstraction.

In a DDD model, repositories are a domain concept. They are defined in the domain layer in terms of aggregates, not in terms of persistence mechanics. The domain layer defines `IOrderRepository` with methods like `GetById`, `Save`, and `FindByCustomer` — expressed in the language of the domain, not in the language of EF Core. The Infrastructure layer implements those interfaces. This is fundamentally different from wrapping `DbContext` in a repository to hide it from the Application layer, which is the approach this ADR argues against.

If your project warrants DDD — a rich domain model, aggregate boundaries, domain services, and invariants enforced within the domain — then repositories belong in that model, and `IApplicationDbContext` should not be used directly in handlers.
