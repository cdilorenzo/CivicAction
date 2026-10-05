# meinBerlin source feasibility

- Issue: #19 (SRC-0)
- Checked: 2026-10-05
- Decision record: [ADR-0003](../adr/0003-meinberlin-source-access-path.md) (Accepted)

This document records whether, and how, CivicAction can ingest civic participation opportunities from meinBerlin (`https://mein.berlin.de`). It answers the questions [007-source-ingestion.md](../007-source-ingestion.md) requires before a live connector is built.

It is an investigation result, not an implemented integration. It provides evidence, not legal advice. The assessment of the terms and rights is an owner task (see [Open owner decisions](#open-owner-decisions)).

## Summary

- **Access.** There is no officially documented API and no official feed or export. The platform's own frontend uses public, read-only JSON endpoints that work without authentication. robots.txt does not exclude them, and they are technically suitable for M1 (access level 3).
- **Permission.** The terms of use address registered users only. Neither they nor the imprint grant a licence for project content or address automated access, caching or storage. Permission is **unclarified**, and the owner has to clarify it with the operator before live polling or display.
- **Data.** At project level, the API provides:
  - a stable integer ID;
  - title and short description;
  - district, organisation name and topics;
  - the relevant phase dates (ISO 8601 with offset);
  - the project URL.

  It does **not** provide the participation type, module URLs, long descriptions or any time effort.
- **Granularity.** A project contains one or more participation modules, several of which can run at the same time. Only the project level is available without HTML parsing. The recommendation is one CivicAction per project, with the project page as `OfficialParticipationRoute`.
- **Fixtures.** No content licence exists, so test fixtures must be synthetic.
- **Recommendation.** Read `/api/projects/?status=activeParticipation` and `?status=futureParticipation` under the conditions in [ADR-0003](../adr/0003-meinberlin-source-access-path.md). HTML parsing is not approved. If the operator does not permit automated use, M1 continues with the SRC-1 fixture connector.

## Method and sample

**Evidence types.** Each finding is labelled with its evidence type:

- **[doc]**: an official page or statement.
- **[obs]**: observed in a live response on 2026-10-05.
- **[code]**: the platform's public source code. This shows how the platform works today, but it is not a published contract and can change with any release.
- **[inf]**: inferred, not verified.

**Order of work.** robots.txt was checked first, then the terms of use, imprint and FAQ. Only after that were API endpoints requested.

**Requests.**
- All requests were manual or sequential, at least 2–4 seconds apart. No authentication, account, login or participation action was used.
- User-Agent: `CivicAction-feasibility-research/0.1 (manual investigation; github.com/cdilorenzo/CivicAction/issues/19)`.
- Raw responses were kept outside the repository. This document contains only field paths, counts, value lists and synthetic examples.

| Requests to `mein.berlin.de` | Count |
| --- | --- |
| robots.txt, sitemap index, adhocracy4 sitemap | 3 |
| Home page, terms of use, imprint, FAQ | 4 |
| `/api/projects/` (unfiltered, `status=activeParticipation`, `status=futureParticipation`), `/api/projects/{id}/` (existing and non-existent ID), `/api/extprojects/`, `/api/plans/` | 7 |
| `/api/schema/`, `/api/docs/` | 2 |
| Project and module HTML pages: 2 exploratory, plus the survey of 36 project and 75 module pages (see [Participation types](#participation-types)) | 113 |
| **Total** | **129** |

**Other services.** Nine search requests went to the GovData CKAN API (`www.govdata.de/ckan/api/3/action/package_search`). One request went to the daten.berlin.de CKAN API, which answered with a bot challenge (HTTP 403). Three requests went to `be.beteiligung.diplanung.de` (robots.txt, home page, imprint) to identify the development-plan platform.

**Source code.** The source was read at these commits:

- [liqd/a4-meinberlin@f2eebf8](https://github.com/liqd/a4-meinberlin/tree/f2eebf8a16dcf8d376b77f5d9529730877238589) (2026-10-05).
- [liqd/adhocracy4@68a8ab3](https://github.com/liqd/adhocracy4/tree/68a8ab3d3ea265ae674696a7fa7ec772bb750397) (2026-09-14).

a4-meinberlin installs adhocracy4 from its `main` branch without pinning a commit ([requirements/base.txt](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/requirements/base.txt#L1-L2)). The version running in production therefore cannot be determined from the source.

**Sample.**

| Set | Size | Use |
| --- | --- | --- |
| Running projects (`status=activeParticipation`) | 31 (30 on-site, 1 development plan on Diplan) | Field matrix, URLs, dates |
| Upcoming projects (`status=futureParticipation`) | 6 | Field matrix, start dates |
| All listed projects (unfiltered `/api/projects/`) | 979 | Value lists, ID range, consistency |
| External projects (`/api/extprojects/`) | 227 | Identity and host analysis |
| Plans (`/api/plans/`) | 1,240 | Scope assessment |
| Project and module pages of the 36 running and upcoming on-site projects | 36 project pages, 75 module pages | Participation types, module structure, phase dates |

## Access levels

| Level | Exists | Usable for CivicAction | Evidence (checked 2026-10-05) |
| --- | --- | --- | --- |
| 1. Officially documented API | **No** | — | No developer documentation, API terms or contact for third-party use was found on the platform ([FAQ](https://mein.berlin.de/faqundsupport/), [terms of use](https://mein.berlin.de/terms-of-use/), [imprint](https://mein.berlin.de/impressum/)) or by web search. The platform serves an automatically generated OpenAPI 3.0.3 schema at [`/api/schema/`](https://mein.berlin.de/api/schema/) with a Swagger UI at `/api/docs/` [obs]. This is generated from code ([urls.py](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/config/urls.py#L239-L246)): it has no descriptions, does not document the `status` filter, carries the generic version `1.0.0`, and makes no stability or usage commitment. |
| 2. Official feed or export | **No** | — | No RSS, Atom or iCal feeds exist [code]. Exports (CSV/XLSX) are dashboard functions for project initiators only and are not public ([exports/views.py](https://github.com/liqd/adhocracy4/blob/68a8ab3d3ea265ae674696a7fa7ec772bb750397/adhocracy4/exports/views.py#L106-L128), [plans/exports.py](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/apps/plans/exports.py#L11-L43)). GovData has no meinBerlin dataset: searches for `meinberlin`, `Beteiligungsplattform`, `Kiezkasse`, `Bürgerhaushalt` and `Liquid Democracy` returned 0 results [obs]. daten.berlin.de blocks scripted queries with a bot challenge, so it could not be searched directly; the owner should confirm in a browser. The [sitemap](https://mein.berlin.de/sitemap.xml) lists 1,736 project URLs without `lastmod` [obs], so it is a URL index, not a data export. |
| 3. Stable structured endpoint with permitted automated use | **Partly** | **Yes, technically**; permission unclarified | `GET /api/projects/`, `/api/extprojects/` and `/api/plans/` are public, read-only, unauthenticated and return complete JSON arrays [obs] [code] ([projects/api.py](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/apps/projects/api.py#L73-L112)). The frontend's project overview uses them ([ProjectsListMapBox.jsx](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/react/projects/ProjectsListMapBox.jsx#L89)). robots.txt allows `/api/`. Stability is not guaranteed: these are internal interfaces of an actively developed platform whose core library is installed from a floating branch. Permitted automated use is neither granted nor prohibited (see [Access conditions](#access-conditions)). |
| 4. HTML parsing | Yes | Not recommended | Project and module pages contain module links, module type markers and phase dates in `<time datetime>` elements [obs]. Parsing them would add one request per project and per module, depend on page markup, and has no explicit permission. It needs a separate owner decision. |

## Access conditions

| Condition | Finding | Status |
| --- | --- | --- |
| Terms of use | The [terms of use](https://mein.berlin.de/terms-of-use/) apply "mit der Anmeldung als Nutzerin bzw. Nutzer". They cover registration, contributions, liability, the rights that contributors grant the operator, and termination. They say nothing about reading the site, automated access or reuse by third parties. [doc] | **Unclarified.** Automated access is not explicitly permitted or prohibited. |
| Content licence | The [imprint](https://mein.berlin.de/impressum/) licenses only the software (AGPLv3) and the meinBerlin logo and graphics (CC BY-SA). Project content is published by the responsible administrations ("von den zuständigen Fachverwaltungen eigenverantwortlich veröffentlicht"). Contributors grant rights only to the operator. The code declares no licence for project data or texts [code]. | **Unclarified.** There is no licence for project texts or images. Short factual data (dates, district, organisation, link) is less critical than texts and images, but this is an owner assessment. |
| Attribution | No attribution rule for reusing project data exists. Images carry credits (`tile_image_copyright`); the base map requires "© basemap.de / BKG \| Datenquellen: © GeoBasis-DE" ([settings](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/config/settings/base.py#L553-L557)). | **Unclarified.** Recommended practice: name and link "meinBerlin" as the source on every action, and do not use images. |
| robots.txt | `User-agent: *`, `Allow: /`, `Disallow: /admin/`, `Disallow: /django-admin/`, `Sitemap: https://mein.berlin.de/sitemap.xml` ([robots.txt](https://mein.berlin.de/robots.txt), [template](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/templates/robots.txt#L1-L4)) [obs]. | **Documented.** `/api/` and project pages are not excluded. |
| Rate limits | No documented limits. The API configures no throttling ([REST_FRAMEWORK settings](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/config/settings/base.py#L812-L815); DRF's default is none) [code]. No rate-limit or `Retry-After` headers were returned, and every platform request succeeded with HTTP 200 (or the intended 404) [obs]. Limits in the web server or WAF are not visible. | **Undocumented.** Use self-imposed limits (see [ADR-0003](../adr/0003-meinberlin-source-access-path.md)). |
| Caching permission | Not addressed by terms or imprint. The API sends `Cache-Control: no-cache, no-store, must-revalidate` [obs]; this is the framework's default for all responses, not a usage statement. | **Unclarified.** |
| Storage as SourceDocument (EXPL-0) | Not addressed. Long texts (`information`, `result`) and images are protected content of the publishing administration or third parties, and no licence permits storing them. | **Not permitted without clarification.** Do not store meinBerlin pages or texts as SourceDocuments until the owner has clarified the rights. |
| Committing as test fixtures (SRC-1) | Not addressed, and no licence permits redistribution in a public repository. | **No.** Use synthetic fixtures that copy only the field structure (see [Impact on follow-up slices](#impact-on-follow-up-slices)). |

## Data availability matrix

The source for every field is `GET /api/projects/?status=…` unless stated otherwise ([ProjectSerializer](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/apps/projects/serializers.py#L86-L114)).

- **Required/optional:** what the sample showed, not a documented contract.
- **Examples:** synthetic, or a generic value from the platform's value lists.

| Field | Available | Field path | Format | Required/optional | Stability | Example |
| --- | --- | --- | --- | --- | --- | --- |
| ExternalId | Yes (projects only) | `id` | integer | Required (37/37) | Database primary key (`AutoField`). Never re-assigned in normal operation [inf]. There is no documented guarantee. `/api/extprojects/` and `/api/plans/` have **no** ID field. | `1234` |
| Title | Yes | `title` | plain string, ≤ 120 characters ([model](https://github.com/liqd/adhocracy4/blob/68a8ab3d3ea265ae674696a7fa7ec772bb750397/adhocracy4/projects/models.py#L201-L333)) | Required (37/37) | Editable by initiators at any time. | `Neugestaltung eines Spielplatzes` |
| ShortDescription | Yes | `description` | plain string, ≤ 250 characters. No markup observed; one HTML entity in 979 projects. | Present in 37/37 | Editable | `Ideen für die Umgestaltung sammeln.` |
| Description (long) | No (API); HTML only | `/projekte/<slug>/information/` page (`information` rich HTML field) | HTML | Optional | Editable | — |
| ParticipationType | **No** (API) | Not exposed. The module type (`blueprint_type`) is only visible in module HTML (see [Participation types](#participation-types)). The API field `participation` is the constant `1` for every project ([serializer](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/apps/projects/serializers.py#L197-L198)). | — | — | — | — |
| StartsAt | Partial | `future_phase`: the start of the next upcoming module, for upcoming projects only. For running projects the start is not in the API; it is only in module HTML. | ISO 8601 with offset, or `false` | Present for 6/6 upcoming, 0/31 running | Editable; changes do not update `created_or_modified` | `2026-11-02T10:00:00+01:00` |
| EndsAt | Partial | `active_phase[2]`: the end of the running module that ends next, for running projects. `past_phase`: the end of a past module. Upcoming projects have no end date. | ISO 8601 with offset, or `false` | Present for 31/31 running, 0/6 upcoming | Editable; changes do not update `created_or_modified` | `2026-12-31T23:59:00+01:00` |
| Deadline | No separate field | — | — | — | — | The participation deadline is the end of the running phase (`EndsAt`). See [Value lists](#value-lists). |
| District | Yes | `district` | string (district name, no code). `null` is replaced by "Gesamtstädtisch" ([serializer](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/apps/projects/serializers.py#L20-L25)). | Always present (because of the fallback) | Values are database rows managed by administrators | `Mitte` |
| Locality | Partial | `point.properties`: `strasse`, `haus`, `plz` (address of the project location). There is no locality (Ortsteil) field. | strings | Optional | Editable | — (not recommended, see [Personal data](#personal-data-in-the-source)) |
| Geo data | Partial | `point.geometry.coordinates` | GeoJSON Point `[lon, lat]`, WGS 84 | Present in 27/31 running and 6/6 upcoming | Editable | `[13.40, 52.52]` |
| Canonical project URL | Yes | `url` | Relative path `/projekte/<slug>/` for on-site projects. For development plans (`subtype` `external`), it is the absolute URL on the Diplan platform. | Required (37/37) | The slug is unique and is not regenerated on rename, but administrators can edit it ([admin](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/apps/projects/admin.py#L162)). Use `id`, not the URL, for identity. | `/projekte/<slug>/` |
| Participation/module URL | No (API); HTML only | Links `/projekte/module/<module-slug>/` on the project page | relative path | 1..n per project | Module slugs are unique and not editable ([model](https://github.com/liqd/adhocracy4/blob/68a8ab3d3ea265ae674696a7fa7ec772bb750397/adhocracy4/modules/models.py#L71-L111)) | `/projekte/module/<module-slug>/` |
| Responsible organisation | Yes (name only) | `organisation` | string, no ID | Required (37/37). 17 distinct organisations among running projects, 86 overall. | Organisation names are editable | `Bezirksamt Mitte` |
| Status | Yes (derived) | `status` (0 running, 1 upcoming, 2 finished); `participation_active` (bool); `participation_string` (German display text) | integer, bool, string | Required | Computed when the server cache is filled (see [Change, correction and deletion behaviour](#change-correction-and-deletion-behaviour)) | `0` |
| Update timestamp | Partial | `created_or_modified` | ISO 8601 with offset | Required | **Unreliable as a change marker.** It does not change when phase dates, modules or topics change, or after bulk archiving ([admin actions](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/apps/projects/admin.py#L14-L21)). | `2026-09-16T11:01:18+02:00` |
| Change detection | No server support | No `ETag` or `Last-Modified`; `Cache-Control: no-store` [obs]. | — | — | — | See [Identity stability and change detection](#identity-stability-and-change-detection) |
| Relevant source documents | HTML only | Project information page `/projekte/<slug>/information/`, results page `/projekte/<slug>/results/`, module pages, linked plan page (`plan_url`, `/vorhaben/<YYYY>-<NNNNN>/`) | HTML | Optional; `plan_url` is set for 22/37 | Editable | — |

**Fields not needed for M1.** The API also returns:

- `topics`: a list of codes; see [Value lists](#value-lists).
- `tile_image`, `tile_image_alt_text`, `tile_image_copyright`: an absolute image URL on `mein.berlin.de`, its alt text and the image credit.
- `access`: 1 = public, 2 = semi-public.
- `plan_title`, `plan_url`.
- `subtype` (`default`, `external`) and `type` (`project`).
- `cost`, `point_label` and `published_projects_count`. For projects these are the constants `""`, `""` and `0`.

## Value lists

**Participation type.** The source has no participation-type field in the API; see [Participation types](#participation-types) for the module types.

**District.** The `district` field contains one of 13 values: the 12 Berlin districts plus "Gesamtstädtisch" (city-wide) [obs] [code]:

- Mitte, Friedrichshain-Kreuzberg, Pankow, Charlottenburg-Wilmersdorf, Spandau, Steglitz-Zehlendorf, Tempelhof-Schöneberg, Neukölln, Treptow-Köpenick, Marzahn-Hellersdorf, Lichtenberg, Reinickendorf;
- Gesamtstädtisch.

Short codes (`mi`, `fk`, `pa`, `cw`, `sp`, `sz`, `ts`, `nk`, `tk`, `mh`, `li`, `rd`, `be`) exist only in the write-only development-plan API ([bplan_api.md](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/docs/bplan_api.md#L323-L341)). They are not in the read API.

"Gesamtstädtisch" is ambiguous: it is either the city-wide district row or the replacement for a missing district. There is no finer locality.

**Status.** Three values:

| `status` | `participation_string` |
| --- | --- |
| `0` | `laufend` |
| `1` | `startet am <dd.mm.yyyy>` or `startet in der Zukunft` |
| `2` | `abgeschlossen` |

The display strings are localized German and must not be parsed ([serializer](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/apps/projects/serializers.py#L120-L140)).

**Topics.** The `topics` field holds 13 codes ([enum](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/apps/contrib/enums.py#L7-L22)):

| Code | Topic |
| --- | --- |
| `ANT` | Antidiskriminierung |
| `WOR` | Arbeit & Wirtschaft |
| `BUI` | Bauen & Wohnen |
| `EDU` | Bildung & Forschung |
| `CHI` | Kinder, Jugend & Familie |
| `FIN` | Finanzen |
| `HEA` | Gesundheit & Sport |
| `INT` | Integration |
| `CUL` | Kultur & Freizeit |
| `NEI` | Nachbarschaft & Teilhabe |
| `URB` | Stadtentwicklung |
| `ENV` | Umwelt & Grünflächen |
| `TRA` | Verkehr |

Projects carry one or two topics.

**Participation level (plans only).** 0 Information, 1 Mitwirkung, 2 Mitentscheidung, 3 Entscheidung ([plans/models.py](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/apps/plans/models.py#L26-L198)). This is not in the project API.

**Deadline versus EndsAt.** meinBerlin has no separate deadline concept. A module's phases have a start and an end, and a phase is active while `start ≤ now < end` ([phases/models.py](https://github.com/liqd/adhocracy4/blob/68a8ab3d3ea265ae674696a7fa7ec772bb750397/adhocracy4/phases/models.py#L16-L20)). The participation deadline is therefore the end of the running phase. For a project with several running modules, the API reports the earliest module end.

Some initiators set end times to midnight (`T00:00:00`) and others to `T23:59:00` [obs]. Treat the end as an exclusive instant and do not reinterpret it as a whole day.

**Timezone.** The platform runs with `TIME_ZONE = "Europe/Berlin"` and `USE_TZ = True` ([settings](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/config/settings/base.py#L217-L228)). The API serializes date-times as ISO 8601 with the Berlin offset (`+01:00` or `+02:00`) [obs]. They map directly to `DateTimeOffset`.

## Participation types

**The read API exposes no participation type.**
- The module type (`blueprint_type`) is fixed when a module is created ([blueprints.py](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/apps/dashboard/blueprints.py#L14-L233), [A4_BLUEPRINT_TYPES](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/config/settings/base.py#L561-L577)).
- On a module page, the type can only be recognized from the participation widget it embeds.

**Survey of all modules.** All 36 running and upcoming on-site projects were surveyed: 36 project pages and the 75 module pages they link to.
- Each page was fetched once, sequentially, at least 4 seconds apart.
- Time: 2026-10-05, 09:36–09:44 UTC.
- Every module was classified, so every participation type found is represented at least once.

| Module type (platform name) | Code | HTML marker on the module page | Running | Ended | Upcoming |
| --- | --- | --- | --- | --- | --- |
| Brainstorming / Ideensammlung | `BS`, `IC` | `data-mb-widget="ideas"` (the two types share the widget) | 17 | 0 | 0 |
| Brainstorming mit Karte / Ideensammlung mit Karte | `MBS`, `MIC` | `data-mb-widget="map-ideas"` | 6 | 2 | 2 |
| Umfrage | `PO` | `data-a4-widget="polls"` | 4 | 0 | 0 |
| Text diskutieren | `TR` | Document chapters with `data-a4-widget="comment_async"` | 3 | 1 | 1 [inf] |
| Interaktive Veranstaltung | `IE` | `data-ie-widget="questions"` | 1 | 0 | 0 |
| Priorisierung mit Karte | `MTP` | `data-mb-widget="map-topics"` | 0 | 0 | 1 [inf] |
| Veranstaltung (offline event) | `OE` | No widget; "Veranstaltungsart" and "Datum" text | 0 (never running) | 37 in total, not split by event date (32 in running projects, 5 in upcoming projects) | |
| Bebauungsplan (development plan) | `BP` | None; redirects to Diplan | 1 running (from the API, `subtype` `external`) | | |
| Bürger*innenhaushalt (1 or 2 phases), Priorisierung, Kiezkasse (legacy) | `PB`, `PB2`, `TP`, `KK` | `proposals`/`support`, `topics`, `kiezkasse-proposals` | Defined in code, not found in the sample | | |
| Verlinkung (external project) | `EP` | Redirects to an external site | Only in `/api/extprojects/` (not recommended) | | |

**[inf] rows.** Upcoming modules do not render their widget yet, so their type was inferred from the module name.

**Offline events.**
- An offline-event module has a technical phase whose start and end are both set to the event date ([offlineevents/models.py](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/apps/offlineevents/models.py#L39-L42)).
- Such a phase is never running, but a future event date places the project in the upcoming list, with the event date as `future_phase`.
- 2 of the 6 upcoming projects consisted only of offline events. The API cannot tell such a project apart from an upcoming online participation.

**Consequence.** The source-side taxonomy that ACT-1 can use is the module-type list above. For records from the read API, however, the type is always unknown.

## Project versus participation module

**Structure** [code]:
- A project has one or more modules, and each module has one or more phases with start and end dates.
- The module type (`blueprint_type`) is fixed when the module is created.
- Development-plan projects and external projects have exactly one phase and no on-site participation.
- In the sample, modules per project were distributed as follows [obs]:

  | Modules per project | Projects |
  | --- | --- |
  | 1 | 26 |
  | 2 | 6 |
  | 3 | 1 |
  | 6 | 1 |
  | 10 | 1 |
  | 18 | 1 |

  Of the 30 running on-site projects, 29 had exactly one running module and one had two. Most additional modules are offline events or ended modules.

**Identifiers.**

| Level | Identifiers | In the read API? |
| --- | --- | --- |
| Project | Integer `id`, slug | Yes, `/api/projects/` |
| Module | Integer primary key, slug | No. Only in module HTML, for example `apiUrl` `/api/modules/<pk>/…` |

The module-level APIs (`/api/modules/<pk>/ideas/` and similar) return user contributions, not module metadata.

**Recommended CivicAction level: the project** (input for ACT-1, which decides).
- Only project-level data is available without HTML parsing.
- A project with several running modules then becomes one CivicAction.
- The project page lists its running modules under "Online-Beteiligung", with direct links [obs].

**Recommended `OfficialParticipationRoute`.**
- On-site projects: the project page, `https://mein.berlin.de` + `url` (`/projekte/<slug>/`).
- Development plans: the absolute Diplan URL, which is where participation actually takes place. The on-site page redirects there permanently ([bplan/views.py](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/apps/bplan/views.py#L90-L105)). Accept it only if its host is on the SRC-2 allowlist.
- Module URLs would point more precisely to the participation itself, but they are not available at access level 3.
- Contributing usually requires a meinBerlin account ([terms of use](https://mein.berlin.de/terms-of-use/)). Some projects allow guest participation (`allow_guest_users`) [code].

**Hosts of participation URLs** [obs]:

| Source | Hosts |
| --- | --- |
| On-site projects (30/31 running, 6/6 upcoming) | `mein.berlin.de`. The URL is relative and is resolved against this host. |
| Development plans in `/api/projects/` (1/31 running, 16/979 overall) | `be.beteiligung.diplanung.de`: "DiPlanBeteiligung Berlin", part of DiPlanung, a joint project of Bayern, Berlin, Brandenburg, Bremen, Hamburg, Niedersachsen and Schleswig-Holstein. Provider and technical operator: Freie und Hansestadt Hamburg, Behörde für Stadtentwicklung und Wohnen, Fachliche Leitstelle DiPlanung. Each procedure's content is the responsibility of the authority that runs it ("Verfahrensträger"); this instance also lists Brandenburg municipalities ([imprint](https://be.beteiligung.diplanung.de/impressum)) [doc]. |
| Images | `mein.berlin.de` |
| External projects (`/api/extprojects/`, not recommended) | 70 distinct hosts, including `www.berlin.de` (80), `www.stadtentwicklung.berlin.de` (21), `be.beteiligung.diplanung.de` (11), `www.infravelo.de` (11), survey tools (`survey.lamapoll.de`, `befragung.sslsurvey.de`, `app.maptionnaire.com`, `form.jotform.com`, `padlet.com`) and video platforms. 13 of 227 URLs use `http`. Only 210 of the 227 URLs are distinct. |

## Identity stability and change detection

**Project ID.**
- Stable and suitable as `ExternalId`. It is the integer primary key, it is unique, it does not change when a project is renamed, and it is the key of `/api/projects/{id}/` [code] [obs].
- The basis is technical (database key, observed behaviour). The operator gives no documented guarantee.
- Observed range: 32–2817.

**Other identifiers.**
- Slug and URL: not suitable as identity, because administrators can edit the slug.
- External projects and plans: no stable key in the API. A plan's primary key can be read from its URL (`/vorhaben/<YYYY>-<NNNNN>/`); an external project has only its external URL, which is not unique. This is another reason to exclude both.

**Change detection options.**
1. **Full snapshot comparison (recommended).** The two status lists are small (about 35 KB and 6.5 KB today) and unpaginated. Each poll can compare the complete set of IDs and a content hash of the normalized fields per record (the SRC-2 "Source Hash").
2. **`created_or_modified` as a watermark: not reliable.** Phase-date, module and topic changes do not update it.
3. **Conditional requests: not available.** There is no `ETag` or `Last-Modified`, and the response says `Cache-Control: no-store`.
4. **Sitemap: not suitable.** It has no `lastmod`.

**Freshness.**
- The list endpoints are served from a server-side cache with a 24-hour expiry ([production settings](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/config/settings/production.py#L8-L14)).
- That cache is refreshed hourly, at scheduled phase starts and ends, and when initiators edit a project in the dashboard ([production settings](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/config/settings/production.py#L46-L71), [tasks.py](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/apps/projects/tasks.py#L37-L106), [signals.py](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/apps/projects/signals.py#L52-L72)).
- Polling more often than once per hour therefore gains little.

## Change, correction and deletion behaviour

**Corrections.** Initiators can edit titles, descriptions, dates, district, topics and location at any time. The API keeps no version or change history [code].

**Removal from the lists.** A project disappears from the lists when it is unpublished (`is_draft`), archived (`is_archived`), made private, or deleted ([get_public_projects](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/apps/projects/api.py#L27-L45)). Deletion is a hard delete with no tombstone ([dashboard/views.py](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/apps/dashboard/views.py#L358-L374)). It also leaves the status lists when its phases end. A consumer cannot tell these cases apart: `/api/projects/{id}/` returns `404` with `{"detail":"No Project matches the given query."}` for a non-existent ID [obs], and uses the same filtered query for hidden projects [code]. Archived projects stay visible on their HTML page and in the sitemap.

**Status consistency.** `status` is computed when the cache is filled. In the sample, the unfiltered list marked 32 projects as running, while the `activeParticipation` list contained 31. The extra project had no running phase (`active_phase` was `false`, and its last phase had ended in May 2026) [obs]. Use the status-filtered lists, and derive "open" from the dates, as ACT-1 already plans.

**Errors and limits** [obs]:
- **Status codes.** 404 returns a JSON `detail` object. No 429 or 5xx was observed.
- **Rate-limit signals.** None: no rate-limit headers and no `Retry-After`.
- **Response times.** 0.25–0.93 s.
- **Response sizes.**

  | Response | Size |
  | --- | --- |
  | Running projects | 35 KB |
  | Upcoming projects | 6.5 KB |
  | Unfiltered project list | 1.08 MB |
  | External projects | 260 KB |
  | Plans | 1.03 MB |
- **Pagination.** None; every list is a single JSON array.

**Maintenance and outages.** No status page, maintenance notice or service level was found. This is unknown.

## Time effort

The source provides **no time effort or duration** for participating [code] [obs].
- `active_phase[1]` is the remaining time of the phase as a German display string ("452 Tage").
- `Plan.duration` is free text describing a plan's runtime. It is not in the API.

The ACT-3 effort policy is therefore the only source of a time estimate. Because the participation type is not in the API either, that policy also has to handle an unknown participation type.

## Personal data in the source

| Location | Personal data | Recommendation |
| --- | --- | --- |
| `/api/projects/`: `tile_image_copyright` | Sometimes names an individual photographer [obs] | Do not ingest images or image credits. |
| `/api/projects/`: `point.properties` (`strasse`, `haus`, `plz`) | Address of the project location; can identify a specific building | Do not ingest. The district is enough for M1. |
| `/api/projects/`: `organisation` | Organisation names (public bodies, agencies, sometimes companies) | May be ingested as `ResponsibleOrganization`. Not personal data in general. |
| `/api/projects/`: `description`; module page texts | Free text can contain contact details: e-mail addresses appeared in 7 of 979 listed project descriptions (none among running or upcoming projects) and on 7 of 75 module pages [obs] | Ingest `description` only as the action summary. Do not log raw payloads or descriptions. Do not ingest module texts. |
| Project information and plan pages (HTML): contact block | `contact_name`, `contact_email`, `contact_phone`, `contact_address_text` of staff ([contact_person.html](https://github.com/liqd/a4-meinberlin/blob/f2eebf8a16dcf8d376b77f5d9529730877238589/meinberlin/apps/projects/templates/meinberlin_projects/includes/contact_person.html#L3-L30)) | Do not ingest. |
| Module and comment APIs (`/api/modules/<pk>/…`, `/api/contenttypes/…/comments/`) | Usernames, avatars, free-text contributions, ratings and poll answers of participants ([comment serializer](https://github.com/liqd/adhocracy4/blob/68a8ab3d3ea265ae674696a7fa7ec772bb750397/adhocracy4/comments_async/serializers.py#L35-L66)) | Never ingest. These are contributions to political participation by identifiable people ([004-data-and-privacy.md](../004-data-and-privacy.md)). |

This document and its examples contain no personal data.

## Trust-boundary risks for SRC-2

SRC-2's mandatory security review should cover at least the following:

- **Untrusted text.** `title` and `description` are free text entered by initiators. Encode them on output, normalize whitespace, and enforce the observed limits (120 and 250 characters). HTML entities occur. Never interpret the text as markup. The text can contain contact details, so do not log raw payloads.
- **Mixed and localized types.**
  - `active_phase` is either `false` or a three-element array.
  - `future_phase` and `past_phase` are either `false` or a string.
  - `point` can be `null`.
  - `participation_string` and `active_phase[1]` are German display strings.

  Parse only the typed fields, and reject or report records that do not match.
- **URLs.**
  - On-site URLs are relative. Resolve them only against the fixed base `https://mein.berlin.de`.
  - Reject protocol-relative or unexpected absolute URLs.
  - Accept an absolute development-plan URL only with `https` and the exact allowlisted host `be.beteiligung.diplanung.de` (owner decision, 2026-10-05).
  - Do not follow cross-host redirects automatically.
- **Hosts.** Connect only to `mein.berlin.de`. Do not fetch images.
- **Size limits.** The lists are unpaginated and grow with the number of projects. Set a response-size limit well above today's roughly 40 KB, and fail visibly when it is exceeded.
- **Schema drift.** The endpoints are unversioned and built from a core library installed from a floating branch. Expect field changes without notice. Contract tests with synthetic fixtures and visible parse errors are required.
- **Availability.** There is no service level. Use timeouts and limited retries with backoff. Keep the last good snapshot instead of deleting actions when a poll fails.
- **Credentials.** None are needed. Do not use Basic or session authentication against the API.

## Recommendation

**Primary path.** Access level 3, as recorded in [ADR-0003](../adr/0003-meinberlin-source-access-path.md) (Accepted).

**Endpoints.**
- `GET https://mein.berlin.de/api/projects/?status=activeParticipation`
- `GET https://mein.berlin.de/api/projects/?status=futureParticipation`

**Granularity.** One CivicAction per project, identified by `id`.

**Route.** The project page, or an allowlisted development-plan URL.

**Polling.** At most hourly and sequential, with an identifying User-Agent.

**Fields ingested.** Only `id`, `title`, `description`, `district`, `organisation`, `topics` (if a later slice needs them), the phase dates, `url` and `subtype`.

**Reasons.**
- These are the only structured, complete, credential-free data with a stable ID.
- They are small, cheap to poll, and already used by the platform's own overview.
- robots.txt does not exclude them.
- No better official path exists.

**Conditions before live use.**
- The owner clarifies with the operator, or decides explicitly, three questions: automated reading, display with attribution, and caching or storage of the normalized fields.
- The contacts are the Senatskanzlei, Landesredaktion Berlin.de, as publisher, and Liquid Democracy e.V. as technical operator ([imprint](https://mein.berlin.de/impressum/)).
- Until then, SRC-2 is built and tested against synthetic fixtures only.

**Not recommended.**
- HTML parsing of project or module pages (a separate owner decision).
- `/api/extprojects/`: no ID, 70 hosts, `http` URLs.
- `/api/plans/`: planning projects rather than participation opportunities, no ID.
- Module and user-content APIs: personal data.

**Alternative** (if the operator does not permit automated use, or the endpoints disappear):
1. Ask the operator for an official feed or export, or an agreement covering the endpoint above. If an official feed appears, prefer it and supersede ADR-0003.
2. Until then, keep the M1 loop running on the SRC-1 fixture connector with synthetic data.
3. The owner decides whether a small, editorially curated set of meinBerlin actions (title, dates and link entered by hand) is acceptable for M1.

Other sources may be considered in separate slices; they are not investigated here.

## Impact on follow-up slices

These are inputs only. Each follow-up issue is updated during its own `/slice-bootstrap`.

### ACT-1 (#16)

| Item | Input from this investigation |
| --- | --- |
| ExternalIdentity | Source key plus the project's integer `id`, stored as a string. |
| Title | Up to 120 characters, plain text. |
| Summary | Available as `description`: plain text, up to 250 characters. |
| ParticipationWindow | Both bounds are optional. Running projects have no start; upcoming projects have no end. When both are present, start ≤ end. Values are `DateTimeOffset` with the Berlin offset. No separate `Deadline` is needed. Offline events produce start = end, so the boundary case already in the ACT-1 acceptance criteria does occur. |
| ParticipationType | Not delivered by the API. meinBerlin records need the explicit "unknown" value unless a later decision approves another source. The module-type list in [Participation types](#participation-types) can inform the taxonomy. An upcoming record may be an in-person event only; the API cannot tell. |
| Locality | A closed list of 12 districts plus city-wide is available. There is no finer locality. |
| ResponsibleOrganization | Available as a name, with no identifier. |
| OfficialParticipationRoute | The project page, or an allowlisted `https` development-plan URL. Module URLs are not available at level 3. |

### SRC-1 (#20)

- Use synthetic fixtures that copy the project-list structure.
- Include these edge cases:
  - `active_phase` = `false`;
  - `future_phase` only;
  - `point` = `null`;
  - "Gesamtstädtisch";
  - a development plan with an absolute external URL;
  - an `http` or foreign-host URL, which must be rejected;
  - an over-long title;
  - a missing `id`.
- Do not use real titles, descriptions or images.

### SRC-2 (#21)

- Endpoints, polling etiquette, URL resolution and host allowlist as recommended above.
- Response-size limit and parse-error reporting.
- "No longer listed" semantics for projects that disappear.
- Full-snapshot change detection with a per-record Source Hash.
- The preconditions for live use.
- The security-review topics in [Trust-boundary risks for SRC-2](#trust-boundary-risks-for-src-2).

### ACT-3 (#18)

- The source provides no duration, and the participation type is unknown for meinBerlin. The effort policy needs a documented default for "unknown", which is an owner decision.
- `DeadlineUrgency` can use the end of the running phase.
- `Freshness` should not rely on `created_or_modified`.
- `LocalityFit` can use the district.

### EXPL-0 (#26)

- No meinBerlin text or page may be stored as a SourceDocument until the storage and reuse rights are clarified.
- Candidate documents are the information, results, module and plan pages, all of which are HTML.

### Also relevant: WEB-3 (#25)

- Participation usually requires a meinBerlin account.
- The handoff target is the project page or the Diplan page.

## Open owner decisions

1. **Rights.** Clarify with the operator automated reading of the list endpoints, display of title, description and link with attribution, caching or storing the normalized fields, and storing texts as SourceDocuments. This is required before live polling or display.
2. **ADR-0003.** Accepted by the owner on 2026-10-05.
3. **Participation type.** Decided on 2026-10-05: meinBerlin records use "unknown" until the question is clarified. No HTML parsing. Still to clarify: whether the operator can expose the module type in the API (tracked in #43).
4. **Allowlist.** Decided on 2026-10-05: DiPlanBeteiligung Berlin is an official participation route, and development plans are included in M1. The allowlist contains exactly `be.beteiligung.diplanung.de`, `https` only, with no wildcard (see [Hosts of participation URLs](#project-versus-participation-module)). SRC-2 implements it.
5. **City-wide.** Decide whether "Gesamtstädtisch" maps to a city-wide `Locality` value (ACT-1).
6. **Open Data.** Optionally confirm in a browser that daten.berlin.de has no meinBerlin dataset (automated search was blocked).
