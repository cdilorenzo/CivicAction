# Quality and testing

## Quality baseline

The solution enables nullable reference types, .NET analyzers at `latest-recommended`, and warnings-as-errors. Preserve these settings and address warnings rather than suppressing them broadly.

The expected deterministic CI baseline is:

1. Restore the solution.
2. Verify formatting with `dotnet format --verify-no-changes --severity info`.
3. Build the solution.
4. Run the test suite.
5. Validate the slice's PR contract.

The exact automation is established in the CI slice; until then, run the equivalent local commands from [AGENTS.md](../AGENTS.md).

## Test guidance

- Unit-test domain invariants and application behavior at their narrowest useful boundary.
- Use integration tests for meaningful infrastructure and host wiring.
- Keep tests deterministic, isolated, and independent of public services and live source data.
- Cover success, invalid input, boundary conditions, and surfaced failures for changed behavior.
- Add regression coverage for bug fixes.
- UI changes should include appropriate interaction/accessibility checks.

Do not weaken or skip an existing check to make a change pass without documenting the reason and scope.
