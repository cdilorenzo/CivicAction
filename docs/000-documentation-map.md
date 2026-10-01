# Documentation map

This directory is the source of durable project context. Keep documents focused, link related decisions, and update the relevant document when an accepted decision changes. Documents that describe future work must distinguish an intended direction from implemented behavior.

## Product and technical references

| Document | Purpose |
| --- | --- |
| [001-product-purpose.md](001-product-purpose.md) | Product goal, current user journey, and scope boundaries |
| [002-architecture.md](002-architecture.md) | Solution structure and architectural boundaries |
| [003-domain-model.md](003-domain-model.md) | Participation domain vocabulary and open domain decisions |
| [004-data-and-privacy.md](004-data-and-privacy.md) | Data minimization, analytics constraints, and privacy decisions |
| [005-security.md](005-security.md) | Security expectations and review triggers |
| [006-accessibility-and-localization.md](006-accessibility-and-localization.md) | UI accessibility and localization baseline |
| [007-source-ingestion.md](007-source-ingestion.md) | Source connector boundary and ingestion expectations |
| [008-quality-and-testing.md](008-quality-and-testing.md) | Test strategy and quality checks |
| [009-operations.md](009-operations.md) | Operational decisions to document before production |
| [010-terms.md](010-terms.md) | Shared project terminology |

## Governance

- [PROJECT-MEMORY.md](PROJECT-MEMORY.md) is the short contributor/assistant orientation.
- [021-adr-index.md](021-adr-index.md) indexes architecture decision records.
- [022-slice-contract.md](022-slice-contract.md) defines issue-to-PR slice expectations.
- [slices/](slices/) contains the slice template and workflow notes.
- [ai/roles/](ai/roles/) contains role contracts for AI-assisted project work.
- [../AGENTS.md](../AGENTS.md) contains repository-wide contribution instructions.

## Editing rules

- Put decisions in the document that owns the topic; link rather than duplicate.
- Label proposals and unknowns as such. A roadmap item is not an implemented feature.
- Use ADRs for decisions with lasting architectural consequences. Update the index when adding or changing an ADR.
- Prefer concrete, testable acceptance criteria in issues and PRs.
