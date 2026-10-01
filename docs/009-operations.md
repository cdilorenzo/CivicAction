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
