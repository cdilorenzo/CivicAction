# Slice contract

A slice is the smallest unit of independently understandable and verifiable project work. The issue owns the goal and acceptance criteria; the PR proves how those criteria were met.

## Issue naming and hierarchy

- Epics use `EPIC <AREA> - <outcome>` and the `epic` plus `epic-<area>` labels.
- Slices use `<AREA>-<N>: <outcome>` and the `slice` plus `epic-<area>` labels.
- A slice has one parent epic. Record blocking prerequisites under dependencies; a parent is not automatically a dependency.
- Keep one independently reviewable deliverable per slice. Split work when parts have different outcomes, decision owners, or completion criteria.

## Issue requirements

Every slice issue must state:

- A product or technical goal with a clear outcome.
- In-scope work and explicit out-of-scope work.
- Testable acceptance criteria.
- Parent epic and actual prerequisites; do not list a dependency unless it blocks the slice.
- Relevant security/privacy review needs.
- Known constraints, open decisions, and the expected artifact or evidence.

Use the GitHub slice issue form and the [slice template](slices/TEMPLATE.md). Use `/slice-bootstrap <ISSUE-NUMBER>` to expand the issue specification where that workflow is available; generated detail does not replace review of the contract.

## Implementation requirements

- Work only within the approved scope. If new information changes the scope or an acceptance criterion, update the issue and agree on the change before implementing it.
- Keep the change coherent and reviewable. Include tests and documentation updates required by the behavior or decision.
- Report blocked acceptance criteria or external assumptions explicitly; do not hide them behind a success-shaped fallback.
- For investigation slices, deliver the specified findings/decision artifact rather than speculative product code.

## Pull request requirements

- Link the slice issue and state which acceptance criteria are satisfied.
- Summarize the implementation and any intentional behavior change.
- List exact validation commands and their results; identify checks that were not run.
- Note security/privacy, accessibility, localization, migrations, and operational impact where relevant.
- Identify unresolved risks or follow-up work. Do not claim completion while a required criterion is unmet.

## Completion

A slice is complete when its acceptance criteria are met, relevant checks pass or are explicitly dispositioned, required docs/ADRs are updated, and the PR is merged. Parent/dependency tracking remains in the issue tracker.
