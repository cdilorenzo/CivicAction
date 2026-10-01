# Slices

Slices are tracked as GitHub issues and delivered through pull requests. Use the [slice contract](../022-slice-contract.md) as the source of the workflow requirements and [TEMPLATE.md](TEMPLATE.md) when drafting an issue or planning a slice.

For AI-assisted workflows, see the role contracts in [docs/ai/roles](../ai/roles/). The corresponding [Copilot Skills](../../.github/skills/) make those workflows invokable in Copilot. Thin [Claude Code commands](../../.claude/commands/) provide matching slash commands and point to the same skill and role contracts.

Available workflows: `ddd-slice-planner`, `domain-model-reviewer`, `architecture-decision-recorder`, `slice-bootstrap`, `slice-implementer`, and `slice-publish`. Use `/slice-bootstrap <ISSUE-NUMBER>` to expand an issue specification; review the generated detail against the slice contract before treating it as approved.
