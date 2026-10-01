import { execFileSync } from 'node:child_process';
import { resolve } from 'node:path';
import { pathToFileURL } from 'node:url';

const requiredSections = [
  ['Product/technical goal'],
  ['Scope'],
  ['Out of scope'],
  ['Acceptance criteria'],
  ['Parent', 'Parent epic'],
  ['Dependencies'],
  ['Security/privacy review'],
  ['Specification and open questions', 'Specification'],
  ['Validation'],
  ['Expected evidence or artifacts', 'Expected evidence'],
];

function extractSection(body, headings) {
  const uncommentedBody = body.replace(/<!--[\s\S]*?-->/g, '');

  for (const heading of headings) {
    const escapedHeading = heading.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    const pattern = new RegExp(
      `^#{2,3}[ \\t]+${escapedHeading}[ \\t]*\\r?\\n([\\s\\S]*?)(?=^#{2,3}[ \\t]+|(?![\\s\\S]))`,
      'im',
    );
    const match = pattern.exec(uncommentedBody);
    if (match) {
      return match[1].trim();
    }
  }

  return '';
}

function hasMeaningfulContent(section) {
  const content = section
    .replace(/<!--[\s\S]*?-->/g, '')
    .replace(/^\s*(?:[-*]\s*)?<[^>\r\n]+>\s*$/gm, '')
    .trim();

  return content.length > 0;
}

function getLabels(issue) {
  return (issue.labels ?? []).map((label) => (
    typeof label === 'string' ? label : label.name
  ));
}

function getDeclaredDependencyNumbers(body) {
  const dependencies = extractSection(body, ['Dependencies']);
  return [...new Set([...dependencies.matchAll(/#(\d+)\b/g)].map((match) => Number(match[1])))];
}

function getDeclaredParentNumber(body) {
  const parent = extractSection(body, ['Parent', 'Parent epic']);
  const match = /#(\d+)\b/.exec(parent);
  return match ? Number(match[1]) : undefined;
}

export function validateSliceReadiness(issue, blockingIssues = [], parentIssue) {
  const errors = [];

  if (!issue || !Number.isSafeInteger(Number(issue.number)) || Number(issue.number) < 1) {
    return ['Issue data must include a numeric issue number.'];
  }

  if (String(issue.state).toUpperCase() !== 'OPEN') {
    errors.push(`Slice #${issue.number} must be open before implementation.`);
  }

  if (!getLabels(issue).includes('ready-for-implementation')) {
    errors.push('Add the ready-for-implementation label only after owner approval.');
  }

  if (typeof issue.body !== 'string') {
    return [...errors, `Slice #${issue.number} has no issue body.`];
  }

  for (const headings of requiredSections) {
    const section = extractSection(issue.body, headings);
    if (!hasMeaningfulContent(section)) {
      errors.push(`Issue #${issue.number} needs a completed "${headings[0]}" section.`);
    }
  }

  const acceptance = extractSection(issue.body, ['Acceptance criteria']);
  if (!/^\s*[-*]\s*\[[ xX]\]\s+(?!<criterion>)[^\s].*$/im.test(acceptance)) {
    errors.push(`Issue #${issue.number} needs at least one described acceptance criterion.`);
  }

  const parentNumber = getDeclaredParentNumber(issue.body);
  if (parentNumber === undefined) {
    errors.push(`Issue #${issue.number} must identify its parent epic.`);
  } else if (!parentIssue || Number(parentIssue.number) !== parentNumber) {
    errors.push(`Parent epic #${parentNumber} has not been verified.`);
  } else if (!getLabels(parentIssue).includes('epic')) {
    errors.push(`Parent issue #${parentNumber} must have the epic label.`);
  }

  const dependencyIssues = new Map(
    blockingIssues.map((dependency) => [Number(dependency.number), dependency]),
  );
  for (const dependencyNumber of getDeclaredDependencyNumbers(issue.body)) {
    const dependency = dependencyIssues.get(dependencyNumber);
    if (!dependency) {
      errors.push(`Blocking dependency #${dependencyNumber} has not been verified.`);
    } else if (String(dependency.state).toUpperCase() !== 'CLOSED') {
      errors.push(
        `Blocking dependency #${dependencyNumber} is not closed (state: ${dependency.state}).`,
      );
    }
  }

  return errors;
}

function getIssue(issueNumber) {
  return JSON.parse(execFileSync(
    'gh',
    ['issue', 'view', issueNumber, '--repo', 'cdilorenzo/CivicAction', '--json', 'number,state,labels,body'],
    { encoding: 'utf8' },
  ));
}

function main(issueNumber) {
  if (!/^[1-9]\d*$/.test(issueNumber ?? '')) {
    throw new Error('Pass a numeric slice issue number.');
  }

  const issue = getIssue(issueNumber);
  const parentNumber = getDeclaredParentNumber(issue.body ?? '');
  const parentIssue = parentNumber === undefined ? undefined : getIssue(String(parentNumber));
  const dependencyNumbers = getDeclaredDependencyNumbers(issue.body ?? '');
  const blockingIssues = dependencyNumbers.map((number) => getIssue(String(number)));
  const errors = validateSliceReadiness(issue, blockingIssues, parentIssue);

  if (errors.length > 0) {
    console.error(`Slice #${issueNumber} is not ready for implementation:`);
    for (const error of errors) {
      console.error(`- ${error}`);
    }
    process.exitCode = 1;
    return;
  }

  console.log(`Slice #${issueNumber} is ready for implementation.`);
}

if (process.argv[1] && import.meta.url === pathToFileURL(resolve(process.argv[1])).href) {
  main(process.argv[2]);
}
