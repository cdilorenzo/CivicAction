# Data and privacy

## Principles

- Collect and retain only data required for an explicit product purpose.
- Do not infer political beliefs, party affiliation, or political profiles.
- Prefer aggregate, privacy-preserving product measurement over person-level tracking.
- Do not collect raw free-form text for analytics. The participation/outcomes roadmap explicitly excludes raw text and party inference.
- Explain any data collection, retention, or sharing in the feature's issue and relevant design documentation before implementation.

These are design constraints, not proof that a particular implementation has already been reviewed or is compliant with applicable law.

## Decisions required before data collection

For each proposed data flow, document:

1. Purpose and minimum data fields.
2. Whether data can identify or be linked to a person.
3. Collection point, recipients, and any third parties.
4. Retention, deletion, and access controls.
5. User notice, consent or other applicable legal basis, as determined by qualified review.
6. Abuse and re-identification risks, including when combining datasets.

Do not introduce tracking, identifiers, or analytics SDKs without an approved issue and privacy/security review appropriate to the change.

## Open questions

Retention periods, hosting location, consent mechanics, and exact event schemas are not settled here. Resolve them before implementation in a scoped issue; record durable architecture decisions as ADRs.
