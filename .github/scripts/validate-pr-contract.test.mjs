import test from 'node:test';
import assert from 'node:assert/strict';
import { validatePullRequestBody } from './validate-pr-contract.mjs';

const validPullRequestBody = `## Summary

Added the CI workflow and PR contract validation.

## Slice

- Issue: #14
- Acceptance criteria satisfied:
  - [x] CI runs the required quality checks.

## Validation

- \`dotnet test CivicAction.slnx --no-build\` - passed.

## Impact and review

- [x] Documentation and ADRs updated where needed.
- [x] Security/privacy impact considered; review completed where required.
- [x] Accessibility and localization considered for UI changes.
- [x] Operational, migration, and compatibility impacts documented where relevant.
- [x] Known limitations and follow-up work are listed.
`;

test('accepts a completed PR contract', () => {
  assert.deepEqual(validatePullRequestBody(validPullRequestBody), []);
});

test('reports missing issue, acceptance, summary, validation, and review details', () => {
  const errors = validatePullRequestBody(`## Summary

<!-- What changed and why? -->

## Slice

- Issue: #
- Acceptance criteria satisfied:
  - [ ] <criterion>

## Validation

- <command> - <result>

## Impact and review

- [ ] Security/privacy impact considered; review completed where required.
`);

  assert.equal(errors.length, 9);
  assert.match(errors[0], /link the slice issue/);
  assert.match(errors[1], /checked, described acceptance criterion/);
  assert.match(errors[2], /Summary section/);
  assert.match(errors[3], /exact command and its result/);
  assert.match(errors[4], /Documentation and ADRs/);
  assert.match(errors[5], /Security\/privacy impact/);
  assert.match(errors[6], /Accessibility and localization/);
  assert.match(errors[7], /Operational, migration, and compatibility/);
  assert.match(errors[8], /Known limitations and follow-up/);
});

test('rejects unchecked impact review fields', () => {
  const body = validPullRequestBody.replace(
    '- [x] Known limitations and follow-up work are listed.',
    '- [ ] Known limitations and follow-up work are listed.',
  );

  assert.deepEqual(validatePullRequestBody(body), [
    'Impact and review section must complete: "Known limitations and follow-up work are listed."',
  ]);
});
