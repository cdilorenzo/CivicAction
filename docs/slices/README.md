# Slices

Slices are tracked as GitHub issues and delivered through pull requests. Use the [slice contract](../022-slice-contract.md) as the source of the workflow requirements and [TEMPLATE.md](TEMPLATE.md) when drafting an issue or planning a slice.

For AI-assisted workflows, see the role contracts in [docs/ai/roles](../ai/roles/). The corresponding [Copilot Skills](../../.github/skills/) make those workflows invokable in Copilot. Thin [Claude Code commands](../../.claude/commands/) provide matching slash commands and point to the same skill and role contracts.

Available workflows: `ddd-slice-planner`, `domain-model-reviewer`, `architecture-decision-recorder`, `slice-bootstrap`, `slice-implementer`, `slice-publish`, and `slice-open-pr`. Use `/slice-bootstrap` with a new goal or issue number to draft an issue specification; review and explicitly approve any GitHub issue write. Only issues labeled `ready-for-implementation` may be implemented. Use `/slice-publish` to review evidence, then `/slice-open-pr` to prepare and open a PR after explicit approval.

The canonical lifecycle, branch naming, GitHub CLI commands, permissions, and human approval gates are in the [slice contract](../022-slice-contract.md).
