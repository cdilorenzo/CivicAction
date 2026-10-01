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

Use the GitHub slice issue form and the [slice template](slices/TEMPLATE.md). Use `/slice-bootstrap` with a new goal or existing issue number to draft/expand the specification; generated detail does not replace review of the contract.

## End-to-end slice lifecycle

1. **Draft:** Use `/slice-bootstrap` for a new goal or existing issue. It prepares the complete issue text and flags unknowns; it does not implement the slice.
2. **Approve and create/update:** A human reviews the exact title, body, and labels before the assistant creates or materially updates the issue. A new slice receives its classification labels (`slice` and `epic-<area>`), but not the readiness label. Add `ready-for-implementation` only after its goal, scope, and acceptance criteria are approved and its blocking dependencies are closed or explicitly removed.
3. **Start implementation:** `/slice-implementer <ISSUE-NUMBER>` must run `node .github/scripts/validate-slice-readiness.mjs <ISSUE-NUMBER>`. The check requires the readiness label, complete issue sections, a parent, and verified closed blocking dependencies. Stop if it fails. Create the linked branch with:

   ```text
   gh issue develop <ISSUE-NUMBER> --repo cdilorenzo/CivicAction --base main --name slice/<ISSUE-NUMBER>-<short-slug> --checkout
   ```

   This records the branch against the issue as well as checking it out. If the branch already exists, verify its issue association instead of creating a competing branch.
4. **Implement and validate:** Make only the approved changes, run focused tests and the repository quality checks, and report failures or checks that discover no tests.
5. **Prepare:** `/slice-publish <PR-NUMBER>` checks the implementation, acceptance-criteria evidence, validation, impact, and limitations. It prepares findings/PR content but does not push, open a PR, or merge.
6. **Open the PR:** `/slice-open-pr <ISSUE-NUMBER>` verifies the issue-linked branch and prepared evidence, presents the final title/body, and asks for explicit approval before pushing or opening the PR. The PR body must include `Fixes #<ISSUE-NUMBER>`, map acceptance criteria to evidence, and list exact validation results.
7. **Review and merge:** GitHub Actions reports quality, dependency, and CodeQL results. A human reviews the change and merges it. The closing keyword closes the slice issue on merge. Actions and assistant workflows do not approve or merge automatically.

Use repository issue and PR forms as scaffolding, not approval signals. The only implementation-readiness signal is the `ready-for-implementation` label. A label alone does not override missing criteria, open blockers, or a later scope change.

### GitHub CLI fallbacks

- Create a reviewed slice issue: `gh issue create --repo cdilorenzo/CivicAction --title "AREA-N: outcome" --label slice --label "epic-<area>" --body-file "<approved-body-file>"`.
- Update an existing issue only after approval of the proposed changes: `gh issue edit "<ISSUE-NUMBER>" --repo cdilorenzo/CivicAction --body-file "<approved-body-file>"`.
- Mark an approved, unblocked issue ready: `gh issue edit "<ISSUE-NUMBER>" --repo cdilorenzo/CivicAction --add-label ready-for-implementation`.
- Verify issue-linked branches: `gh issue develop --list "<ISSUE-NUMBER>" --repo cdilorenzo/CivicAction`.
- Open a reviewed PR after explicit approval: `gh pr create --repo cdilorenzo/CivicAction --base main --head "<issue-linked-branch>" --title "<approved-title>" --body-file "<approved-pr-body-file>"`.

GitHub mutations (issue writes, readiness labels, pushes, and PR creation) require explicit user/owner approval of the proposed operation. The assistant must not infer approval from a request to draft or review. Repository settings such as branch protection remain owner-managed.

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
- State which required reviews and checks are complete or still pending; do not imply review or CI completion before evidence exists.
- Identify unresolved risks or follow-up work. Do not claim completion while a required criterion is unmet.

## Completion

A slice is complete when its acceptance criteria are met, relevant checks pass or are explicitly dispositioned, required docs/ADRs are updated, and the PR is merged. Parent/dependency tracking remains in the issue tracker.
