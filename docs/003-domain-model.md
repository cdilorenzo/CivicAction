# Participation domain model

## Roadmap direction

The Core Participation roadmap identifies `Participation` as a bounded context and `CivicAction` as the intended primary aggregate. The domain decision slice is responsible for establishing the ubiquitous language and the aggregate's invariants and boundaries before production domain behavior is added.

The name alone is not a complete model. This document intentionally does not define fields, lifecycle states, identity rules, or invariants that have not been accepted.

## Modeling rules

- Keep the domain language aligned with terms in [010-terms.md](010-terms.md).
- Record domain decisions and their rationale in an accepted ADR, and link that ADR below.
- Treat imported source data as input to the domain, not as domain truth without an explicit rule.
- Keep derived explanations, ranking data, and source-specific details outside the aggregate unless a domain decision establishes otherwise.

## Decision record

- `ADR-0001` is reserved for the Participation bounded-context and aggregate-boundary decision. Add a link here when it is accepted.

Until that decision is accepted, implementation must follow an explicitly approved slice specification and must not create undocumented domain invariants.
