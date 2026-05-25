# Engine Reference Documentation

This directory contains curated, version-pinned documentation snapshots for the
game engine(s) used in this project. These files exist because **LLM knowledge
has a cutoff date** and game engines update frequently.

## Why This Exists

Codex's training data has a knowledge cutoff (currently May 2025). Game engines
like Godot, Unity, and Unreal ship updates that introduce breaking API changes,
new features, and deprecated patterns. Without these reference files, agents will
suggest outdated code.

## Structure

Each engine gets its own directory:

```
<engine>/
?쒋?? VERSION.md              # Pinned version, verification date, knowledge gap window
?쒋?? breaking-changes.md     # API changes between versions, organized by risk level
?쒋?? deprecated-apis.md      # "Don't use X ??Use Y" lookup tables
?쒋?? current-best-practices.md  # New practices not in model training data
?붴?? modules/                # Per-subsystem quick references (~150 lines max each)
    ?쒋?? rendering.md
    ?쒋?? physics.md
    ?붴?? ...
```

## How Agents Use These Files

Engine-specialist agents are instructed to:

1. Read `VERSION.md` to confirm the current engine version
2. Check `deprecated-apis.md` before suggesting any engine API
3. Consult `breaking-changes.md` for version-specific concerns
4. Read relevant `modules/*.md` for subsystem-specific work

## Maintenance

### When to Update

- After upgrading the engine version
- When the LLM model is updated (new knowledge cutoff)
- After running `/refresh-docs` (if available)
- When you discover an API the model gets wrong

### How to Update

1. Update `VERSION.md` with the new engine version and date
2. Add new entries to `breaking-changes.md` for the version transition
3. Move newly deprecated APIs into `deprecated-apis.md`
4. Update `current-best-practices.md` with new patterns
5. Update relevant `modules/*.md` with API changes
6. Set "Last verified" dates on all modified files

### Quality Rules

- Every file must have a "Last verified: YYYY-MM-DD" date
- Keep module files under 150 lines (context budget)
- Include code examples showing correct/incorrect patterns
- Link to official documentation URLs for verification
- Only document things that differ from the model's training data
---

## 한국어 설명

이 파일은 Codex Game Studio Framework에서 '.\docs\engine-reference\README.md' 경로가 담당하는 원문 지침과 참조 정보를 보존합니다. Codex 환경에서는 AGENTS.md, codex/ 지식 베이스, Unity 우선 엔진 참조를 기준으로 읽으면 됩니다. 핵심 결정은 원문을 삭제하지 않고, Codex 기준 경로와 Unity 제작 흐름을 함께 이해하도록 보조 설명을 추가하는 것입니다.


