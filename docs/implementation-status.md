# 구현 현황 및 전체 구현율

작성일: 2026-06-02
대상 프로젝트: Light & Shadow Survivor
평가 기준: 현재 `Assets/_Project/Scripts`, PlayMode 테스트, 프로젝트 문서, 최근 검토 결과 기준

## 요약

현재 프로젝트는 핵심 생존 액션 루프가 코드 레벨에서 상당 부분 연결된 상태입니다. 손전등 공격, 빛 피해 수신, 몬스터 체력/사망, 라운드 진행, 스폰, 경험치, 업그레이드, 엔딩 흐름의 기본 골격은 구현되어 있습니다. 다만 프리팹/씬 연결 검증, 보스/특수 몬스터 완성도, UI/연출, 밸런스, 세이브/설정, 자동화 테스트 범위는 아직 보강이 필요합니다.

- 핵심 로직 구현률: 약 72%
- 플레이 가능한 MVP 완성도: 약 65%
- 릴리즈 후보 완성도: 약 55%
- 자동화 테스트 커버리지: 약 35%

전체 구현율은 릴리즈 기준으로 약 60%로 봅니다. 기능 골격은 있으나, 실제 플레이 품질과 안정성을 보장하려면 테스트와 씬/프리팹 연결 확인이 더 필요합니다.

## 영역별 구현률

| 영역 | 구현률 | 상태 | 근거 |
| --- | ---: | --- | --- |
| 플레이어 이동/체력 | 70% | 부분 완료 | `PlayerController`, `PlayerHealth`, `PlayerExperience` 구현. 사망 중복 처리 guard는 보강 필요. |
| 손전등 전투 | 85% | 높음 | 거리, 각도, 장애물 차단, 지속 피해 구현. PlayMode 테스트 3개 통과. |
| 빛 피해 반응 | 75% | 부분 완료 | `LightDamageReceiver`가 몬스터 타입별 피해/슬로우 배율 적용. Split 등 일부 반응은 미완. |
| 몬스터 기본 로직 | 65% | 부분 완료 | `MonsterBase`, `GhostAI`, 특수 몬스터 스크립트 존재. XP 중복 지급, 보스 실드 복구, Teleport 이동 방식 등 검토 이슈 있음. |
| 몬스터 스폰/라운드 | 70% | 부분 완료 | `MonsterSpawner`, `RoundManager` 구현. 중간 레벨업과 라운드 타이머 재초기화 리스크 있음. |
| 보스/특수 몬스터 | 55% | 보강 필요 | Boss/Shield/Teleport/Blob 구조는 있으나 패턴 완성도와 테스트 부족. |
| 업그레이드/성장 | 60% | 부분 완료 | XP, 레벨업, 업그레이드 매니저/데이터 구조 존재. 세이브/히스토리/밸런스 검증 부족. |
| 맵/충돌/상호작용 | 65% | 부분 완료 | Ground/Obstacle/Player 물리 검증 통과. 유령 몬스터는 Player와 물리 충돌하지 않는 설계. |
| UI/게임 흐름 | 55% | 보강 필요 | HUD/업그레이드/게임오버/결과 UI 스크립트 존재. 실제 씬 연결과 연출 검증 필요. |
| 엔딩/시간대 연출 | 55% | 보강 필요 | `EndingManager`, `TimeOfDayManager` 존재. 연출 품질과 씬 상태 전환 테스트 부족. |
| 오디오/VFX | 35% | 초기 | 매니저와 일부 효과는 있으나 전투/피격/환경 SFX 연결 부족. |
| 자동화 테스트 | 35% | 시작됨 | 손전등, 맵/플레이어/몬스터 충돌 PlayMode 테스트 추가. 전체 게임 루프 테스트는 부족. |

## 최근 검증 결과

### 손전등 PlayMode 테스트

결과: 3개 중 3개 통과

- `DamagesEnemyInsideConeAndRange`: 통과
- `DoesNotDamageEnemyOutsideRange`: 통과
- `ObstacleLayerBlocksLightDamage`: 통과

결과 파일: `Temp/flashlight-playmode-test-results.xml`, `Temp/implementation-status-playmode-test-results.xml`

### 맵/플레이어/몬스터 상호작용 테스트

현재 설계 반영 기준:

- 플레이어는 Ground를 통과하지 않아야 함.
- 플레이어는 Obstacle에 막혀야 함.
- 유령 몬스터는 Player와 물리 충돌하지 않아야 함.
- 유령 몬스터의 공격은 물리 충돌이 아니라 `GhostAI`의 거리 기반 공격으로 검증함.

최신 실행 결과는 손전등/충돌 PlayMode 테스트 8개 중 8개 통과입니다. Player-Enemy 물리 충돌은 유령 몬스터 설계에 따라 무시되는 것이 정상입니다.

## 주요 완료 항목

- 손전등 공격 판정: 거리, 각도, 장애물 차단, 지속 피해
- 빛 피해 수신: 몬스터 타입별 피해/슬로우 반응
- 기본 몬스터 AI: 플레이어 탐색, 근접 거리 공격, 충돌체 보정
- 몬스터 체력/사망/경험치 구조
- 라운드 진행과 스폰 간격 조절
- 3라운드 이후 엔딩 흐름 골격
- PlayMode 테스트 기반 추가
- GameStudio 작업 지침 문서 추가

## 남은 핵심 리스크

1. 중간 레벨업에서 업그레이드 진입 시 현재 라운드 타이머가 재초기화될 수 있음.
2. `PlayerHealth` 사망 처리가 중복 호출될 수 있음.
3. `MonsterBase` 즉시 XP 지급과 `MonsterDeathHandler` XP 오브 드랍이 중복 보상을 만들 수 있음.
4. 보스 실드 취약 상태 이후 실드 복구 흐름이 불명확함.
5. `TeleportGhost`가 NavMeshAgent `Warp` 대신 transform 위치 변경을 사용해 경로 상태가 어긋날 수 있음.
6. 실제 씬/프리팹 연결 상태는 코드 테스트만으로 충분히 보장되지 않음.
7. UI, 오디오, VFX, 밸런스는 릴리즈 품질 기준에서 추가 작업 필요.

## 다음 우선순위

1. 라운드/업그레이드 상태 전환 버그 수정 및 테스트 추가
2. XP 중복 지급 정책 확정 후 `MonsterBase`/`MonsterDeathHandler` 정리
3. PlayerHealth 사망 guard 추가
4. 보스/특수 몬스터별 PlayMode 테스트 추가
5. GameScene 실제 프리팹 연결 점검
6. UI 흐름 테스트: 시작, 라운드, 업그레이드, 게임오버, 엔딩
7. 오디오/VFX 연결과 최소 밸런스 패스

## 테스트 실행 메모

Unity 표준 `-runTests`가 현재 환경에서 초기 컴파일 타이밍 때문에 테스트 실행까지 이어지지 않는 경우가 있어, `BatchPlayModeTestRunner`를 통해 TestRunner API를 직접 호출하는 방식으로 검증했습니다.

대표 실행 명령:

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Unity.exe' -batchmode -projectPath 'C:\Users\tjdgns\UnityProj\LNS_seungJun\LightNShadow-Survivor' -executeMethod LightNShadowSurvivor.Tests.BatchPlayModeTestRunner.RunFlashlightTests -testResults 'C:\Users\tjdgns\UnityProj\LNS_seungJun\LightNShadow-Survivor\Temp\collision-playmode-test-results.xml' -logFile 'C:\Users\tjdgns\UnityProj\LNS_seungJun\LightNShadow-Survivor\Temp\collision-playmode-test.log'
```

## 판정

현재 상태는 “핵심 로직 MVP 구현 후 안정화 단계”입니다. 기능의 큰 줄기는 구현되어 있으나, 릴리즈 품질로 보기에는 테스트, 씬 연결, 보스/특수 몬스터, UI/연출, 밸런스가 부족합니다.

