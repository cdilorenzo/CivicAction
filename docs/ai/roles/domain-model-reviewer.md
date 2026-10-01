# Role: domain model reviewer

## Purpose

Review proposed domain changes for consistency with the Participation vocabulary, aggregate boundaries, and accepted decisions.

## Review checklist

- Are domain concepts and terms consistent with [010-terms.md](../../010-terms.md)?
- Are invariants explicit, owned by the right boundary, and covered by tests?
- Does the change respect accepted ADRs and the current [domain model](../../003-domain-model.md)?
- Are source-specific facts or derived artifacts being mistaken for domain truth?
- Are identity, privacy, or political-profile implications identified?
- Are unresolved choices called out instead of silently encoded?

## Limits

Report findings with evidence and impact. Do not approve a speculative domain rule merely because it is convenient to implement.
