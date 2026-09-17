---
schema_version: 1
authority: index-only
generated: true
authoritative_source: architecture.md
---

# Architecture map

> This file is a generated retrieval index. It is not authoritative for architecture claims.

**Index-only declaration:** This file contains index entries only and must not introduce new architecture claims.

## Lifecycle

| State | Status | Authoritative section |
|---|---|---|
| Baseline | Implemented | [ARC-BASE-001](architecture.md#arc-base-001-current-implemented-state) |
| Transition | Unknown | [ARC-TRANS-001](architecture.md#arc-trans-001-no-approved-transition-currently-evidenced) |
| Target | Unknown | [ARC-TARGET-001](architecture.md#arc-target-001-no-approved-target-currently-evidenced) |

## Drivers and constraints

| ID | Lifecycle | Topic | Source |
|---|---|---|---|
| ARC-DRV-001 | Baseline | .NET 10 clean architecture baseline | Source | [Open](architecture.md#arc-drv-001-net-10-clean-architecture-baseline) |
| ARC-DRV-002 | Baseline | Template-gated deployment shape | Source | [Open](architecture.md#arc-drv-002-template-gated-deployment-shape) |

## Context

| ID | Lifecycle | Topic | Source |
|---|---|---|---|
| ARC-CTX-001 | Baseline | Layered multi-project solution | Source | [Open](architecture.md#arc-ctx-001-layered-multi-project-solution) |

## Components

| ID | Lifecycle | Topic | Source |
|---|---|---|---|
| ARC-CMP-001 | Baseline | AppHost orchestrates the runtime | Source | [Open](architecture.md#arc-cmp-001-apphost-orchestrates-the-runtime) |
| ARC-CMP-002 | Baseline | Web exposes the HTTP boundary | Source | [Open](architecture.md#arc-cmp-002-web-exposes-the-http-boundary) |
| ARC-CMP-003 | Baseline | Application implements use cases | Source | [Open](architecture.md#arc-cmp-003-application-implements-use-cases) |
| ARC-CMP-004 | Baseline | Infrastructure owns persistence and identity | Source | [Open](architecture.md#arc-cmp-004-infrastructure-owns-persistence-and-identity) |
| ARC-CMP-005 | Baseline | Domain holds the core model | Source | [Open](architecture.md#arc-cmp-005-domain-holds-the-core-model) |
| ARC-CMP-006 | Baseline | ServiceDefaults centralizes Aspire defaults | Source | [Open](architecture.md#arc-cmp-006-servicedefaults-centralizes-aspire-defaults) |
| ARC-CMP-007 | Baseline | Shared holds resource names | Source | [Open](architecture.md#arc-cmp-007-shared-holds-resource-names) |

## Interfaces and integrations

| ID | Lifecycle | Topic | Source |
|---|---|---|---|
| ARC-INT-001 | Baseline | MediatR request and notification boundary | Source | [Open](architecture.md#arc-int-001-mediatr-request-and-notification-boundary) |
| ARC-INT-002 | Baseline | Endpoint-group route discovery | Source | [Open](architecture.md#arc-int-002-endpoint-group-route-discovery) |
| ARC-INT-003 | Baseline | Identity and OpenAPI integration | Source | [Open](architecture.md#arc-int-003-identity-and-openapi-integration) |
| ARC-INT-004 | Baseline | EF Core context contract | Source | [Open](architecture.md#arc-int-004-ef-core-context-contract) |
| ARC-INT-005 | Baseline | Aspire defaults and service discovery | Source | [Open](architecture.md#arc-int-005-aspire-defaults-and-service-discovery) |

## Data

| ID | Lifecycle | Topic | Source |
|---|---|---|---|
| ARC-DATA-001 | Baseline | Todo and identity data are owned by Infrastructure | Source | [Open](architecture.md#arc-data-001-todo-and-identity-data-are-owned-by-infrastructure) |

## Security

| ID | Lifecycle | Topic | Source |
|---|---|---|---|
| ARC-SEC-001 | Baseline | Identity-based authentication and authorization | Source | [Open](architecture.md#arc-sec-001-identity-based-authentication-and-authorization) |
| ARC-SEC-002 | Baseline | Optional secret store integration | Source | [Open](architecture.md#arc-sec-002-optional-secret-store-integration) |

## Deployment and operations

| ID | Lifecycle | Topic | Source |
|---|---|---|---|
| ARC-DEP-001 | Baseline | Aspire-hosted web and database topology | Source | [Open](architecture.md#arc-dep-001-aspire-hosted-web-and-database-topology) |
| ARC-OPS-001 | Baseline | Startup initialization and seeding | Source | [Open](architecture.md#arc-ops-001-startup-initialization-and-seeding) |
| ARC-OPS-002 | Baseline | Observability, error handling, and performance logging | Source | [Open](architecture.md#arc-ops-002-observability-error-handling-and-performance-logging) |

## Decisions

| ID | Lifecycle | Topic | Source |
|---|---|---|---|
| ARC-DEC-001 | Proposed | Keep HTTP composition in Web and use-case logic in Application | Source | [Open](architecture.md#arc-dec-001-keep-http-composition-in-web-and-use-case-logic-in-application) |
| ARC-DEC-002 | Proposed | Keep Aspire as the runtime orchestrator | Source | [Open](architecture.md#arc-dec-002-keep-aspire-as-the-runtime-orchestrator) |

## Baseline architecture

| ID | Lifecycle | Topic | Source |
|---|---|---|---|
| ARC-BASE-001 | Implemented | Current implemented state | Source | [Open](architecture.md#arc-base-001-current-implemented-state) |

## Transition architecture

| ID | Lifecycle | Topic | Source |
|---|---|---|---|
| ARC-TRANS-001 | Unknown | No approved transition currently evidenced | Source | [Open](architecture.md#arc-trans-001-no-approved-transition-currently-evidenced) |

## Target architecture

| ID | Lifecycle | Topic | Source |
|---|---|---|---|
| ARC-TARGET-001 | Unknown | No approved target currently evidenced | Source | [Open](architecture.md#arc-target-001-no-approved-target-currently-evidenced) |

## Contradictions

| ID | Lifecycle | Topic | Source |
|---|---|---|---|
| ARC-CON-001 | Unknown | Frontend runtime path is ambiguous | Source | [Open](architecture.md#arc-con-001-frontend-runtime-path-is-ambiguous) |

## Unknowns

| ID | Lifecycle | Topic | Source |
|---|---|---|---|
| ARC-UNK-001 | Unknown | Active database provider | Source | [Open](architecture.md#arc-unk-001-active-database-provider) |
| ARC-UNK-002 | Unknown | React client intent | Source | [Open](architecture.md#arc-unk-002-react-client-intent) |
| ARC-UNK-003 | Unknown | Additional repositories | Source | [Open](architecture.md#arc-unk-003-additional-repositories) |

## Retrieval hints

| Keywords | Relevant identifiers |
|---|---|
| API, endpoint, MediatR, handler | ARC-INT-001, ARC-INT-002, ARC-CMP-003 |
| database, identity, EF Core | ARC-INT-004, ARC-DATA-001, ARC-CMP-004 |
| authentication, authorization, trust | ARC-SEC-001, ARC-SEC-002, ARC-CMP-002 |
| deployment, Aspire, startup | ARC-DEP-001, ARC-CMP-001, ARC-CMP-006 |
| logging, metrics, recovery | ARC-OPS-001, ARC-OPS-002 |
