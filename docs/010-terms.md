# Shared terms

Use these terms consistently in code, documentation, issues, and user-facing copy. Add or revise entries when an accepted domain decision establishes their meaning.

| Term | Current meaning |
| --- | --- |
| Action loop | The intended journey: select a time budget, review relevant actions, open a detail, then follow the official participation route. |
| Circle | A private, invitation-based group for social coordination around civic participation. It is a separate aggregate and does not own CivicActions. |
| CivicAction | The primary Participation aggregate: a globally discoverable civic opportunity derived from an external source. Its accepted boundaries are recorded in [ADR-0001](adr/0001-participation-bounded-context-and-aggregate.md); its full schema and lifecycle are not defined here. |
| Explanation | Derived presentation information about a CivicAction; it is not authoritative domain state under [ADR-0001](adr/0001-participation-bounded-context-and-aggregate.md). |
| Official participation route | The external service or process provided by the responsible organization through which a person participates. |
| SharedAction | A conceptual Circle-to-CivicAction reference for sharing an existing action; it does not duplicate or modify CivicAction state. Detailed behavior is deferred to a separately approved Circle slice. |
| Source connector | An adapter that reads an external source and maps its records to CivicAction's normalized source boundary. |
| Slice | A small, independently reviewable unit of work with explicit scope, acceptance criteria, and a verifiable outcome. |

Individual participation-completion records and Circle activity summaries are not approved data requirements; do not infer that they are collected or persisted.

Do not use a term to imply behavior or a decision that has not been accepted and implemented.
