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

## Advisory AI code review

Pull requests the repository owner opens against `main` receive an automatic,
advisory code review from GitHub Copilot's native code review feature. This is
a GitHub platform capability configured through the owner's personal Copilot
settings (`github.com` profile picture menu -> Copilot settings -> Code
review); it is not a custom GitHub Actions workflow, and it introduces no new
secret, credential, or external AI provider.

Configuration:

- **Automatic Copilot code review**: enabled, so every pull request the owner
  authors against `main` is reviewed without a manual request.
- **Review new pushes**: enabled, so each additional commit to an open pull
  request receives an updated review.
- **Review effort level**: `Balanced`. This is a built-in GitHub Copilot tier,
  not a selectable model, that trades higher AI-credit cost for deeper
  analysis of complex logic, security-sensitive code, and cross-service
  changes; it consumes more AI credits than the `Lite` tier.

By default, Copilot posts its review as an advisory "Comment" review, which
does not count toward required approvals. GitHub also offers an opt-in,
preview "Allow Copilot to approve pull requests" setting
(repository Settings -> Copilot -> Code review -> Approvals) that would let a
Copilot approval count toward merge requirements; it is currently **Off** for
this repository, and no ruleset requires a Copilot review. Keep it Off to
preserve advisory-only behavior, and reconfirm this setting alongside the
other owner-managed repository security settings above after any ownership,
plan, or repository-setting change.

Cost and billing: each automatic review has two separately attributed
components. AI credits for the model interaction are charged to the pull
request author (the repository owner's existing Copilot plan); GitHub Actions
minutes for the agentic capabilities (full-repository context gathering) are
attributed to the repository, not to the author's Copilot plan. Because
CivicAction is a public repository, standard GitHub-hosted Actions minutes are
unmetered, so no Actions-minute cost applies today. No new credential or
billing account is introduced.

Scope limitation: automatic review currently covers only pull requests the
repository owner authors. It does not yet cover pull requests from other
authors or forks; extending coverage would require a repository ruleset
("Automatically request Copilot code review") whose availability on this
repository's plan has not been verified, and is left to a future slice.

To disable: turn off **Automatic Copilot code review** in the same Copilot
settings page. Turning it off does not remove the ability to manually request
a review (click **Request** next to Copilot under Reviewers on any pull
request).
