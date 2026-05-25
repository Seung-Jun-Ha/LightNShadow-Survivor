# Design Directory

When authoring or editing files in this directory, follow these standards.

## GDD Files (`design/gdd/`)

Every GDD must include all **8 required sections** in this order:
1. Overview ??one-paragraph summary
2. Player Fantasy ??intended feeling and experience
3. Detailed Rules ??unambiguous mechanics
4. Formulas ??all math defined with variables
5. Edge Cases ??unusual situations handled
6. Dependencies ??other systems listed
7. Tuning Knobs ??configurable values identified
8. Acceptance Criteria ??testable success conditions

**File naming:** `[system-slug].md` (e.g. `movement-system.md`, `combat-system.md`)

**Systems index:** `design/gdd/systems-index.md` ??update when adding a new GDD.

**Design order:** Foundation ??Core ??Feature ??Presentation ??Polish

**Validation:** Run `/design-review [path]` after authoring any GDD.
Run `/review-all-gdds` after completing a set of related GDDs.

## Quick Specs (`design/quick-specs/`)

Lightweight specs for tuning changes, minor mechanics, or balance adjustments.
Use `/quick-design` to author.

## UX Specs (`design/ux/`)

- Per-screen specs: `design/ux/[screen-name].md`
- HUD design: `design/ux/hud.md`
- Interaction pattern library: `design/ux/interaction-patterns.md`
- Accessibility requirements: `design/ux/accessibility-requirements.md`

Use `/ux-design` to author. Validate with `/ux-review` before passing to `/team-ui`.
---

## 한국어 설명

이 파일은 Codex Game Studio Framework에서 '.\design\Codex.md' 경로가 담당하는 원문 지침과 참조 정보를 보존합니다. Codex 환경에서는 AGENTS.md, codex/ 지식 베이스, Unity 우선 엔진 참조를 기준으로 읽으면 됩니다. 핵심 결정은 원문을 삭제하지 않고, Codex 기준 경로와 Unity 제작 흐름을 함께 이해하도록 보조 설명을 추가하는 것입니다.


