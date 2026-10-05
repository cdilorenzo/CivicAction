# Source ingestion

## Roadmap direction

The source-ingestion roadmap calls for importing real Berlin civic actions through a normalized `ICivicActionSource` boundary, beginning with a feasibility investigation for meinBerlin. A planned connector is not evidence that the source has a stable or permitted API.

## Feasibility before integration

Before building a live connector, document:

- Available API/feed and access conditions.
- Stable identifiers and change-detection options.
- Availability and meaning of deadlines, location/jurisdiction, detail content, and official participation links.
- Rate limits, attribution, caching, and other applicable access requirements.
- Failure and update behavior, including source removals or corrections.
- A viable alternative if no stable and permitted integration exists.

The meinBerlin investigation (SRC-0) is recorded in [sources/meinberlin-feasibility.md](sources/meinberlin-feasibility.md), and its access decision in [ADR-0003](adr/0003-meinberlin-source-access-path.md).

## Connector expectations

- Keep source-specific parsing and transport in an adapter behind the normalized source boundary.
- Map external records to explicit internal contracts; validate required values and surface malformed or partial data.
- Keep fixtures representative and deterministic. Tests must not depend on a live external source.
- Preserve provenance and an official participation link where the contract requires them.
- Do not silently fabricate, drop, or reinterpret source facts. Surface connector errors through the repository's normal logging and monitoring mechanisms.

The port's exact contract, refresh strategy, persistence, and live-source policy remain subject to their own slices and ADRs.
