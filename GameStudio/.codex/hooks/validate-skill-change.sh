#!/bin/bash
# Codex PostToolUse hook: Advises running skill-test after skill file changes
# Fires when any file inside codex/skills/ is written or edited.
#
# Exit behavior:
#   exit 0 = advisory only (non-blocking)
#
# Input schema (PostToolUse for Write|Edit):
# { "tool_name": "Write", "tool_input": { "file_path": "...", "content": "..." } }

INPUT=$(cat)

# Parse file path -- use jq if available, fall back to grep
if command -v jq >/dev/null 2>&1; then
    FILE_PATH=$(echo "$INPUT" | jq -r '.tool_input.file_path // empty')
else
    FILE_PATH=$(echo "$INPUT" | grep -oE '"file_path"[[:space:]]*:[[:space:]]*"[^"]*"' | sed 's/"file_path"[[:space:]]*:[[:space:]]*"//;s/"$//')
fi

# Normalize path separators (Windows backslash to forward slash)
FILE_PATH=$(echo "$FILE_PATH" | sed 's|\\|/|g')

# Only act on files inside codex/skills/
if ! echo "$FILE_PATH" | grep -qE '(^|/)\codex/skills/'; then
    exit 0
fi

# Extract skill name from path (codex/skills/[skill-name]/SKILL.md)
SKILL_NAME=$(echo "$FILE_PATH" | grep -oE '\codex/skills/[^/]+' | sed 's|\codex/skills/||')

if [ -z "$SKILL_NAME" ]; then
    exit 0
fi

echo "=== Skill Modified: $SKILL_NAME ===" >&2
echo "Run /skill-test static $SKILL_NAME to validate structural compliance." >&2
echo "====================================" >&2

exit 0
# 한국어 설명
# 이 파일은 Codex Game Studio Framework에서 '.Codex\hooks\validate-skill-change.sh' 경로가 담당하는 원문 지침과 참조 정보를 보존합니다.
# Codex 환경에서는 AGENTS.md, codex/ 지식 베이스, Unity 우선 엔진 참조를 기준으로 읽으면 됩니다.
# 핵심 결정: 원문은 유지하고, Codex 기준 경로와 Unity 제작 흐름을 함께 설명합니다.


