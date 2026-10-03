# Participation domain model

## Roadmap direction

The Core Participation roadmap identifies `Participation` as a bounded context and `CivicAction` as the intended primary aggregate. [ADR-0001](adr/0001-participation-bounded-context-and-aggregate.md) now establishes the ubiquitous language and aggregate boundaries before production domain behavior is added.

The accepted boundary and invariants are recorded in [ADR-0001](adr/0001-participation-bounded-context-and-aggregate.md). The name alone is not a complete model; this document does not define fields, lifecycle states, identity rules, or invariants beyond that decision.

## Modeling rules

- Keep the domain language aligned with terms in [010-terms.md](010-terms.md).
- Record lasting domain decisions and their rationale in an accepted ADR, and link that ADR below.
- Treat imported source data as input to the domain, not as domain truth without an explicit rule.
- Keep derived explanations, ranking data, and source-specific details outside the aggregate unless a domain decision establishes otherwise.

## Accepted boundaries

- `Participation` owns the product's civic-action domain concepts and official participation-route references. The actual participation service remains external.
- `CivicAction` is the primary aggregate for a globally discoverable, externally sourced civic opportunity. This decision does not prescribe a complete field schema or lifecycle.
- A `Circle` is a separate aggregate for private, invitation-based social coordination. It may reference/share an existing `CivicAction`, but cannot create, delete, or modify its content or status. Circle behavior is deferred beyond the initial ACT/SRC MVP.
- Explanations and ranking are derived presentation data, not authoritative `CivicAction` state.
- A person's participation completion is not Circle-owned state. Recording individual completion or producing activity summaries is not approved by ADR-0001 and requires a separate product and privacy decision before collection or implementation.

Do not introduce fields, lifecycle states, identity rules, persistence, or additional invariants unless an approved slice and, where relevant, an accepted ADR establish them.
