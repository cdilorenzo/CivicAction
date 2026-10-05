# ADR-0003: meinBerlin source access path

- Status: Accepted
- Date: 2026-10-05
- Issue: #19

## Context

The source-ingestion roadmap starts with meinBerlin (`https://mein.berlin.de`), the State of Berlin's participation platform. [007-source-ingestion.md](../007-source-ingestion.md) requires a feasibility investigation before a live connector is built. The investigation for SRC-0 is recorded in [sources/meinberlin-feasibility.md](../sources/meinberlin-feasibility.md). Its findings that matter for this decision:

- There is no officially documented API with usage terms, and no official feed or export. The platform publishes an automatically generated OpenAPI schema, but no stability or usage commitment.
- The platform's own frontend reads public, unauthenticated, read-only JSON endpoints. `GET /api/projects/?status=activeParticipation` and `?status=futureParticipation` return all running and upcoming public projects, with a stable integer `id`, title, short description, district, organisation name, topics, the relevant phase dates and the project URL. robots.txt does not exclude `/api/`.
- The terms of use address registered users only. Neither they nor the imprint grant a licence for project content or address automated access, caching or storage. These questions are open and need owner clarification with the operator.
- The participation type (module type) and module-level URLs are not in the API. They are only available by parsing HTML.
- External projects (`/api/extprojects/`) have no identifier and link to about 70 different hosts. Plans (`/api/plans/`) describe planning projects rather than participation opportunities, and have no identifier field either.

## Decision

The owner accepted this decision on 2026-10-05. Accepting it does not answer the rights questions in point 4; live use stays blocked until they are answered.

1. **Access path.** SRC-2 reads only these two endpoints:
   - `GET https://mein.berlin.de/api/projects/?status=activeParticipation`
   - `GET https://mein.berlin.de/api/projects/?status=futureParticipation`

   This is access level 3: a structured endpoint without a documented contract. HTML parsing, module and user-content endpoints, `/api/plans/` and `/api/extprojects/` are not part of the M1 integration.
2. **Granularity and identity.** One meinBerlin project is one source record. Its external identity is the source key plus the project's integer `id`. Disappearing from both lists means "no longer listed", because the API cannot distinguish ended, archived, unpublished, private and deleted projects.
3. **Participation route.**
   - On-site projects: the route is the API's relative `url`, resolved against the fixed base `https://mein.berlin.de`.
   - Development-plan (`Bplan`) projects: their absolute `url` is accepted only when it is `https` and its host is on an approved allowlist. By owner decision (2026-10-05), the allowlist contains exactly `be.beteiligung.diplanung.de` (DiPlanBeteiligung Berlin), with no wildcard. Development plans are therefore part of M1. SRC-2 implements and reviews the allowlist.
   - Module-level URLs are not used in M1.
4. **Preconditions for live use.** SRC-2 may be implemented and tested against synthetic fixtures. Scheduled live polling, and displaying meinBerlin content to users, require that the owner first records the operator's answer, or an explicit owner decision, on three questions:
   - automated reading of these endpoints;
   - displaying title, short description and link with attribution;
   - caching or storing the normalized fields.
5. **Access etiquette.**
   - Sequential requests only, never in parallel.
   - At most one poll per endpoint per hour, matching the platform's hourly cache refresh.
   - An identifying `User-Agent` with a contact, plus timeouts, a response-size limit, and limited retries with backoff.
6. **Data minimisation.** Only normalized fields that a CivicAction use case needs are kept. The following are not ingested:
   - images and image credits (credits can name individuals);
   - address properties of the location point;
   - contact persons;
   - user-generated content or usernames.
7. **Fixtures and source documents.**
   - Test fixtures are synthetic and copy only the observed JSON structure, never real content.
   - No meinBerlin page or text is stored as a SourceDocument until its storage and reuse rights are clarified.

## Consequences

- SRC-2 has a small, cheap integration surface: two JSON arrays of about 40 KB in total today, no pagination, and no credentials.
- The endpoints are an internal interface of the platform. They are unversioned and can change with any release. SRC-2 therefore needs defensive parsing and must surface errors, with contract tests against synthetic fixtures.
- `ParticipationType` cannot be filled from the API. By owner decision (2026-10-05), meinBerlin records use ACT-1's explicit "unknown" value until the question is clarified (#43), and the ACT-3 effort policy must handle that value.
- For a running project, the API provides the end of the module that ends next, but not the start of the running phase. ACT-1's `ParticipationWindow` must allow a missing start.
- An upcoming project can consist only of in-person events. Their technical phase starts and ends at the event date, and the API cannot tell such a project apart from upcoming online participation.
- The permission and licence questions remain open until the owner clarifies them. Until then, real meinBerlin data must not be polled on a schedule, displayed or committed.

## Alternatives considered

- **Parse project and module HTML pages (level 4).** This would provide module types, module URLs and phase start dates. It is not selected for M1 because it multiplies the request volume, depends on page markup, and is not explicitly permitted. It needs a separate owner decision.
- **Include `/api/extprojects/`.** Not selected: these records have no identifier, their URLs are not unique, 13 of 227 use `http`, and they point to about 70 hosts, including survey tools.
- **Use `/api/plans/`.** Not selected: plans are planning projects, often without any participation, and they carry no identifier field.
- **Crawl the sitemap.** Not selected: it lists project URLs without `lastmod`, so every page would need HTML parsing.
- **Open Data dataset or feed.** None was found. If the operator offers an official feed or export, prefer it, and supersede this ADR.
- **No automated ingestion.** If the operator does not permit automated use, the M1 loop continues with the SRC-1 fixture connector, and the owner decides on an editorially curated source or an agreement with the operator.
