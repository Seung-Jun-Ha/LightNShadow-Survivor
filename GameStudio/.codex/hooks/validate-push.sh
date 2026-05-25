#!/bin/bash
# Codex PreToolUse hook: Validates git push commands
# Warns on pushes to protected branches
# Exit 0 = allow, Exit 2 = block
#
# Input schema (PreToolUse for Bash):
# { "tool_name": "Bash", "tool_input": { "command": "git push origin main" } }

INPUT=$(cat)

# Parse command -- use jq if available, fall back to grep
if command -v jq >/dev/null 2>&1; then
    COMMAND=$(echo "$INPUT" | jq -r '.tool_input.command // empty')
else
    COMMAND=$(echo "$INPUT" | grep -oE '"command"[[:space:]]*:[[:space:]]*"[^"]*"' | sed 's/"command"[[:space:]]*:[[:space:]]*"//;s/"$//')
fi

# Only process git push commands
if ! echo "$COMMAND" | grep -qE '^git[[:space:]]+push'; then
    exit 0
fi

CURRENT_BRANCH=$(git rev-parse --abbrev-ref HEAD 2>/dev/null)
MATCHED_BRANCH=""

# Check if pushing to a protected branch
for branch in develop main master; do
    if [ "$CURRENT_BRANCH" = "$branch" ]; then
        MATCHED_BRANCH="$branch"
        break
    fi
    # Also check if pushing to a protected branch explicitly (quote branch name for safety)
    if echo "$COMMAND" | grep -qE "[[:space:]]${branch}([[:space:]]|$)"; then
        MATCHED_BRANCH="$branch"
        break
    fi
done

if [ -n "$MATCHED_BRANCH" ]; then
    echo "Push to protected branch '$MATCHED_BRANCH' detected." >&2
    echo "Reminder: Ensure build passes, unit tests pass, and no S1/S2 bugs exist." >&2
    # Allow the push but warn -- uncomment below to block instead:
    # echo "BLOCKED: Run tests before pushing to $CURRENT_BRANCH" >&2
    # exit 2
fi

exit 0
# 한국어 설명
# 이 파일은 Codex Game Studio Framework에서 '.Codex\hooks\validate-push.sh' 경로가 담당하는 원문 지침과 참조 정보를 보존합니다.
# Codex 환경에서는 AGENTS.md, codex/ 지식 베이스, Unity 우선 엔진 참조를 기준으로 읽으면 됩니다.
# 핵심 결정: 원문은 유지하고, Codex 기준 경로와 Unity 제작 흐름을 함께 설명합니다.


