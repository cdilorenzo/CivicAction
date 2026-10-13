# ADR-0002: Shared components repository

- Status: Accepted
- Date: 2026-10-13
- Issue: #41

## Context

CivicAction and TuWasGutes are separate .NET products by the same owner. TuWasGutes lives in the private repository `cdilorenzo/OneGood`; CivicAction is public. Both read external sources over HTTP and need to test that code without live services.

TuWasGutes already has a fake HTTP message handler for its client tests: 49 lines, used in five test files at commit `08b30b4`. CivicAction's SRC-2 connector must be tested against synthetic data and never against live sources ([ADR-0003](0003-meinberlin-source-access-path.md), [008-quality-and-testing.md](../008-quality-and-testing.md)), so it needs the same kind of helper.

On 2026-10-05 the owner decided to keep product-independent technical building blocks in a separate repository and created `cdilorenzo/BuildingBlocks` (public, MIT licence, default branch `main`). At commit `74854eb` it contains only a licence and a README: no CI, no package and no release. An earlier analysis had recommended not to create a shared repository yet. This ADR records the decision, both positions, and the rules that apply to CivicAction.

The component-by-component assessment of TuWasGutes is recorded in [reuse/tuwasgutes-reuse-matrix.md](../reuse/tuwasgutes-reuse-matrix.md). Its evidence cites file paths at commit `08b30b4` of a private repository, so readers without access to that repository cannot verify it.

## Decision

Decision: `CREATE SHARED REPOSITORY`. The owner made this decision on 2026-10-05 and carried it out the same day by creating the repository; this ADR records it retroactively. The owner accepted this record on 2026-10-13.

1. **Shared repository.** Product-independent technical building blocks are developed, versioned and released in `cdilorenzo/BuildingBlocks`. The repository exists; its baseline (SHARED-0, BuildingBlocks#1) and first package (SHARED-1, BuildingBlocks#2) are delivered by slices in that repository.
2. **Naming.** Package IDs and root namespaces use the prefix `DiLorenzo.BuildingBlocks.<Area>`.
3. **Distribution.** Shared code is distributed only as versioned NuGet packages on nuget.org. CivicAction is public, so every consumer must restore without credentials.
4. **First building block.** `DiLorenzo.BuildingBlocks.Testing.Http` is a product-neutral fake `HttpMessageHandler` for deterministic HTTP-client tests.
   - First consumer: TuWasGutes replaces its local helper (TWG-1, OneGood#4).
   - Second consumer: CivicAction uses it in test projects only, for the SRC-2 connector tests (REUSE-1, #42).
   - The first consumer validates the package before CivicAction adopts it.
5. **Shared code, not shared domain or data.** Shared packages contain no product terms, domain types, product data, database schema or identity model. Action selection, ranking, complete/skip, categories, progress/streaks and domain entities stay product-local. There is no shared database and no shared user or account.
6. **Dependency rules.** CivicAction follows the rules in [002-architecture.md](../002-architecture.md#shared-building-blocks):
   - no project, package or path reference to TuWasGutes;
   - no shared package in `CivicAction.Domain`;
   - shared packages only from nuget.org, pinned to an exact version (`[x.y.z]`);
   - restore without credentials and without an additional package source;
   - no committed path or project references to other repositories;
   - each adoption of a shared package is its own slice;
   - first-party .NET building blocks before custom wrappers.
7. **Criteria for further building blocks.** A component becomes a shared package only if all of these hold:
   - Both real products have the same technical responsibility for it.
   - Its contract carries no product knowledge: no product terms, domain types or source-specific behaviour.
   - It already exists, or is needed for a concrete, issue-backed purpose, in both products.
   - No first-party .NET library or API already covers the need.

   Each new package needs its own approved slices: one in BuildingBlocks and one per consumer.
8. **Supply chain.**
   - **Permanence.** Packages on nuget.org can be unlisted but not deleted, so every published version is a permanent public commitment.
   - **Pinning.** Consumers pin exact versions. An upgrade is a deliberate change in its own slice.
   - **Dependency review.**
     - CivicAction runs dependency review and CodeQL on every pull request ([`security-review.yml`](../../.github/workflows/security-review.yml)).
     - SHARED-0 plans the same checks for BuildingBlocks.
     - TuWasGutes has no CI and no dependency review at `08b30b4`. This is a known gap. Closing it is an owner follow-up outside this ADR and is not claimed here; until then, TuWasGutes checks the package source and version manually in the adopting pull request.
   - **Publish credential.** Only the BuildingBlocks release workflow holds the publish credential (SHARED-0). CivicAction holds no credential and needs none to restore.
   - **No unreviewed adoption.** TuWasGutes code reaches BuildingBlocks only through a reviewed slice with characterization tests. It does not reach CivicAction unreviewed: a pattern marked ADAPT is re-implemented, not copied.

## Consequences

- The fake HTTP handler is maintained once. SHARED-1 also adds recording of the requests sent, so CivicAction's connector tests can assert which hosts were called.
- The rules exist before the first shared package does, so no consumer has to be corrected later.
- A five-issue release chain across three repositories precedes CivicAction's use: this ADR, then SHARED-0, SHARED-1, TWG-1 and REUSE-1. SRC-2 is expected to depend on REUSE-1, so a delay in the chain can delay the live connector.
- The owner maintains a second repository with its own CI, publish credential and SemVer discipline.
- `CivicAction.Domain` is unaffected. ACT-1 and SRC-0 were not blocked by this decision.
- The reuse matrix is a point-in-time assessment at `08b30b4`. A proposal for a new shared package re-checks the current state of both products against the criteria in point 7.
- Follow-up:
  - REUSE-1 (#42) still specifies `Version="1.0.0"` and adopts the exact-match notation `[1.0.0]` at its bootstrap.
  - The owner decides on CI and dependency review for TuWasGutes.

## Alternatives considered

- **No shared repository yet (duplicate the helper).** Arguments for this position:
  - The only concrete candidate is a helper of about 49 lines, so duplicating it would cost little.
  - A shared repository adds ongoing cost for one maintainer: a second repository, CI, a publish credential, SemVer discipline and dependency review.
  - Only one consumer exists today. CivicAction has no connector tests yet, so its use is planned, not real.
  - Packages on nuget.org can only be unlisted, never deleted, so even a tiny helper becomes a public API commitment.
  - It adds a five-issue chain across three repositories before CivicAction gets the helper, and can delay SRC-2.

  The owner rejected it for these reasons:
  - Both products need the same thing today: TuWasGutes has the helper, and CivicAction's SRC-2 tests need one because they must run against synthetic data rather than live sources.
  - The first shared block is deliberately tiny. It proves the whole pipeline at low risk before anything larger is shared: a separate repository, CI, versioning, nuget.org publishing, and pinned, credential-free restores.
  - The rules are cheaper to set before code exists: no shared domain or data, exact version pins, no credentials needed to restore, no cross-repository references.
  - CivicAction is public, so packages go to nuget.org and not to a feed that needs a token.
- **Reference TuWasGutes directly** (project, path or source reference). Not selected: TuWasGutes is private, so public CivicAction builds could not restore it. It would also couple CivicAction to another product's internals.
- **GitHub Packages as the feed.** Not selected (owner decision, 2026-10-05): restoring from GitHub Packages requires a token even for public packages, which conflicts with credential-free restores of the public CivicAction repository.
- **Shared domain, database or identity.** Not selected: the products have different domains and data, and the reuse epic's guardrails exclude shared domain and shared data. Any shared technical authentication building block would need its own reuse decision under point 7.
