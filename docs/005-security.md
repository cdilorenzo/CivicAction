# Security

## Baseline

- Treat all imported source content, URLs, and user-controlled values as untrusted.
- Validate inputs at system boundaries and use safe, framework-supported output encoding.
- Keep credentials and secrets out of source control, logs, fixtures, and issue/PR text.
- Use least privilege for credentials and service identities; do not introduce credentials until their handling and rotation are documented.
- Avoid logging personal data, political-interest data, tokens, or source payloads containing sensitive information.
- Keep dependencies and supported platform versions current under the project's maintenance process.

## Review triggers

Request a security review before introducing or materially changing:

- User authentication, authorization, identity linking, or personal data collection.
- Fetching user-supplied URLs or parsing untrusted documents.
- Credentials, privileged integrations, or new externally reachable endpoints.
- Storage or transfer of sensitive data.
- A new trust boundary or security-sensitive dependency.

Describe the threat surface, trust boundaries, mitigations, and remaining risks in the issue/PR. Do not treat this baseline as a substitute for a threat model or security review.
