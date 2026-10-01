# Quality and testing

## Quality baseline

The solution enables nullable reference types, .NET analyzers at `latest-recommended`, and warnings-as-errors. Preserve these settings and address warnings rather than suppressing them broadly.

The deterministic CI baseline runs on pull requests targeting `main` when opened,
edited, synchronized, or reopened. It uses .NET SDK `10.0.400` and runs:

1. Restore the solution.
2. Verify formatting with
   `dotnet format CivicAction.slnx --verify-no-changes --severity info`.
3. Build the solution.
4. Run the test suite.
5. Validate the slice's PR contract with
   `.github/scripts/validate-pr-contract.mjs`.
6. Review dependency changes and run CodeQL security analysis using
   `.github/workflows/security-review.yml`.

The PR contract check requires a linked slice issue and matching `Fixes #N`
closing reference, at least one checked acceptance criterion, a non-empty
summary, an exact validation command and result, and a completed impact/review
checklist that records review status (including pending work) in the PR template.
Branch-protection and required-status settings remain owner-managed. Run the
equivalent local .NET commands from [AGENTS.md](../AGENTS.md).

## Test guidance

- Unit-test domain invariants and application behavior at their narrowest useful boundary.
- Use integration tests for meaningful infrastructure and host wiring.
- Keep tests deterministic, isolated, and independent of public services and live source data.
- Cover success, invalid input, boundary conditions, and surfaced failures for changed behavior.
- Add regression coverage for bug fixes.
- UI changes should include appropriate interaction/accessibility checks.

Do not weaken or skip an existing check to make a change pass without documenting the reason and scope.
