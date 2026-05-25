# Agent Coordination Rules

1. **Vertical Delegation**: Leadership agents delegate to department leads, who
   delegate to specialists. Never skip a tier for complex decisions.
2. **Horizontal Consultation**: Agents at the same tier may consult each other
   but must not make binding decisions outside their domain.
3. **Conflict Resolution**: When two agents disagree, escalate to the shared
   parent. If no shared parent, escalate to `creative-director` for design
   conflicts or `technical-director` for technical conflicts.
4. **Change Propagation**: When a design change affects multiple domains, the
   `producer` agent coordinates the propagation.
5. **No Unilateral Cross-Domain Changes**: An agent must never modify files
   outside its designated directories without explicit delegation.

## Model Tier Assignment

Skills and agents are assigned to model tiers based on task complexity:

| Tier | Model | When to use |
|------|-------|-------------|
| **GPT-5.4-Mini** | `Codex-GPT-5.4-Mini-4-5-20251001` | Read-only status checks, formatting, simple lookups ??no creative judgment needed |
| **GPT-5.4** | `Codex-GPT-5.4-4-6` | Implementation, design authoring, analysis of individual systems ??default for most work |
| **GPT-5.5** | `Codex-GPT-5.5-4-6` | Multi-document synthesis, high-stakes phase gate verdicts, cross-system holistic review |

Skills with `model: gpt-5.4-mini`: `/help`, `/sprint-status`, `/story-readiness`, `/scope-check`,
`/project-stage-detect`, `/changelog`, `/patch-notes`, `/onboard`

Skills with `model: gpt-5.5`: `/review-all-gdds`, `/architecture-review`, `/gate-check`

All other skills default to GPT-5.4. When creating new skills, assign GPT-5.4-Mini if the
skill only reads and formats; assign GPT-5.5 if it must synthesize 5+ documents with
high-stakes output; otherwise leave unset (GPT-5.4).

## Subagents vs Agent Teams

This project uses two distinct multi-agent patterns:

### Subagents (current, always active)
Spawned via `Task` within a single Codex session. Used by all `team-*` skills
and orchestration skills. Subagents share the session's permission context, run
sequentially or in parallel within the session, and return results to the parent.

**When to spawn in parallel**: If two subagents' inputs are independent (neither
needs the other's output to begin), spawn both Task calls simultaneously rather
than waiting. Example: `/review-all-gdds` Phase 1 (consistency) and Phase 2
(design theory) are independent ??spawn both at the same time.

### Agent Teams (experimental ??opt-in)
Multiple independent Codex *sessions* running simultaneously, coordinated
via a shared task list. Each session has its own context window and token budget.
Requires `Codex_CODE_EXPERIMENTAL_AGENT_TEAMS=1` environment variable.

**Use agent teams when**:
- Work spans multiple subsystems that will not touch the same files
- Each workstream would take >30 minutes and benefits from true parallelism
- A senior agent (technical-director, producer) needs to coordinate 3+ specialist
  sessions working on different epics simultaneously

**Do not use agent teams when**:
- One session's output is required as input for another (use sequential subagents)
- The task fits in a single session's context (use subagents instead)
- Cost is a concern ??each team member burns tokens independently

**Current status**: Opt-in via `Codex_CODE_EXPERIMENTAL_AGENT_TEAMS=1`. Document first usage here when adopted.

## Parallel Task Protocol

When an orchestration skill spawns multiple independent agents:

1. Issue all independent Task calls before waiting for any result
2. Collect all results before proceeding to dependent phases
3. If any agent is BLOCKED, surface it immediately ??do not silently skip
4. Always produce a partial report if some agents complete and others block
---

## 한국어 설명

이 파일은 Codex Game Studio Framework에서 '.\codex\docs\coordination-rules.md' 경로가 담당하는 원문 지침과 참조 정보를 보존합니다. Codex 환경에서는 AGENTS.md, codex/ 지식 베이스, Unity 우선 엔진 참조를 기준으로 읽으면 됩니다. 핵심 결정은 원문을 삭제하지 않고, Codex 기준 경로와 Unity 제작 흐름을 함께 이해하도록 보조 설명을 추가하는 것입니다.


