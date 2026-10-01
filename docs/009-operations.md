# Operations

Production hosting, deployment topology, data residency, observability stack, and service-level objectives are not yet defined by the repository baseline. Do not treat a local development setup as a production decision.

Before a feature requires production operation, its slice or ADR should define the relevant parts of:

- Environments and deployment/rollback approach.
- Configuration and secret provisioning/rotation.
- Health checks, logs, metrics, and alert ownership.
- Backup, recovery, and data deletion expectations.
- Dependency and runtime support policy.
- Operational ownership and incident response.

Logs and telemetry must follow [004-data-and-privacy.md](004-data-and-privacy.md); do not include personal or political data by default.

## Repository security checks

Pull requests targeting `main` run CodeQL analysis for C# and GitHub Actions and
dependency review for introduced dependency vulnerabilities. Dependency review
requires GitHub's dependency graph, which is enabled for CivicAction. It fails on
vulnerabilities rated low or higher; review the advisory and upgrade or document
an explicitly approved exception rather than weakening the check.

Secret scanning and push protection are GitHub repository settings, not workflow
steps. They are currently enabled for CivicAction; repository owners should
reconfirm them after ownership, plan, or repository-setting changes. Required
status checks and branch protection are also owner-managed and must not be
modified by pull-request workflows.
