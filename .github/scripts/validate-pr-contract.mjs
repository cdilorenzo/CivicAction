import { readFileSync } from 'node:fs';
import { resolve } from 'node:path';
import { pathToFileURL } from 'node:url';

const requiredImpactChecks = [
  'Documentation and ADRs updated where needed.',
  'Security/privacy impact considered; review completed where required.',
  'Accessibility and localization considered for UI changes.',
  'Operational, migration, and compatibility impacts documented where relevant.',
  'Known limitations and follow-up work are listed.',
];

function extractSection(body, heading) {
  const uncommentedBody = body.replace(/<!--[\s\S]*?-->/g, '');
  const escapedHeading = heading.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
  const headingPattern = new RegExp(
    `^##[ \\t]+${escapedHeading}[ \\t]*\\r?\\n([\\s\\S]*?)(?=^##[ \\t]+|(?![\\s\\S]))`,
    'im',
  );

  return headingPattern.exec(uncommentedBody)?.[1]?.trim() ?? '';
}

export function validatePullRequestBody(body) {
  const errors = [];
  const slice = extractSection(body, 'Slice');
  const summary = extractSection(body, 'Summary');
  const validation = extractSection(body, 'Validation');
  const impact = extractSection(body, 'Impact and review');

  if (!/#\d+|https:\/\/github\.com\/[^/\s)]+\/[^/\s)]+\/issues\/\d+/i.test(slice)) {
    errors.push('Slice section must link the slice issue (for example, #14).');
  }

  if (
    !/Acceptance criteria satisfied:/i.test(slice)
    || !/^\s*-\s*\[x\]\s+(?!<criterion>)[^\s].*$/im.test(slice)
  ) {
    errors.push('Slice section must include at least one checked, described acceptance criterion.');
  }

  if (!summary) {
    errors.push('Summary section must describe the change and any behavior change.');
  }

  const hasCommandAndResult = validation.split(/\r?\n/).some((line) => {
    const match = /^\s*-\s*(.+?)\s+-\s+(.+?)\s*$/.exec(line);
    return match
      && !/^<command>$/i.test(match[1].trim())
      && !/^<result>$/i.test(match[2].trim());
  });

  if (!hasCommandAndResult) {
    errors.push('Validation section must list at least one exact command and its result as "- command - result".');
  }

  for (const label of requiredImpactChecks) {
    const escapedLabel = label.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    const checkPattern = new RegExp(`^\\s*-\\s*\\[([ xX])\\]\\s*${escapedLabel}\\s*$`, 'im');
    const match = checkPattern.exec(impact);

    if (!match || match[1].toLowerCase() !== 'x') {
      errors.push(`Impact and review section must complete: "${label}"`);
    }
  }

  return errors;
}

function main(eventPath) {
  if (!eventPath) {
    throw new Error('Pass the GitHub event JSON path as the first argument.');
  }

  const event = JSON.parse(readFileSync(eventPath, 'utf8'));
  const body = event.pull_request?.body;

  if (typeof body !== 'string') {
    throw new Error('GitHub event does not contain a pull request body.');
  }

  const errors = validatePullRequestBody(body);
  if (errors.length > 0) {
    console.error('Slice PR Contract failed:');
    for (const error of errors) {
      console.error(`- ${error}`);
    }
    process.exitCode = 1;
    return;
  }

  console.log('Slice PR Contract passed.');
}

if (process.argv[1] && import.meta.url === pathToFileURL(resolve(process.argv[1])).href) {
  main(process.argv[2]);
}
