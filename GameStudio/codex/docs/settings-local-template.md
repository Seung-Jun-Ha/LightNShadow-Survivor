# settings.local.json Template

Create `codex/settings.local.json` for personal overrides that should NOT
be committed to version control. Add it to `.gitignore`.

## Example settings.local.json

```json
{
  "permissions": {
    "allow": [
      "Bash(git *)",
      "Bash(npm *)",
      "Read",
      "Glob",
      "Grep"
    ],
    "deny": [
      "Bash(rm -rf *)",
      "Bash(git push --force *)"
    ]
  }
}
```

## Permission Modes

Codex supports different permission modes. Recommended for game dev:

### During Development (Default)
Use **normal mode** ??Codex asks before running most commands. This is safest
for production code.

### During Prototyping
Use **auto-accept mode** with limited scope ??faster iteration on throwaway code.
Only use this when working in `prototypes/` directory.

### During Code Review
Use **read-only** permissions ??Codex can read and search but not modify files.

## Customizing Hooks Locally

You can add personal hooks in `settings.local.json` that extend (not override)
the project hooks. For example, adding a notification when builds complete:

```json
{
  "hooks": {
    "Stop": [
      {
        "matcher": "",
        "hooks": [
          {
            "type": "command",
            "command": "bash -c 'echo Session ended at $(date)'",
            "timeout": 5
          }
        ]
      }
    ]
  }
}
```
---

## 한국어 설명

이 파일은 Codex Game Studio Framework에서 '.\codex\docs\settings-local-template.md' 경로가 담당하는 원문 지침과 참조 정보를 보존합니다. Codex 환경에서는 AGENTS.md, codex/ 지식 베이스, Unity 우선 엔진 참조를 기준으로 읽으면 됩니다. 핵심 결정은 원문을 삭제하지 않고, Codex 기준 경로와 Unity 제작 흐름을 함께 이해하도록 보조 설명을 추가하는 것입니다.


