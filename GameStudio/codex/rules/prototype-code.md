---
paths:
  - "prototypes/**"
---

# Prototype Code Standards (Relaxed)

Prototypes are throwaway code for validating ideas. Standards are intentionally
relaxed to maximize iteration speed. The goal is learning, not production quality.

## What's Allowed in Prototypes
- Hardcoded values (no need for data-driven config)
- Minimal or no doc comments
- Simple architecture (no dependency injection required)
- Singletons and global state
- Copy-pasted code (no need for abstraction)
- Debug output left in place
- Placeholder art and audio
- Quick-and-dirty solutions

## What's Still Required
- Each prototype lives in its own subdirectory: `prototypes/[name]/`
- Every prototype MUST have a `README.md` with:
  - What hypothesis is being tested
  - How to run the prototype
  - Current status (in-progress / concluded)
  - Findings (updated when prototype concludes)
- No production code may reference or import from `prototypes/`
- Prototypes must not modify files outside `prototypes/`
- Prototypes must not be deployed or shipped

## When a Prototype Succeeds
If a prototype validates a concept and the feature moves to production:
1. The prototype code is NOT migrated directly ??it is rewritten to production standards
2. The prototype `README.md` findings inform the production design document
3. The prototype directory is preserved for reference but never extended

## Cleanup
Concluded prototypes should be archived or deleted after findings are captured.
Never let prototype code grow into production code through incremental "cleanup."
---

## 한국어 설명

이 파일은 Codex Game Studio Framework에서 '.\codex\rules\prototype-code.md' 경로가 담당하는 원문 지침과 참조 정보를 보존합니다. Codex 환경에서는 AGENTS.md, codex/ 지식 베이스, Unity 우선 엔진 참조를 기준으로 읽으면 됩니다. 핵심 결정은 원문을 삭제하지 않고, Codex 기준 경로와 Unity 제작 흐름을 함께 이해하도록 보조 설명을 추가하는 것입니다.


