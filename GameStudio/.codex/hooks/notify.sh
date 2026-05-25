#!/usr/bin/env bash
# Notification hook ??fires when Codex sends a notification
# Shows a Windows toast via PowerShell

# Read notification JSON from stdin
INPUT=$(cat)

# Extract message ??try jq first, fall back to grep
if command -v jq &>/dev/null; then
  MESSAGE=$(echo "$INPUT" | jq -r '.message // empty' 2>/dev/null)
fi
if [ -z "$MESSAGE" ]; then
  MESSAGE=$(echo "$INPUT" | grep -oE '"message":"[^"]*"' | sed 's/"message":"//;s/"//')
fi
if [ -z "$MESSAGE" ]; then
  MESSAGE="Codex needs your attention"
fi

# Sanitize message for PowerShell string embedding (escape single quotes)
MESSAGE_SAFE=$(echo "$MESSAGE" | sed "s/'/''/g" | head -c 200)

# Show Windows balloon tip notification (works on all Windows 10/11 without extra modules)
powershell.exe -NonInteractive -WindowStyle Hidden -Command "
  Add-Type -AssemblyName System.Windows.Forms
  \$notify = New-Object System.Windows.Forms.NotifyIcon
  \$notify.Icon = [System.Drawing.SystemIcons]::Information
  \$notify.BalloonTipTitle = 'Codex'
  \$notify.BalloonTipText = '$MESSAGE_SAFE'
  \$notify.Visible = \$true
  \$notify.ShowBalloonTip(5000)
  Start-Sleep -Seconds 6
  \$notify.Dispose()
" 2>/dev/null &

echo "Notification: $MESSAGE_SAFE"
# 한국어 설명
# 이 파일은 Codex Game Studio Framework에서 '.Codex\hooks\notify.sh' 경로가 담당하는 원문 지침과 참조 정보를 보존합니다.
# Codex 환경에서는 AGENTS.md, codex/ 지식 베이스, Unity 우선 엔진 참조를 기준으로 읽으면 됩니다.
# 핵심 결정: 원문은 유지하고, Codex 기준 경로와 Unity 제작 흐름을 함께 설명합니다.


