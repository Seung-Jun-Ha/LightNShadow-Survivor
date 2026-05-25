# Directory Structure

```text
/
?쒋?? AGENTS.md                    # Master configuration
?쒋?? codex/                     # Agent definitions, skills, hooks, rules, docs
?쒋?? src/                         # Game source code (core, gameplay, ai, networking, ui, tools)
?쒋?? assets/                      # Game assets (art, audio, vfx, shaders, data)
?쒋?? design/                      # Game design documents (gdd, narrative, levels, balance)
?쒋?? docs/                        # Technical documentation (architecture, api, postmortems)
??  ?붴?? engine-reference/        # Curated engine API snapshots (version-pinned)
?쒋?? tests/                       # Test suites (unit, integration, performance, playtest)
?쒋?? tools/                       # Build and pipeline tools (ci, build, asset-pipeline)
?쒋?? prototypes/                  # Throwaway prototypes (isolated from src/)
?붴?? production/                  # Production management (sprints, milestones, releases)
    ?쒋?? session-state/           # Ephemeral session state (active.md ??gitignored)
    ?붴?? session-logs/            # Session audit trail (gitignored)
```
---

## 한국어 설명

이 파일은 Codex Game Studio Framework에서 '.Codex\docs\directory-structure.md' 경로가 담당하는 원문 지침과 참조 정보를 보존합니다. Codex 환경에서는 AGENTS.md, codex/ 지식 베이스, Unity 우선 엔진 참조를 기준으로 읽으면 됩니다. 핵심 결정은 원문을 삭제하지 않고, Codex 기준 경로와 Unity 제작 흐름을 함께 이해하도록 보조 설명을 추가하는 것입니다.


