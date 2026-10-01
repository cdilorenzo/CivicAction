import test from 'node:test';
import assert from 'node:assert/strict';
import { validateSliceReadiness } from './validate-slice-readiness.mjs';

const readySlice = {
  number: 33,
  state: 'OPEN',
  labels: [{ name: 'slice' }, { name: 'ready-for-implementation' }],
  body: `## Product/technical goal

Deliver the approved slice lifecycle.

## Scope

- Update repository guidance.

## Out of scope

- Product behavior.

## Acceptance criteria

- [ ] Each handoff is documented and verifiable.

## Parent

- Epic: #1

## Dependencies

- #14 - the CI baseline must be available.

## Security/privacy review

- Required: review workflow permissions.

## Specification and open questions

- No unresolved owner decisions.

## Validation

- Run focused checks.

## Expected evidence or artifacts

- Workflow and tests.
`,
};

const parentEpic = {
  number: 1,
  state: 'OPEN',
  labels: [{ name: 'epic' }, { name: 'epic-dx' }],
};

test('accepts an approved slice with closed dependencies and complete sections', () => {
  assert.deepEqual(
    validateSliceReadiness(readySlice, [{ number: 14, state: 'CLOSED' }], parentEpic),
    [],
  );
});

test('rejects a slice without the implementation-readiness label', () => {
  const issue = {
    ...readySlice,
    labels: [{ name: 'slice' }],
  };

  assert.deepEqual(
    validateSliceReadiness(issue, [{ number: 14, state: 'CLOSED' }], parentEpic),
    ['Add the ready-for-implementation label only after owner approval.'],
  );
});

test('rejects missing issue sections and unresolved dependency states', () => {
  const issue = {
    ...readySlice,
    body: readySlice.body.replace(
      '## Security/privacy review\n\n- Required: review workflow permissions.\n\n',
      '',
    ),
  };

  const errors = validateSliceReadiness(issue, [{ number: 14, state: 'OPEN' }], parentEpic);
  assert.deepEqual(errors, [
    'Issue #33 needs a completed "Security/privacy review" section.',
    'Blocking dependency #14 is not closed (state: OPEN).',
  ]);
});

test('rejects unresolved dependency and parent references', () => {
  assert.deepEqual(validateSliceReadiness(readySlice), [
    'Parent epic #1 has not been verified.',
    'Blocking dependency #14 has not been verified.',
  ]);
});

test('rejects a parent issue that is not labeled as an epic', () => {
  const parent = { ...parentEpic, labels: [{ name: 'slice' }] };

  assert.deepEqual(
    validateSliceReadiness(readySlice, [{ number: 14, state: 'CLOSED' }], parent),
    ['Parent issue #1 must have the epic label.'],
  );
});

test('accepts the GitHub issue form heading "Parent epic"', () => {
  const issue = {
    ...readySlice,
    body: readySlice.body.replace('## Parent\n', '### Parent epic\n'),
  };

  assert.deepEqual(
    validateSliceReadiness(issue, [{ number: 14, state: 'CLOSED' }], parentEpic),
    [],
  );
});
