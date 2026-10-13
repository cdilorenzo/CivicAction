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

## Shared building blocks

Product-independent technical building blocks that CivicAction shares with the owner's other product, TuWasGutes, come from the repository `cdilorenzo/BuildingBlocks` as NuGet packages. The decision, the criteria for adding a shared package and the supply-chain consequences are in [ADR-0002](adr/0002-shared-components-repository.md). These rules apply:

- CivicAction never references TuWasGutes: no project, package or path reference to its repository. Naming TuWasGutes in documentation, such as ADR-0002 or the [reuse assessment](reuse/tuwasgutes-reuse-matrix.md), is not a reference.
- `CivicAction.Domain` references no shared package.
- Shared code enters CivicAction only as a versioned `DiLorenzo.BuildingBlocks.*` package from nuget.org, pinned to an exact version with NuGet's exact-match notation, for example `Version="[1.0.0]"`. A plain `Version="1.0.0"` is only a minimum version and is not allowed for shared packages.
- Restore works without credentials and without any package source besides nuget.org. Do not commit a `nuget.config` that adds a local, private or authenticated source; try unreleased package builds only with an uncommitted local source.
- Do not commit path references or `ProjectReference`s to projects in other repositories.
- Each adoption of a shared package is its own slice. That slice evaluates the review triggers in [005-security.md](005-security.md).
- Prefer first-party .NET libraries and APIs over custom wrappers, in CivicAction and in shared packages.

Shared packages contain no product terms, domain types, product data, database schema or identity model.

## Decisions

This document describes the intended starting shape, not a complete architecture. Open questions must be resolved by scoped issues and recorded in [021-adr-index.md](021-adr-index.md); do not assume a database, hosting model, or deployment topology.
