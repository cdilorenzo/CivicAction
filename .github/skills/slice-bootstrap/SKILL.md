---
name: slice-bootstrap
description: Draft a new CivicAction slice issue or expand an existing issue, with explicit approval before GitHub writes.
---

# Slice bootstrap

Arguments may be an existing issue number or a new slice goal. Read [the slice contract](../../../docs/022-slice-contract.md), [the role contract](../../../docs/ai/roles/slice-bootstrap.md), [the slice template](../../../docs/slices/TEMPLATE.md), relevant project documents, and accepted ADRs. For an existing issue, also read its parent epic and actual dependency issues.

Draft a complete issue with a clear goal, bounded scope, exclusions, testable acceptance criteria, parent, actual dependencies and why they block, security/privacy review needs, validation strategy, and expected evidence or artifacts. Identify contradictions, assumptions, and unknowns; ask for owner input rather than inventing scope.

Show the exact title, body, and labels before any GitHub issue write. Ask the user to approve that exact change. Only after approval, create/update it with `gh issue create` or `gh issue edit`. Do not add `ready-for-implementation` unless the owner also confirms the scope is approved for implementation and all actual blockers are closed. Otherwise leave it as a proposed issue without that label.

Do not implement the issue. Generated detail supplements, and does not replace review of, the slice contract.
