---
name: slice-open-pr
description: Open a CivicAction slice pull request after validating its issue link, evidence, and user-approved PR content.
---

# Open a slice pull request

Read [the slice contract](../../../docs/022-slice-contract.md), [the PR template](../../pull_request_template.md), the approved issue, and the prepared implementation evidence.
Follow the [slice PR opener role contract](../../../docs/ai/roles/slice-open-pr.md).

Before acting:

- Run `node .github/scripts/validate-slice-readiness.mjs <ISSUE-NUMBER>` and stop if readiness, parent, required fields, or blockers fail validation.
- Verify the issue has `ready-for-implementation`, actual blocking dependencies are closed, the current branch is linked to the issue with `gh issue develop --list <ISSUE-NUMBER>`, and the branch contains the reviewed implementation.
- Check whether a PR already exists for the branch. Do not create a duplicate.
- Prepare a title and complete PR body that fills the repository PR template, includes `Fixes #<ISSUE-NUMBER>`, maps acceptance criteria to evidence, lists exact validation commands/results, and documents relevant review impacts and known limitations.
- Show the user the exact title, body, head branch, and base branch (`main`). Ask for explicit approval before pushing or creating the PR. A prior request to draft, review, or publish evidence is not approval of a specific PR body.

After approval, push the issue-linked branch if necessary and create the PR with GitHub CLI. Report the resulting URL and checks. Do not approve or merge the PR. Never bypass failing or pending required checks.
