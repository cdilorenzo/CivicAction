# Participation domain model

## Roadmap direction

The Core Participation roadmap identifies `Participation` as a bounded context and `CivicAction` as the intended primary aggregate. [ADR-0001](adr/0001-participation-bounded-context-and-aggregate.md) now establishes the ubiquitous language and aggregate boundaries before production domain behavior is added.

The accepted boundary is recorded in [ADR-0001](adr/0001-participation-bounded-context-and-aggregate.md). The minimal `CivicAction` schema and its invariants are recorded under [CivicAction aggregate](#civicaction-aggregate). Lifecycle states and persistence are not defined yet.

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

## CivicAction aggregate

Implemented in `CivicAction.Domain` (namespace `CivicAction.Domain.Participation`) by ACT-1 (#16). The field selection rests on the data availability found in [meinBerlin source feasibility](sources/meinberlin-feasibility.md) ([ADR-0003](adr/0003-meinberlin-source-access-path.md)). The model is source-neutral: limits of a source belong to its connector, not to the domain.

An instance can only be created through `CivicAction.Create`, which rejects every violation with an `ArgumentException`. It never corrects, truncates, or replaces a value; trimming surrounding whitespace is the only normalization. Lengths count UTF-16 characters. The aggregate has no mutating members: a re-import builds a new instance with the same `Id` and `ExternalIdentity`.

| Field | Required | Rule | M1 use |
| --- | --- | --- | --- |
| `Id` (`CivicActionId`) | yes | `Guid`, not empty; assigned by the caller | identity in every slice |
| `ExternalIdentity` | yes | `SourceKey` (1–64 characters from `a-z`, `0-9`, `-`) plus `ExternalId` (trimmed, 1–128 characters); equality is ordinal | idempotent re-import |
| `Title` | yes | trimmed, 1–200 characters, plain text | card, detail |
| `OfficialParticipationRoute` | yes | absolute `https` URI, no user information, at most 2048 characters | handoff to the official participation |
| `ParticipationWindow` | yes | optional `Start` and `End` (`DateTimeOffset`, offset kept), at least one of them, `Start` ≤ `End`; `IsOpenAt(instant)` uses the interval `[Start, End)` | card (deadline), ranking, "open" |
| `ParticipationType` | yes | closed enumeration: `Unknown`, `IdeaCollection`, `Survey`, `TextDiscussion`, `Event`, `DevelopmentPlanConsultation` | card, effort policy (ACT-3) |
| `Summary` | no | if present: trimmed, 1–500 characters, plain text | card, detail |
| `Locality` | no | closed enumeration: the 12 Berlin districts and `CityWide`; missing means unknown | card, locality fit (ACT-3) |
| `ResponsibleOrganization` | no | if present: trimmed, 1–200 characters, a name without identifier | card, detail |

Decisions behind the schema:

- **Identity.** `ExternalIdentity` replaces a separate source reference. Retrieval metadata such as hashes and fetch times belongs to ingestion, not to the aggregate.
- **Window.** meinBerlin running projects have no start and upcoming projects have no end, so both bounds are optional. A window with neither bound cannot say whether participation is open and is rejected. There is no separate deadline: it is the end of the window. The end is exclusive, so a window with `Start` = `End` is valid but never open (offline events). Offsets are kept as delivered; conversion to Europe/Berlin for display belongs to the presentation.
- **Open.** "Open" is derived from the window and a time the caller supplies (`TimeProvider` in the application). It is not stored.
- **Participation type.** meinBerlin's API does not deliver it, so those records use `Unknown` until that is clarified (#43).
- **Locality.** It means a district or the whole city. It is not the street address of a project location, which is not ingested. "Gesamtstädtisch" of meinBerlin maps to `CityWide`.
- **Text.** `Title`, `Summary`, and `ResponsibleOrganization` are untrusted plain text. The domain does not interpret markup; output encoding belongs to the presentation, and detecting contact details in free text to the connector.

Not part of the aggregate: `EstimatedMinutes`, ranking values, `Jurisdiction`, a stored status or lifecycle, explanations, update timestamps, `Deadline`, geodata, topics, images, contact data, source documents, and any person-related data.
