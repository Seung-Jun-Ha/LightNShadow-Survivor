# AGENTS.local.md Template

Copy this file to the project root as `AGENTS.local.md` for personal overrides.
This file is gitignored and will not be committed.

```markdown
# Personal Preferences

## Model Preferences
- Prefer GPT-5.5 for complex design tasks
- Use GPT-5.4-Mini for quick lookups and simple edits

## Workflow Preferences
- Always run tests after code changes
- Compact context proactively at 60% usage
- Use /clear between unrelated tasks

## Local Environment
- Python command: python (or py / python3)
- Shell: Git Bash on Windows
- IDE: VS Code with Codex extension

## Communication Style
- Keep responses concise
- Show file paths in all code references
- Explain architectural decisions briefly

## Personal Shortcuts
- When I say "review", run /code-review on the last changed files
- When I say "status", show git status + sprint progress
```

## Setup

1. Copy this template to your project root: `cp codex/docs/Codex-local-template.md AGENTS.local.md`
2. Edit to match your preferences
3. Verify `AGENTS.local.md` is in `.gitignore` (Codex reads it from the project root)
---

## 한국어 설명

이 파일은 Codex Game Studio Framework에서 '.Codex\docs\Codex-local-template.md' 경로가 담당하는 원문 지침과 참조 정보를 보존합니다. Codex 환경에서는 AGENTS.md, codex/ 지식 베이스, Unity 우선 엔진 참조를 기준으로 읽으면 됩니다. 핵심 결정은 원문을 삭제하지 않고, Codex 기준 경로와 Unity 제작 흐름을 함께 이해하도록 보조 설명을 추가하는 것입니다.


