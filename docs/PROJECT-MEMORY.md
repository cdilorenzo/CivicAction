# CivicAction project memory

This is the short orientation document for contributors and AI assistants. The numbered documents are the durable product and technical references; update those rather than letting this summary become a second specification.

## Product direction

CivicAction is being built around a Berlin action loop: a person chooses a time budget, sees a small set of relevant civic actions, opens a detail, and follows the official participation route. The current roadmap describes this as a responsive web/PWA experience backed by source connectors and a Participation domain.

The product must support privacy-safe participation measurement without political profiling. The analytics epic explicitly excludes raw text and party inference. See [004-data-and-privacy.md](004-data-and-privacy.md).

## Repository baseline

- .NET 10 solution: [../CivicAction.slnx](../CivicAction.slnx)
- Projects: Domain, Application, Infrastructure, Api, Web, and Localization, with corresponding test projects where applicable.
- Nullable reference types are enabled in the projects; .NET analyzers are enabled at `latest-recommended`; warnings are errors.
- The repository currently contains a buildable skeleton. Do not treat an empty project as evidence that product behavior or architecture is already implemented.

## Settled direction versus open decisions

- The `Participation` bounded context and primary `CivicAction` aggregate are established by [ADR-0001](adr/0001-participation-bounded-context-and-aggregate.md). The ADR also records the separate `Circle` boundary and defers any user-level participation data decision.
- The source-ingestion roadmap calls for a normalized `ICivicActionSource` boundary and begins with feasibility work for meinBerlin. No live source integration or source availability is implied by this plan.
- Other details remain open until documented in an accepted ADR or product decision. In particular, do not invent persistence, ranking, identity, retention, or deployment requirements.

## Source-of-truth order

1. Accepted ADRs for recorded technical decisions.
2. Approved product/architecture documents in the numbered [documentation map](000-documentation-map.md).
3. The linked issue and its acceptance criteria for the scope of a slice.
4. Existing code and tests for behavior that is actually implemented.

If these disagree, stop and raise the discrepancy; do not silently choose one. The memory is a summary, not an override of the source documents.

## Useful workflow

- Follow [022-slice-contract.md](022-slice-contract.md) and the issue's acceptance criteria.
- Start with the project's existing patterns, add focused tests, and run the applicable validation commands in [AGENTS.md](../AGENTS.md).
- Record architecture decisions in an ADR and link it from [021-adr-index.md](021-adr-index.md).
