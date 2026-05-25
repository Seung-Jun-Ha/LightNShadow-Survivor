# CCGS Skill Testing Framework

Quality assurance infrastructure for the **Codex Game Studio Framework** framework.
Tests the skills and agents themselves ??not any game built with them.

> **This folder is self-contained and optional.**
> Game developers using CCGS don't need it. To remove it entirely:
> `rm -rf "CCGS Skill Testing Framework"` ??nothing in `codex/` depends on it.

---

## What's in here

```
CCGS Skill Testing Framework/
?쒋?? README.md              ??you are here
?쒋?? AGENTS.md              ??tells Codex how to use this framework
?쒋?? catalog.yaml           ??master registry: all 73 skills + 49 agents, coverage tracking
?쒋?? quality-rubric.md      ??category-specific pass/fail metrics for /skill-test category
??
?쒋?? skills/                ??behavioral spec files for skills (one per skill)
??  ?쒋?? gate/              ??gate category specs
??  ?쒋?? review/            ??review category specs
??  ?쒋?? authoring/         ??authoring category specs
??  ?쒋?? readiness/         ??readiness category specs
??  ?쒋?? pipeline/          ??pipeline category specs
??  ?쒋?? analysis/          ??analysis category specs
??  ?쒋?? team/              ??team category specs
??  ?쒋?? sprint/            ??sprint category specs
??  ?붴?? utility/           ??utility category specs
??
?쒋?? agents/                ??behavioral spec files for agents (one per agent)
??  ?쒋?? directors/         ??creative-director, technical-director, producer, art-director
??  ?쒋?? leads/             ??lead-programmer, narrative-director, audio-director, etc.
??  ?쒋?? specialists/       ??engine/code/shader/UI specialists
??  ?쒋?? godot/             ??Godot-specific specialists
??  ?쒋?? unity/             ??Unity-specific specialists
??  ?쒋?? unreal/            ??Unreal-specific specialists
??  ?쒋?? operations/        ??QA, live-ops, release, localization, etc.
??  ?붴?? creative/          ??writer, world-builder, game-designer, etc.
??
?쒋?? templates/             ??spec file templates for writing new specs
??  ?쒋?? skill-test-spec.md ??template for skill behavioral specs
??  ?붴?? agent-test-spec.md ??template for agent behavioral specs
??
?붴?? results/               ??test run outputs (written by /skill-test spec, gitignored)
```

---

## How to use it

All testing is driven by two skills already in the framework:

### Check structural compliance

```
/skill-test static [skill-name]     # Check one skill (7 checks)
/skill-test static all              # Check all 73 skills
```

### Run a behavioral spec test

```
/skill-test spec gate-check         # Evaluate a skill against its written spec
/skill-test spec design-review
```

### Check against category rubric

```
/skill-test category gate-check     # Evaluate one skill against its category metrics
/skill-test category all            # Run rubric checks across all categorized skills
```

### See full coverage picture

```
/skill-test audit                   # Skills + agents: has-spec, last tested, result
```

### Improve a failing skill

```
/skill-improve gate-check           # Test ??diagnose ??propose fix ??retest loop
```

---

## Skill categories

| Category | Skills | Key metrics |
|----------|--------|-------------|
| `gate` | gate-check | Review mode read, full/lean/solo director panel, no auto-advance |
| `review` | design-review, architecture-review, review-all-gdds | Read-only, 8-section check, correct verdicts |
| `authoring` | design-system, quick-design, art-bible, create-architecture, ??| Section-by-section May-I-write, skeleton-first |
| `readiness` | story-readiness, story-done | Blockers surfaced, director gate in full mode |
| `pipeline` | create-epics, create-stories, dev-story, map-systems, ??| Upstream dependency check, handoff path clear |
| `analysis` | consistency-check, balance-check, code-review, tech-debt, ??| Read-only report, verdict keyword, no writes |
| `team` | team-combat, team-narrative, team-audio, ??| All required agents spawned, blocked surfaced |
| `sprint` | sprint-plan, sprint-status, milestone-review, ??| Reads sprint data, status keywords present |
| `utility` | start, adopt, hotfix, localize, setup-engine, ??| Passes static checks |

---

## Agent tiers

| Tier | Agents |
|------|--------|
| `directors` | creative-director, technical-director, producer, art-director |
| `leads` | lead-programmer, narrative-director, audio-director, ux-designer, qa-lead, release-manager, localization-lead |
| `specialists` | gameplay-programmer, engine-programmer, ui-programmer, tools-programmer, network-programmer, ai-programmer, level-designer, sound-designer, technical-artist |
| `godot` | godot-specialist, godot-gdscript-specialist, godot-csharp-specialist, godot-shader-specialist, godot-gdextension-specialist |
| `unity` | unity-specialist, unity-ui-specialist, unity-shader-specialist, unity-dots-specialist, unity-addressables-specialist |
| `unreal` | unreal-specialist, ue-gas-specialist, ue-replication-specialist, ue-umg-specialist, ue-blueprint-specialist |
| `operations` | devops-engineer, security-engineer, performance-analyst, analytics-engineer, community-manager |
| `creative` | writer, world-builder, game-designer, economy-designer, systems-designer, prototyper |

---

## Updating the catalog

`catalog.yaml` tracks test coverage for every skill and agent. After running a test:

- `/skill-test spec [name]` will offer to update `last_spec` and `last_spec_result`
- `/skill-test category [name]` will offer to update `last_category` and `last_category_result`
- `last_static` and `last_static_result` are updated manually or via `/skill-improve`

---

## Writing a new spec

1. Find the spec template at `templates/skill-test-spec.md`
2. Copy it to `skills/[category]/[skill-name].md`
3. Update the `spec:` field in `catalog.yaml` to point to the new file
4. Run `/skill-test spec [skill-name]` to validate it

---

## Removing this framework

This folder has no hooks into the main project. To remove:

```bash
rm -rf "CCGS Skill Testing Framework"
```

The skills `/skill-test` and `/skill-improve` will still function ??they'll simply
report that `catalog.yaml` is missing and suggest running `/skill-test audit` to
initialize it.
---

## 한국어 설명

이 파일은 Codex Game Studio Framework에서 '.\CCGS Skill Testing Framework\README.md' 경로가 담당하는 원문 지침과 참조 정보를 보존합니다. Codex 환경에서는 AGENTS.md, codex/ 지식 베이스, Unity 우선 엔진 참조를 기준으로 읽으면 됩니다. 핵심 결정은 원문을 삭제하지 않고, Codex 기준 경로와 Unity 제작 흐름을 함께 이해하도록 보조 설명을 추가하는 것입니다.


