# Architecture decision records

Architecture Decision Records (ADRs) capture decisions with lasting technical or domain consequences. An ADR records context, the decision, consequences, and alternatives; it does not replace the product issue or implementation contract.

## Creating and changing an ADR

1. Discuss the decision in a scoped issue or slice.
2. Add `docs/adr/NNNN-short-title.md` from the template below.
3. Link the issue, affected source documents, and relevant implementation.
4. Set the status to `Proposed` until the decision is accepted. Do not implement a proposed choice as if settled.
5. Update this index and any affected source-of-truth documents when the ADR is accepted, superseded, or retired.
6. Do not rewrite historical decisions to hide a change; create a superseding ADR and link both records.

## Index

| ADR | Title | Status |
| --- | --- | --- |
| [0001](adr/0001-participation-bounded-context-and-aggregate.md) | Participation bounded context and aggregate | Accepted |
| [0003](adr/0003-meinberlin-source-access-path.md) | meinBerlin source access path | Accepted |

## ADR template

```markdown
# ADR-NNNN: Short title

- Status: Proposed
- Date: YYYY-MM-DD
- Issue: #NN
- Supersedes: (optional)

## Context
What problem or constraint requires a decision?

## Decision
What has been decided? State the choice precisely.

## Consequences
What improves, what becomes harder, and what follow-up is required?

## Alternatives considered
Which reasonable alternatives were considered and why were they not selected?
```

ADR 0001 is a placeholder, not an accepted decision. Replace its pending sections with the decision slice's outcome and update its status when the decision is accepted.
