---
name: scope-check
description: "Analyze a feature or sprint for scope creep by comparing current scope against the original plan. Flags additions, quantifies bloat, and recommends cuts. Use when user says 'any scope creep', 'scope review', 'are we staying in scope'."
argument-hint: "[feature-name or sprint-N]"
user-invocable: true
allowed-tools: Read, Glob, Grep, Bash
model: gpt-5.4-mini
---

# Scope Check

This skill is read-only ??it reports findings but writes no files.

Compares original planned scope against current state to detect, quantify, and triage
scope creep.

**Argument:** `$ARGUMENTS[0]` ??feature name, sprint number, or milestone name.

---

## Phase 1: Find the Original Plan

Locate the baseline scope document for the given argument:

- **Feature name** ??read `design/gdd/[feature].md` or matching file in `design/`
- **Sprint number** (e.g., `sprint-3`) ??read `production/sprints/sprint-03.md` or similar
- **Milestone** ??read `production/milestones/[name].md`

If the document is not found, report the missing file and stop. Do not proceed without
a baseline to compare against.

---

## Phase 2: Read the Current State

Check what has actually been implemented or is in progress:

- Scan the codebase for files related to the feature/sprint
- Read git log for commits related to this work (`git log --oneline --since=[start-date]`)
- Check for TODO/FIXME comments that indicate unfinished scope additions
- Check active sprint plan if the feature is mid-sprint

---

## Phase 3: Compare Original vs Current Scope

Produce the comparison report:

```markdown
## Scope Check: [Feature/Sprint Name]
Generated: [Date]

### Original Scope
[List of items from the original plan]

### Current Scope
[List of items currently implemented or in progress]

### Scope Additions (not in original plan)
| Addition | Source | When | Justified? | Effort |
|----------|--------|------|------------|--------|
| [item] | [commit/person] | [date] | [Yes/No/Unclear] | [S/M/L] |

### Scope Removals (in original but dropped)
| Removed Item | Reason | Impact |
|-------------|--------|--------|
| [item] | [why removed] | [what's affected] |

### Bloat Score
- Original items: [N]
- Current items: [N]
- Items added: [N] (+[X]%)
- Items removed: [N]
- Net scope change: [+/-N] ([X]%)

### Risk Assessment
- **Schedule Risk**: [Low/Medium/High] ??[explanation]
- **Quality Risk**: [Low/Medium/High] ??[explanation]
- **Integration Risk**: [Low/Medium/High] ??[explanation]

### Recommendations
1. **Cut**: [Items that should be removed to stay on schedule]
2. **Defer**: [Items that can move to a future sprint/version]
3. **Keep**: [Additions that are genuinely necessary]
4. **Flag**: [Items that need a decision from producer/creative-director]
```

---

## Phase 4: Verdict

Assign a canonical verdict based on net scope change:

| Net Change | Verdict | Meaning |
|-----------|---------|---------|
| ??0% | **PASS** | On Track ??within acceptable variance |
| 10??5% | **CONCERNS** | Minor Creep ??manageable with targeted cuts |
| 25??0% | **FAIL** | Significant Creep ??must cut or formally extend timeline |
| >50% | **FAIL** | Out of Control ??stop, re-plan, escalate to producer |

Output the verdict prominently:

```
**Scope Verdict: [PASS / CONCERNS / FAIL]**
Net change: [+X%] ??[On Track / Minor Creep / Significant Creep / Out of Control]
```

---

## Phase 5: Next Steps

After presenting the report, offer concrete follow-up:

- **PASS** ??no action required. Suggest re-running before next milestone.
- **CONCERNS** ??offer to identify the 2?? additions with best cut ratio. Reference `/sprint-plan update` to formally re-scope.
- **FAIL** ??recommend escalating to producer. Reference `/sprint-plan update` for re-planning or `/estimate` to re-baseline timeline.

Always end with:
> "Run `/scope-check [name]` again after cuts are made to verify the verdict improves."

---

### Rules

- Scope creep is additions without corresponding cuts or timeline extensions
- Not all additions are bad ??some are discovered requirements. But they must be acknowledged and accounted for
- When recommending cuts, prioritize preserving the core player experience over nice-to-haves
- Always quantify scope changes ??"it feels bigger" is not actionable, "+35% items" is
---

## 한국어 설명

이 파일은 Codex Game Studio Framework에서 '.Codex\skills\scope-check\SKILL.md' 경로가 담당하는 원문 지침과 참조 정보를 보존합니다. Codex 환경에서는 AGENTS.md, codex/ 지식 베이스, Unity 우선 엔진 참조를 기준으로 읽으면 됩니다. 핵심 결정은 원문을 삭제하지 않고, Codex 기준 경로와 Unity 제작 흐름을 함께 이해하도록 보조 설명을 추가하는 것입니다.


