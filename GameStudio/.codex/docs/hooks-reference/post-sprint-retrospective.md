# Hook: post-sprint-retrospective

## Trigger

Manual trigger at the end of each sprint (typically invoked by the producer
agent or the human developer).

## Purpose

Automatically generates a retrospective starting point by analyzing the sprint
data: what was planned vs completed, velocity changes, bug trends, and common
blockers. This is not a git hook but a workflow hook invoked through the
`producer` agent.

## Implementation

This is a workflow hook, not a git hook. It is invoked by running:

```
@producer Generate sprint retrospective for Sprint [N]
```

The producer agent should:

1. **Read the sprint plan** from `production/sprints/sprint-[N].md`
2. **Calculate metrics**:
   - Tasks planned vs completed
   - Story points planned vs completed (if used)
   - Carryover items from previous sprint
   - New tasks added mid-sprint
   - Average task completion time
3. **Analyze patterns**:
   - Most common blockers
   - Which agent/area had the most incomplete work
   - Which estimates were most inaccurate
4. **Generate the retrospective**:

```markdown
# Sprint [N] Retrospective

## Metrics
| Metric | Value |
|--------|-------|
| Tasks Planned | [N] |
| Tasks Completed | [N] |
| Completion Rate | [X%] |
| Carryover from Previous | [N] |
| New Tasks Added | [N] |
| Bugs Found | [N] |
| Bugs Fixed | [N] |

## Velocity Trend
[Sprint N-2]: [X] | [Sprint N-1]: [Y] | [Sprint N]: [Z]
Trend: [Improving / Stable / Declining]

## What Went Well
- [Automatically detected: tasks completed ahead of estimate]
- [Facilitator adds team observations]

## What Went Poorly
- [Automatically detected: tasks that were carried over or cut]
- [Automatically detected: areas with significant estimate overruns]
- [Facilitator adds team observations]

## Blockers
| Blocker | Frequency | Resolution Time | Prevention |
|---------|-----------|----------------|-----------|

## Action Items for Next Sprint
| # | Action | Owner | Priority |
|---|--------|-------|----------|

## Estimation Accuracy
| Area | Avg Planned | Avg Actual | Accuracy |
|------|------------|-----------|----------|
```

5. **Save** to `production/sprints/sprint-[N]-retro.md`
---

## 한국어 설명

이 파일은 Codex Game Studio Framework에서 '.Codex\docs\hooks-reference\post-sprint-retrospective.md' 경로가 담당하는 원문 지침과 참조 정보를 보존합니다. Codex 환경에서는 AGENTS.md, codex/ 지식 베이스, Unity 우선 엔진 참조를 기준으로 읽으면 됩니다. 핵심 결정은 원문을 삭제하지 않고, Codex 기준 경로와 Unity 제작 흐름을 함께 이해하도록 보조 설명을 추가하는 것입니다.


