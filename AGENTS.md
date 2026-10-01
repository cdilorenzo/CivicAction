# CivicAction repository guidance

## Start here

1. Read [docs/PROJECT-MEMORY.md](docs/PROJECT-MEMORY.md).
2. Read the numbered product or architecture documents relevant to the change.
3. For slice work, read [docs/022-slice-contract.md](docs/022-slice-contract.md) and the linked issue.
4. For architecture decisions, read [docs/021-adr-index.md](docs/021-adr-index.md) and the relevant ADR.

## Working agreements

- Follow the end-to-end slice lifecycle in [docs/022-slice-contract.md](docs/022-slice-contract.md): draft and approve an issue, mark it `ready-for-implementation`, create an issue-linked branch, implement, validate, prepare the PR, and get human review before merge.
- Do not create or materially edit GitHub issues, push a branch, or open a pull request until the user has approved the exact proposed content and action. Never merge autonomously.
- Keep changes within the approved issue scope. Do not add product behavior to a foundation or investigation slice.
- Treat accepted ADRs as the record of settled architecture decisions. Update the relevant source document when a decision changes.
- Do not infer political affiliation or build political profiles. Collect only data required for a documented product purpose; see [docs/004-data-and-privacy.md](docs/004-data-and-privacy.md).
- Preserve the source-connector boundary and keep source-specific behavior out of the core domain unless an approved decision says otherwise.
- Keep nullable reference types and the repository's analyzer and warnings-as-errors settings enabled. Do not suppress warnings without a documented, narrow reason.
- Add or update focused tests for behavior changes. Keep tests deterministic and independent of live external sources.
- Update documentation when behavior, an architecture decision, operational expectations, or a public contract changes.
- Make accessibility and localization part of UI changes, not follow-up work.

## Validation

Run the smallest applicable checks while iterating. Before publishing a change, run:

```text
dotnet restore CivicAction.slnx
dotnet format CivicAction.slnx --verify-no-changes --severity info
dotnet build CivicAction.slnx --no-restore
dotnet test CivicAction.slnx --no-build
```

If a check cannot run, report the exact command and the reason; do not describe an unrun check as passing.

## Completion

Use the pull request template. Link the issue, describe the behavior or decision changed, record tests and limitations, and confirm the slice contract. Do not mark a slice complete while a required acceptance criterion remains unmet.
