# ADR-0001: Participation bounded context and aggregate

- Status: Accepted
- Date: 2026-10-02
- Issue: #15

## Context

The Core Participation roadmap identifies `Participation` as a bounded context and `CivicAction` as its intended primary aggregate. The product's initial action loop discovers civic opportunities and hands people off to an official participation route. The bounded context must distinguish the source-derived action from derived presentation data, private social coordination, and any record of a person's participation.

Before this decision, the domain model did not define aggregate invariants or boundaries. The proposals that `CivicAction` is the primary aggregate, `Explanation` is derived rather than domain truth, and `Circle` is outside the `CivicAction` aggregate therefore required explicit evaluation.

## Decision

1. `Participation` is the bounded context for the product's civic actions and their official participation routes. It does not own the external participation service or imply that CivicAction records a person's participation.
2. `CivicAction` is the primary aggregate. It represents a globally discoverable civic opportunity derived from an external source. Source records are input; connector-specific transport and parsing remain outside the domain. This decision does not prescribe a complete field schema or lifecycle.
3. An `Explanation` and ranking information are derived presentation data, not authoritative `CivicAction` state. Source-specific details remain outside the aggregate unless a later accepted decision establishes a domain rule for them.
4. A `Circle` is a distinct aggregate for a private, invitation-based group used for social coordination around civic participation. It does not own a `CivicAction`. It may share/reference an existing `CivicAction` through a conceptual `SharedAction` relationship, but may not create, delete, or change the action's content or status. Exact fields and behavior of Circle membership, invitations, messages, and shared-action records are left to a separately scoped Circle decision.
5. A person's participation completion, if considered in future work, is not state owned by a `Circle`. This ADR does not approve recording identifiable completion events, activity summaries, or any related personal data. Their purpose, data model, access, retention/deletion, notice/legal basis, and re-identification risks require a separate product and privacy decision before collection or implementation.
6. Circle functionality is excluded from the initial ACT/SRC MVP flow. Any Circle work requires its own approved scope after the core source-to-action-to-official-route flow is established.

The three proposals are therefore accepted with the boundaries and qualifications above. This ADR makes no persistence, identity, tracking, or analytics decision.

## Consequences

- `CivicAction` remains independent of a Circle and of the external service through which a person participates.
- A future Circle aggregate can refer to `CivicAction` without duplicating or mutating its domain state.
- Explanations and ranking can change without changing the authoritative action.
- No user-level participation history or Circle activity summary may be inferred as an implementation requirement from this ADR. Any such proposal needs a separate privacy review and approved issue.
- Circle functionality must not be added to the initial ACT/SRC MVP slices; its requirements and any data handling need separate approval.
- Domain implementation must not invent fields, lifecycle states, or invariants beyond this decision and a subsequently approved slice.

## Alternatives considered

- **Make `Circle` part of the `CivicAction` aggregate:** rejected because a private group's membership and sharing lifecycle is independent of the globally discoverable action, and would give the group an inappropriate ownership boundary.
- **Let a Circle own a copy of a CivicAction:** rejected because the Circle could then diverge from the source-derived action or appear to change its authoritative status. A reference preserves the ownership boundary.
- **Treat explanations or ranking as authoritative action state:** rejected because they are derived presentation data, not source-independent domain truth.
- **Treat group activity as the owner of individual completion:** rejected. A group is not the owner of a person's participation, and no collection or aggregation of individual completion data is approved here.
