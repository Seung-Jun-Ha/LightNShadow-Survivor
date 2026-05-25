#!/usr/bin/env bash
# post-compact.sh ??fires after conversation compaction
# Reminds Codex to restore session state from the file-backed checkpoint.

ACTIVE="production/session-state/active.md"

echo "=== Context Restored After Compaction ==="

if [ -f "$ACTIVE" ]; then
  SIZE=$(wc -l < "$ACTIVE" 2>/dev/null || echo "?")
  echo "Session state file exists: $ACTIVE ($SIZE lines)"
  echo "IMPORTANT: Read this file now to restore your working context."
  echo "It contains: current task, decisions made, files in progress, open questions."
else
  echo "No session state file found at $ACTIVE"
  echo "If you were mid-task, check production/session-logs/ for the last session audit."
fi

echo "========================================="
# 한국어 설명
# 이 파일은 Codex Game Studio Framework에서 '.Codex\hooks\post-compact.sh' 경로가 담당하는 원문 지침과 참조 정보를 보존합니다.
# Codex 환경에서는 AGENTS.md, codex/ 지식 베이스, Unity 우선 엔진 참조를 기준으로 읽으면 됩니다.
# 핵심 결정: 원문은 유지하고, Codex 기준 경로와 Unity 제작 흐름을 함께 설명합니다.


