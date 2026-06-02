# GameStudio Employee and Skill Operating Guide

This guide defines how Codex should behave when working on
`LightNShadow-Survivor`. Use it at the start of every project task.

## Core Rule

For every non-trivial task, select the relevant GameStudio employee role before
acting. If a GameStudio skill applies, select the skill as well. State the
selected employee and skill briefly before continuing.

## Default Employees

Use these employees by default:

- `producer`: scope control, milestones, task sequencing, risk tracking.
- `game-designer`: mechanics, progression, balance, scenario interpretation.
- `systems-designer`: formulas, upgrade tiers, spawn pacing, economy loops.
- `lead-programmer`: architecture, API boundaries, refactors, code review.
- `gameplay-programmer`: Unity C# gameplay implementation.
- `unity-specialist`: Unity API usage, MonoBehaviour patterns, scene/prefab
  safety, performance risks.
- `ui-programmer`: HUD, menus, upgrade cards, result/game-over screens.
- `qa-lead`: test strategy, regression checklist, release confidence.
- `qa-tester`: concrete playtest cases and reproduction steps.

## Employee Selection Rules

- Design analysis: use `game-designer` and `systems-designer`.
- Gameplay code: use `gameplay-programmer`, with `unity-specialist` for Unity
  implementation risks.
- Cross-system architecture: use `lead-programmer`.
- Scene, prefab, component, or Unity Editor behavior: use `unity-specialist`.
- UI changes: use `ui-programmer`, with `ux-designer` if flow or layout changes.
- Testing or verification: use `qa-lead` for strategy, `qa-tester` for concrete
  cases.
- Schedule, priority, or scope conflict: use `producer`.

When a task spans multiple domains, choose one primary employee and one or two
supporting employees. Avoid creating unnecessary parallel work.

## Skill Selection Rules

Use GameStudio skills when their workflow clearly matches the task:

- `team-combat`: flashlight damage, monster reactions, boss mechanics.
- `team-level`: round pacing, spawn phases, encounter flow.
- `team-ui`: HUD, upgrade UI, result UI, menu flow.
- `team-qa`: test plans, smoke checks, regression suites.
- `scope-check`: constrain vague or expanding requests.
- `map-systems`: trace how existing scripts connect before changing them.
- `prototype`: quick proof-of-concept work that should remain disposable.
- `gate-check`: verify readiness before moving to another phase.
- `smoke-check`: quick post-change validation.
- `review-all-gdds`: compare design docs against implementation.
- `reverse-document`: document current implementation from code.

If no skill fits, proceed with the selected employee role only.

## Standard Work Sequence

1. Read the nearest `AGENTS.md`, then this guide.
2. Identify the selected employee role and applicable skill.
3. Read only the relevant docs and scripts. Prefer targeted reads over broad
   project scans.
4. Summarize three key decisions before making broad changes.
5. Keep edits scoped to `Assets/_Project`, `docs`, or `GameStudio` unless the
   user asks otherwise.
6. Do not modify existing unrelated user changes.
7. Verify with the fastest meaningful check available.
8. Report changed files, validation results, risks, and next steps.

## Unity Project Safety

- Do not edit `Library/`, `Temp/`, `obj/`, `Logs/`, `.vs/`, or generated package
  cache files.
- Prefer serialized fields and data-driven values for gameplay tuning.
- Avoid debug-only constants in production logic.
- Avoid per-frame allocations in hot gameplay paths where a simple non-alloc
  Unity API exists.
- Use existing namespaces and folder patterns.
- Keep scene/prefab assumptions explicit when a script depends on assigned
  inspector references.

## LightNShadow-Survivor Priorities

When implementing the reference scenario, prioritize in this order:

1. Player movement and mouse aiming.
2. Flashlight cone, range, line-of-sight, and continuous light damage.
3. Monster health, shield, death, rewards, and type-specific light reactions.
4. Round timers: 60 seconds, 60 seconds, 120 seconds.
5. Scenario-driven spawn pacing and boss phases.
6. Upgrade selection and exact stat tiers.
7. Game over and Sunrise ending.
8. Result UI and stats.

## Reporting Format

Every final response for this project should include:

- Selected employee(s) and skill(s).
- Files changed.
- Verification performed.
- Residual risks or assumptions.
- Two concrete tests when automated tests were not run.

Also preserve the user's ChatDB output requirements when the session is being
recorded through the chat workflow.
