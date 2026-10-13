# TuWasGutes reuse assessment

- Issue: #41 (REUSE-0)
- Assessed: 2026-10-13
- Evidence baseline: `cdilorenzo/OneGood`, branch `master`, commit `08b30b4` (the branch head on 2026-10-13)
- Decision record: [ADR-0002](../adr/0002-shared-components-repository.md) (Accepted)

This document assesses each TuWasGutes component against CivicAction's roadmap. For each one it decides whether the component is shared through `cdilorenzo/BuildingBlocks`, re-implemented as a pattern, kept product-local, or not needed. It also records patterns from TuWasGutes to adopt or avoid in four CivicAction slices.

It is a point-in-time assessment, not an implementation plan. A row does not approve work: the slice named as its target decides its own scope.

## Evidence and disclosure

- TuWasGutes is a private repository. Evidence is a file path at commit `08b30b4`, or "not present" together with how that was checked. Readers without access to that repository cannot verify it.
- The files were read at `08b30b4` without building or running TuWasGutes. Statements about runtime behaviour come from reading the code.
- This document contains no code excerpts, vulnerability details, secrets, internal hosts or configuration values from TuWasGutes. Where the inspection found security-relevant code, the component is not recommended for adoption, and the details are not recorded here.

Path abbreviations (relative to the TuWasGutes repository root):

| Abbreviation | Path |
| --- | --- |
| `Api/` | `src/OneGood.Api/` |
| `www/` | `src/OneGood.Api/wwwroot/` |
| `Core/` | `src/OneGood.Core/` |
| `Infra/` | `src/OneGood.Infrastructure/` |
| `Workers/` | `src/OneGood.Workers/` |
| `Tests/` | `tests/OneGood.Tests.Unit/` |

## Decision values

| Value | Meaning | Target |
| --- | --- | --- |
| SHARE | Product-neutral; provided as a package from the shared repository. | The package name |
| ADAPT | The pattern is re-implemented in CivicAction after review, without copying code and without any reference to TuWasGutes. | The responsible CivicAction slice |
| KEEP LOCAL | Product-specific; not shared. | The responsible CivicAction slice, or "stays in TuWasGutes" |
| NOT NEEDED | CivicAction has no approved need. | A reason |

No row decides CivicAction's persistence, ranking, identity, retention or deployment. Rows on those topics name the slice that decides.

## Summary

- One component is shared: the fake HTTP message handler, as `DiLorenzo.BuildingBlocks.Testing.Http` ([ADR-0002](../adr/0002-shared-components-repository.md)).
- All domain mechanisms are product-local: action selection, ranking, complete/skip, categories, progress/streaks and domain entities.
- UI patterns are adapted as patterns only. No TuWasGutes UI code or styles are taken over.
- The remaining infrastructure is either specific to TuWasGutes and its sources, or CivicAction has no approved need for it.
- One candidate needs a later re-check: both products may read abgeordnetenwatch (FED-2).

## Matrix

### Technical building blocks

| Component | TuWasGutes at `08b30b4` | CivicAction need | Decision | Target |
| --- | --- | --- | --- | --- |
| Fake HTTP message handler | A 49-line test helper that returns queued canned responses, is thread-safe and fails when the queue is empty. It does not record requests. Used in five test files. `Tests/Helpers/FakeHttpMessageHandler.cs` | Deterministic connector tests without live sources: SRC-2 (#21), [ADR-0003](../adr/0003-meinberlin-source-access-path.md) | SHARE | `DiLorenzo.BuildingBlocks.Testing.Http` (SHARED-1), used by CivicAction test projects only through REUSE-1 (#42) |
| Source clients | The volunteering clients are typed `HttpClient`s with a timeout and User-Agent. The other clients are scoped services that build TuWasGutes entities directly. Both catch errors broadly, and neither uses a resilience library. `lib/VolunteerSources/VolunteerSourcesServiceExtensions.cs`; `Infra/ExternalApis/*`; `Infra/DependencyInjection.cs` | The meinBerlin connector behind the CivicAction-specific `ICivicActionSource` port: SRC-1 (#20), SRC-2 (#21). No TuWasGutes source is a CivicAction M1 source. EPIC FED (#9) plans an abgeordnetenwatch connector (FED-2), and TuWasGutes reads that source in `Infra/ExternalApis/PetitionClient.cs`. | KEEP LOCAL | SRC-2 (#21); patterns in the [SRC-2 learnings](#src-2-meinberlin-live-connector-21). FED-2 re-checks an abgeordnetenwatch client against ADR-0002 point 7. The TuWasGutes client maps directly to its own entity, so it is not shareable as it is. |
| HTML helper | Regex tag stripping with entity decoding. Two copies share the same logic and a third, private variant has diverged. `Infra/ExternalApis/HtmlHelper.cs`; `lib/VolunteerSources/HtmlHelper.cs`; `Infra/ExternalApis/PetitionClient.cs` | None in M1. ADR-0003 excludes HTML parsing, and output encoding follows [005-security.md](../005-security.md). | NOT NEEDED | No M1 use. The diverging copies show the drift that ADR-0002 avoids. |
| Source registry and aggregation | A static list of sources. The aggregator fetches all sources in parallel, removes duplicates on source name plus external ID (enforced by a unique index), and deactivates records not refreshed in a run. Hosted services run the schedule. `Core/Sources/SourceRegistry.cs`; `Infra/Services/CauseAggregatorService.cs`; `Infra/Data/OneGoodDbContext.cs`; `Workers/*` | Source port and fixture import: SRC-1 (#20). Live refresh: SRC-2 (#21) and SRC-3 (EPIC SRC #3). | KEEP LOCAL | SRC-1 (#20), SRC-2 (#21); see the [SRC-2 learnings](#src-2-meinberlin-live-connector-21) |
| Caching | A content-cache interface over `IDistributedCache` that stores TuWasGutes content as JSON, backed by Redis or by per-process memory. The interface is shaped around TuWasGutes content. `Core/Interfaces/IContentCache.cs`; `Infra/Caching/DistributedContentCache.cs` | None approved. Caching meinBerlin content is blocked until the rights are clarified (ADR-0003). | NOT NEEDED | If a later slice needs caching, it uses first-party .NET caching APIs directly ([002-architecture.md](../002-architecture.md#shared-building-blocks)). |
| Persistence | EF Core 10. It references the SQLite, SQL Server, in-memory and PostgreSQL (Npgsql) providers, and the design-time factory uses Npgsql. `Infra/Data/OneGoodDbContextFactory.cs`; `Infra/Migrations/*`; `Infra/Repositories/*` | CivicAction persistence, not yet decided: ACT-2 (#17) | KEEP LOCAL | ACT-2 (#17) decides. This row is evidence, not a default. There is no shared database and there are no shared mappings or migrations. |
| AI services and classification | A provider interface with three LLM provider implementations. It is used for classification, scoring, card text, summaries and translation. `Core/AI/*`; `Infra/AI/*`; `Infra/Classification/AiCauseClassifier.cs`; `Infra/Services/AiEngineService.cs` | An optional, evidence-bound LLM explanation: EXPL-2 (#28), with a mandatory security review. No LLM in ranking: ACT-3 (#18) requires a deterministic policy. | KEEP LOCAL | EXPL-2 (#28) decides. The first-party rule in [002-architecture.md](../002-architecture.md#shared-building-blocks) applies to any AI abstraction. |
| Translation | LLM-based machine translation of source content into German. `Infra/Translation/*`; `Infra/Services/TranslationService.cs` | None approved. CivicAction shows official texts with attribution (ADR-0003). Translating them is not in the roadmap. | NOT NEEDED | No approved need: official texts are shown as published, with attribution (ADR-0003). |
| Localization | Client-side JSON files for eight languages, with runtime language selection and `dir` switching, plus a small server-side resx set. `www/i18n/*.json`; `www/js/modules/i18n.js`; `Core/Resources/*.resx`; `docs/localization.md` | A localization skeleton: WEB-0 (#22). Localized UI: WEB-1 (#23). Rules in [006-accessibility-and-localization.md](../006-accessibility-and-localization.md). | ADAPT | WEB-0 (#22): patterns only, no strings or code; see the [WEB-1 learnings](#web-1-time-budget-and-local-action-cards-23) |
| Slug, sitemap, prerender and SEO | Slug generation with a uniqueness check, a sitemap written at runtime, prerendering for crawlers only (detected by User-Agent), and a nightly SEO worker. `Infra/Services/SlugService.cs`; `Infra/Services/SitemapService.cs`; `Api/PrerenderMiddleware.cs`; `Workers/DailySeoWorker.cs` | None approved. Detail-page URLs are a matter for WEB-2 (#24). | NOT NEEDED | No approved need for SEO features. WEB-2 (#24) decides detail-page URLs. |
| Push and realtime | Web Push that cleans up expired subscriptions, a daily push worker, and a broadcast-only SignalR hub. `Infra/Services/PushNotificationService.cs`; `Workers/DailyPushWorker.cs`; `Api/Hubs/CauseHub.cs` | None. WEB-1 (#23) excludes push notifications. | NOT NEEDED | No approved need: WEB-1 (#23) excludes push notifications, and no slice needs realtime updates. |
| API keys and authentication | Hashed API keys with quotas for a partner API, and user login with optional external OAuth. `Api/ApiKeyAuthFilter.cs`; `Infra/Services/ApiKeyService.cs`; `Api/Controllers/AuthController.cs` | None in M1: no accounts (EPIC ID #7) and no partner API | NOT NEEDED | EPIC ID (#7). A shared technical authentication building block needs its own reuse decision. There is no shared user or account. |
| Analytics | An event endpoint that stores events with session, network and device fields. `Api/Controllers/AnalyticsController.cs`; `Core/Models/AnalyticsEvent.cs` | Privacy-safe aggregate metrics only after PART-0 (#38), built in PART-2 (#31) | KEEP LOCAL | PART-0 (#38) decides which data is allowed. The TuWasGutes event model is not a template; see the [PART-0 learnings](#part-0-participation-completion-and-privacy-decision-38). |
| CI and security automation | Not present. Checked by listing `.github/` and `.github/workflows/` at `08b30b4`: the only workflow is an issue-driven agent workflow, with no build, test, dependency-review or CodeQL workflow and no Dependabot or Renovate configuration. | Already in place in CivicAction (`ci.yml`, `security-review.yml`) | NOT NEEDED | The TuWasGutes gap is an owner follow-up (ADR-0002, point 8) |
| Hosting | A container image that runs the API and the worker process together, a deployment descriptor for a hosting platform, and a keep-alive service that stops the free hosting plan from putting the app to sleep. `Dockerfile`; `render.yaml`; `start-all.sh`; `Api/KeepAliveService.cs` | Not decided ([009-operations.md](../009-operations.md)) | NOT NEEDED | The operations decision in the first slice that needs production hosting |

### Domain mechanisms

| Component | TuWasGutes at `08b30b4` | CivicAction need | Decision | Target |
| --- | --- | --- | --- | --- |
| Action selection | Picks one cause at a time. It excludes recently completed, skipped and session-excluded causes, orders the rest by stored scores, and relaxes the exclusions step by step if nothing is left. `Infra/Services/ActionEngine.cs`; `Infra/Repositories/CauseRepository.cs`; `Infra/Services/FeaturedTodayService.cs` | Up to three open actions for a time budget: ACT-3 (#18) | KEEP LOCAL | ACT-3 (#18); see the [ACT-3 learnings](#act-3-query-for-relevant-actions-by-time-budget-18) |
| Ranking | A pure, static urgency calculator that returns a breakdown of its components. Some of the stored scores are written by an LLM, and the admin page re-implements the formula in client code. `Core/Services/UrgencyScoreCalculator.cs`; `Infra/Services/CauseAggregatorService.cs`; `www/js/scoring.js` | A deterministic, explainable Ranking Policy v1: ACT-3 (#18) | KEEP LOCAL | ACT-3 (#18) |
| Complete/skip | Completions are stored on the server per user, with an optional amount and note. Skips are cached for 24 hours, and the browser keeps its own copy. `Core/Models/UserAction.cs`; `Infra/Repositories/UserRepository.cs`; `www/js/modules/storage.js` | Decided by PART-0 (#38) before PART-1 (#30) | KEEP LOCAL | PART-0 (#38); see the [PART-0 learnings](#part-0-participation-completion-and-privacy-decision-38) |
| Categories | Closed enums for cause category and action type, with a slug map. A cause's category comes from a per-source override or from LLM classification. `Core/Enums/CauseCategory.cs`; `Core/Enums/ActionType.cs`; `Api/CategorySlugMap.cs`; `Infra/Classification/AiCauseClassifier.cs` | CivicAction's own closed taxonomies: ACT-1 (#16), ADR-0003 | KEEP LOCAL | ACT-1 (#16, implemented), ACT-3 (#18) |
| Progress/streaks | A daily streak is calculated both in the browser and on the server, and the client's value is synchronised to the server. `www/js/modules/storage.js`; `Infra/Repositories/UserRepository.cs`; `Api/Controllers/AuthController.cs` | None approved. Any progress feature needs PART-0 (#38). | KEEP LOCAL | Stays in TuWasGutes; see the [PART-0 learnings](#part-0-participation-completion-and-privacy-decision-38) |
| Domain entities | Models made only of properties, with no invariants, in a core project with no package references. `Core/Models/*`; `src/OneGood.Core/OneGood.Core.csproj` | The `CivicAction` aggregate ([ADR-0001](../adr/0001-participation-bounded-context-and-aggregate.md), ACT-1 #16) | KEEP LOCAL | ACT-1 (#16, implemented) |

### UI patterns

| Component | TuWasGutes at `08b30b4` | CivicAction need | Decision | Target |
| --- | --- | --- | --- | --- |
| PWA | A manifest and a service worker. Static assets are served cache-first from a hand-maintained precache list with a version constant, navigation is network-first, and API calls are not cached. The manifest locks the screen orientation to portrait. `www/manifest.json`; `www/sw.js` | An installable PWA: WEB-4 (EPIC WEB #4, no issue yet) | ADAPT | WEB-4: patterns only. Do not lock the orientation (WCAG 2.1 success criterion 1.3.4). |
| Swipe and cards | One card at a time. Swiping left or right skips or acts, and buttons offer the same actions. The card animations have no reduced-motion handling. `www/js/modules/swipe.js`; `www/css/card.css`; `www/js/modules/render.js` | Up to three action cards: WEB-1 (#23) | ADAPT | WEB-1 (#23): card content only. The swipe interaction is not adopted; see the [WEB-1 learnings](#web-1-time-budget-and-local-action-cards-23). |
| Client modules | Plain JavaScript ES modules in a single-page shell, one each for state, storage, i18n, rendering and API calls. `www/js/modules/state.js`; `www/js/modules/storage.js`; `www/js/modules/i18n.js`; `www/js/modules/render.js`; `www/js/modules/api.js` | The UI technology is decided by WEB-0 (#22). | KEEP LOCAL | WEB-0 (#22). No TuWasGutes UI code or styles are taken over (WEB-0 excludes them). |
| MAUI app | A native client project outside the solution, with placeholder folders and its own German/English strings. `src/OneGood.Maui/`; `OneGood.slnx` | None. CivicAction is a responsive web/PWA. | NOT NEEDED | No native client is in the roadmap. |

## Pattern learnings

These learnings are input for the named slices, not decisions. Each slice decides what it adopts, within its own approved scope.

### SRC-2: meinBerlin live connector (#21)

- **Adopt:** register each source as a typed `HttpClient` with an explicit timeout and an identifying User-Agent. ADR-0003 already requires both, and also a response-size limit, which TuWasGutes does not set. Evidence: `lib/VolunteerSources/VolunteerSourcesServiceExtensions.cs`.
- **Avoid:** browser-like or missing User-Agent strings. Also avoid clients that change the base address or headers of an injected `HttpClient` in their constructor. Evidence: `Infra/ExternalApis/PetitionClient.cs`, `Infra/DependencyInjection.cs`.
- **Adopt:** honour `Retry-After` and back off on HTTP 429, with sequential requests. Evidence: `lib/VolunteerSources/Stadtteilzentren/StadtteilzentrenClient.cs`.
- **Adopt:** parse into a source-shaped DTO first, then map it to the product contract in a separate, thin mapper. Evidence: `lib/VolunteerSources/Models/VolunteerOpportunityDto.cs`, `Infra/ExternalApis/BetterplaceDtoMapper.cs`.
- **Avoid:** catching broad exceptions in a client and returning an empty result.
  - In TuWasGutes, the only check is a global minimum number of fetched records across all sources. An outage of one source therefore looks as if all its records disappeared, and those records are deactivated.
  - Record fetch success per source instead. Mark records "no longer listed" (ADR-0003) only after a successful fetch of that source.
  - Evidence: `Infra/ExternalApis/*`, `Infra/Services/CauseAggregatorService.cs`.
- **Adopt:** a composite external identity made of the source key and the external ID, enforced by a unique index. Evidence: `Infra/Data/OneGoodDbContext.cs`. ADR-0003 already defines this identity for meinBerlin.
- **Avoid:** registering the same refresh service in two host processes. Evidence: `Api/Program.cs`, `Workers/Program.cs`.
- **Avoid:** sources that are covered only by live-network integration tests. Every source needs deterministic tests with synthetic responses. Evidence: `tests/OneGood.Tests.Integration/ExternalApisIntegrationTests.cs`.

### ACT-3: query for relevant actions by time budget (#18)

- **Adopt:** a pure scoring function that returns its components and raw signals, tested for determinism with a fixed clock. Evidence: `Core/Services/UrgencyScoreCalculator.cs`, `Tests/UrgencyScoreCalculatorTests.cs`.
- **Avoid:** ranking inputs written by an LLM. ACT-3's policy must be deterministic and explainable. Evidence: `Infra/Services/CauseAggregatorService.cs`, `Infra/Services/AiEngineService.cs`.
- **Avoid:** ranking rules in a repository, and a second copy of the formula in client code that drifts from the server. Keep one policy in the Application layer. Evidence: `Infra/Repositories/CauseRepository.cs`, `www/js/scoring.js`.
- **Avoid:** side effects in the selection path, such as clearing state or incrementing counters. The query is read-only. Evidence: `Infra/Services/ActionEngine.cs`.
- **Avoid:** time claims that no data supports.
  - TuWasGutes has no effort or duration attribute; its time promise exists only in UI copy.
  - ACT-3's effort policy must be explicit and labelled as an estimate.
  - Evidence: no such attribute in `Core/Models/*` or `Core/Enums/*`, checked by searching for "duration", "minutes", "effort" and "time budget". The copy is in `www/i18n/*.json`.
- **Avoid:** mixing UTC and local calendar days.
  - Use one authoritative day calculation with an explicit time zone, and `TimeProvider` for window boundaries.
  - Commit `08b30b4` itself fixes a day-boundary bug in TuWasGutes.
  - Evidence: `Infra/Services/FeaturedTodayService.cs`, `www/js/modules/storage.js`.

### WEB-1: time budget and local action cards (#23)

- **Adopt:** no interaction that depends on a gesture. TuWasGutes backs every swipe with a visible button. CivicAction cards should be plain, focusable links or buttons in a short list. Evidence: `www/js/modules/swipe.js`.
- **Avoid:** animations without `prefers-reduced-motion` handling, and content changes with no announcement or focus handling. Neither `prefers-reduced-motion` nor `aria-live` occurs under `www/` at `08b30b4`. Evidence: `www/css/card.css`, `www/js/modules/render.js`.
- **Avoid:** localizing only the visible text.
  - Localize accessible names (`aria-label`, `title`, `alt`) too.
  - Never show a key when its translation is missing.
  - Format plurals, dates and durations for the active locale.
  - Evidence: `www/js/modules/i18n.js`.
- **Adopt:** an explicit list of supported languages, a persisted language choice, `dir` switching for right-to-left languages, and the same set of keys in every language file. Evidence: `www/js/modules/i18n.js`, `www/i18n/*.json`.
- **Avoid:** fetch calls that fail silently. WEB-1 needs visible, localized loading, empty and error states. Evidence: `www/js/modules/api.js`.
- **Avoid:** duration promises in copy that no data supports (see ACT-3). Evidence: `www/i18n/*.json`.

### PART-0: participation completion and privacy decision (#38)

The three anti-patterns that PART-0 names are confirmed at `08b30b4`:

- **Avoid:** counting a click to the external site as completion. Evidence: `www/js/app.js`. **Adopt instead:** an explicit confirmation that is separate from the hand-off, as the TuWasGutes explore mode already does (`www/js/app.js`).
- **Avoid:** a long-lived anonymous cookie identifier, with a server-side user record created implicitly on first use. Evidence: `Api/Controllers/ActionsController.cs`, `Infra/Repositories/UserRepository.cs`.
- **Avoid:** client-authoritative streaks synchronised to the server. Evidence: `www/js/modules/storage.js`, `Api/Controllers/AuthController.cs`.

Further learnings:

- **Avoid:** optional free-text and amount fields in completion records when no purpose has been decided for them. Evidence: `Core/Models/UserAction.cs`.
- **Avoid:** analytics events that carry network, device or session identifiers or free text. Evidence: `Core/Models/AnalyticsEvent.cs`, `Api/Controllers/AnalyticsController.cs`.
- **Avoid:** an engagement history on the device (which actions were seen or acted on) and a persistent analytics identifier in browser storage.
  - For civic participation, such a history can reveal political interests.
  - A device-local variant (PART-0 variant B) therefore needs the same scrutiny as server-side storage.
  - Evidence: `www/js/modules/storage.js`.
- **Adopt:** decide the retention and deletion of every behavioural record before collecting it. Deleting a profile must also cover the records linked to it. Evidence: `Core/Models/UserAction.cs`, `Core/Models/AnalyticsEvent.cs`, `Infra/Repositories/UserRepository.cs`.

## Limitations

- The assessment covers commit `08b30b4` only. Later TuWasGutes changes are not tracked here. A proposal for a new shared package re-checks the current state against ADR-0002 point 7.
- Runtime behaviour was not observed. Statements that could not be confirmed by reading the code were left out.
- Security-relevant findings are deliberately not recorded. The affected components are not adoption candidates.
