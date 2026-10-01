# Architecture

## Current repository shape

The solution targets .NET 10 and separates responsibilities into these projects:

| Project | Intended responsibility |
| --- | --- |
| `CivicAction.Domain` | Domain concepts and invariants |
| `CivicAction.Application` | Use cases and application coordination |
| `CivicAction.Infrastructure` | External systems and persistence adapters |
| `CivicAction.Api` | HTTP/API host |
| `CivicAction.Web` | Web/PWA host and user experience |
| `CivicAction.Localization` | Shared localization resources and support |

Each project has a corresponding test project. The current code is a skeleton; these intended responsibilities do not imply that the boundaries are already enforced or that product functionality exists.

## Dependency direction

Keep domain rules independent of hosting, storage, UI frameworks, and source-specific SDKs. Application use cases coordinate domain behavior through explicit boundaries. Infrastructure implements those boundaries. Hosts compose the application and expose it to users or clients.

Do not add a project reference that reverses this direction without an accepted ADR. Prefer the narrowest dependency that expresses the required contract.

## Integration boundaries

- Source-specific integration belongs behind a normalized source boundary. See [007-source-ingestion.md](007-source-ingestion.md).
- Official participation remains an external handoff unless an approved product and architecture decision establishes otherwise.
- Cross-project contracts that affect persistence, APIs, security, or deployment should be documented and, when lasting, recorded in an ADR.

## Decisions

This document describes the intended starting shape, not a complete architecture. Open questions must be resolved by scoped issues and recorded in [021-adr-index.md](021-adr-index.md); do not assume a database, hosting model, or deployment topology.
