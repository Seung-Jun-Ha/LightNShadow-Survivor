# 구현 현황 및 전체 구현율

작성일: 2026-06-03
대상 프로젝트: Light & Shadow Survivor
평가 기준: 현재 `Assets/_Project/Scripts`, PlayMode 테스트, 프로젝트 문서, 최근 검토 결과 기준

## 요약

현재 프로젝트는 핵심 생존 액션 루프가 코드 레벨에서 상당 부분 연결된 상태입니다. 손전등 공격, 빛 피해 수신, 몬스터 체력/사망, 라운드 진행, 스폰, 경험치, 업그레이드, 게임오버, 엔딩 흐름의 기본 골격은 구현되어 있습니다. 2026-06-03 최신 검증에서 `dotnet build` 오류 0개와 PlayMode 테스트 27/27 통과를 확인했습니다. 다만 프리팹/씬 연결 검증, 보스/특수 몬스터 완성도, UI/연출, 밸런스, 세이브/설정, 자동화 테스트 범위는 아직 보강이 필요합니다.

- 핵심 로직 구현률: 약 78%
- 플레이 가능한 MVP 완성도: 약 70%
- 릴리즈 후보 완성도: 약 60%
- 자동화 테스트 커버리지: 약 48%

전체 구현율은 릴리즈 기준으로 약 66%로 봅니다. 기능 골격은 유지된 채 주요 상태 전환, 사망, 보상, 업그레이드 재개 리스크가 해소되었지만, 실제 플레이 품질과 안정성을 보장하려면 씬/프리팹 연결 확인, 보스/특수 몬스터 검증, UI/연출 완성도 확인이 더 필요합니다. 세부 실행 항목과 테스트 메모는 [docs/implementation-backlog.md](docs/implementation-backlog.md)로 분리했습니다.

## 목표 대비 진행률 업데이트

프로젝트 목표는 "어둠 속에서 빛으로 유령 계열 몬스터를 막아내고, 3라운드를 버틴 뒤 Sunrise 엔딩에 도달하는 생존 액션"입니다. 현재 기준으로 목표 대비 진행률은 다음과 같습니다.

- 목표 1: 이동, 조준, 손전등 공격, 생존 기본 루프 구현 - 약 82% 달성
  - 플레이어 이동, 충돌체 보정, 마우스 조준, 손전등 피해 판정, 사망 후 게임오버 흐름이 구현되어 있고 PlayMode 테스트로 핵심 판정이 검증되었습니다.
  - 남은 작업은 실제 씬 프리팹 연결 확인, 조작감 튜닝, 카메라/입력 예외 처리입니다.
- 목표 2: 라운드가 진행될수록 몬스터 속도와 위험 요소가 자연스럽게 증가 - 약 72% 달성
  - 라운드 타이머, 라운드별 스폰 조절, 몬스터 사망/보상, 3라운드 후 엔딩 흐름은 연결되어 있습니다.
  - 중간 레벨업 업그레이드 후 라운드 타이머가 리셋되는 문제와 상태 전환 시간 배율 리스크는 수정되었습니다.
  - 남은 작업은 실제 웨이브 밸런스, 보스 페이즈, 특수 몬스터별 압박감 검증입니다.
- 목표 3: 업그레이드가 체감되는 성장 구조 - 약 68% 달성
  - 이동 속도, 내구도, 빛 강도, 빛 범위, 오라 위습 업그레이드 구조가 있고 null 선택/복귀 흐름을 보강했습니다.
  - 남은 작업은 업그레이드 카드 풀의 밸런스, 선택 UI 프리팹 연결 검증, 성장 체감 플레이테스트입니다.
- 목표 4: Sunrise 엔딩에서 어둠이 걷히는 변화를 명확히 전달 - 약 60% 달성
  - `EndingManager`, `TimeOfDayManager`, 결과 UI 흐름의 코드 골격과 엔딩 진입 시 플레이어 액션 비활성화 테스트가 있습니다.
  - 남은 작업은 연출 품질, 사운드/VFX, 실제 씬 조명 전환 검증입니다.

## 영역별 구현률

| 영역               | 구현률 | 상태      | 근거                                                                                                                                                         |
| ------------------ | -----: | --------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| 플레이어 이동/체력 |    82% | 부분 완료 | `PlayerController`, `PlayerHealth`, `PlayerExperience` 구현. 충돌체 보정, 사망 연출 후 GameOver, 레벨업 null-safe 회귀 테스트 통과.                          |
| 손전등 전투        |    85% | 높음      | 거리, 각도, 장애물 차단, 지속 피해 구현. PlayMode 테스트 3개 통과.                                                                                           |
| 빛 피해 반응       |    75% | 부분 완료 | `LightDamageReceiver`가 몬스터 타입별 피해/슬로우 배율 적용. Split 등 일부 반응은 미완.                                                                      |
| 몬스터 기본 로직   |    76% | 부분 완료 | `MonsterBase`, `GhostAI`, 특수 몬스터 스크립트 존재. 사망 처리, XP 오브 드롭, XP 중복 지급, Teleport 이동 방식 리스크 수정. 보스 실드 복구는 추가 검증 필요. |
| 몬스터 스폰/라운드 |    78% | 부분 완료 | `MonsterSpawner`, `RoundManager` 구현. 중간 레벨업 후 라운드 타이머 재초기화 리스크와 상태 전환 시간 배율 수정 및 회귀 테스트 통과.                          |
| 보스/특수 몬스터   |    55% | 보강 필요 | Boss/Shield/Teleport/Blob 구조는 있으나 패턴 완성도와 테스트 부족.                                                                                           |
| 업그레이드/성장    |    68% | 부분 완료 | XP, 레벨업, 업그레이드 매니저/데이터 구조 존재. null 선택 복귀와 중간 업그레이드 복귀 테스트 추가. 세이브/히스토리/밸런스 검증 부족.                         |
| 맵/충돌/상호작용   |    65% | 부분 완료 | Ground/Obstacle/Player 물리 검증 통과. 유령 몬스터는 Player와 물리 충돌하지 않는 설계.                                                                       |
| UI/게임 흐름       |    62% | 보강 필요 | HUD/업그레이드/게임오버/결과 UI 스크립트 존재. 상태별 timeScale/커서 정책 보강. 실제 씬 연결과 연출 검증 필요.                                               |
| 엔딩/시간대 연출   |    60% | 보강 필요 | `EndingManager`, `TimeOfDayManager` 존재. 엔딩 진입 흐름 테스트 통과. 연출 품질과 씬 상태 전환 테스트 부족.                                                  |
| 오디오/VFX         |    35% | 초기      | 매니저와 일부 효과는 있으나 전투/피격/환경 SFX 연결 부족.                                                                                                    |
| 자동화 테스트      |    48% | 시작됨    | 손전등, 맵/플레이어/몬스터 충돌, 라운드/엔딩, 핵심 회귀 PlayMode 테스트 27개 통과. 전체 씬/프리팹 테스트는 부족.                                             |

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

### 2026-06-03 회귀 테스트

결과: 전체 PlayMode 테스트 27개 중 27개 통과

- `MidRoundUpgradeResumeDoesNotResetRoundTimer`: 통과
- `PlayerDeathEventFiresOnlyOnce`: 통과
- `MonsterWithDeathHandlerDoesNotGrantDirectXP`: 통과
- `PlayerDeathHandlerTriggersGameOverAfterDeathRoutine`: 통과
- `MonsterDeathHandlerCountsKillAndDropsXpOrb`: 통과
- `PlayerExperienceLevelUpDoesNotRequireGameManager`: 통과
- `StartRoundFromTerminalStateResetsStatsAndResumesTime`: 통과

추가 검증:

- `dotnet build LightNShadow-Survivor.sln`: 성공, 오류 0개
- Unity PlayMode TestRunner: 성공, 실패 0개

최신 실행:

- 2026-06-03 23:27 KST 기준 `dotnet build LightNShadow-Survivor.sln`: 성공, 오류 0개
- 2026-06-03 23:27 KST 기준 Unity PlayMode TestRunner: 27개 중 27개 통과
- 결과 XML: `C:/Users/tjdgns/AppData/LocalLow/DefaultCompany/LightNShadowSurviver/TestResults.xml`

### 2026-06-03 구현도 개선 패스

결과: 핵심 게임 루프와 상태 관리 안정화 완료. 핵심 로직 구현률은 약 78%, 플레이 가능한 MVP 완성도는 약 70%, 릴리즈 기준 전체 구현율은 약 66%로 산정합니다.

- `GameManager` 상태 진입 정책 보강: `MainMenu`, `Round`, `Upgrade`, `Ending`, `GameOver` 진입 시 `Time.timeScale`과 커서 상태를 일관 적용.
- 새 라운드 시작 시 이전 게임의 처치 수/생존 시간 통계가 남지 않도록 `GameStatsManager.ResetStats()` 연결.
- `GameOver` 진입 시 사망 연출 이후 시간이 정지되도록 정리.
- 주요 싱글톤(`GameManager`, `RoundManager`, `UIManager`, `GameStatsManager`, `GameCycleManager`, `PlayerController`, `PlayerExperience`) 파괴 시 `Instance` 해제.
- `GameCycleManager`의 업그레이드 UI null 방어 및 null 업그레이드 선택 흐름 보강.
- 몬스터 사망 흐름 구현 보강: 처치 카운트, 충돌 비활성화, XP 오브 드롭, 아이템 드롭, 낙하/디졸브 후 제거.
- 플레이어 사망 흐름 구현 보강: `PlayerDeathHandler`가 있을 때 사망 연출 완료 후 `GameOver` 1회 전환.

## 남은 핵심 리스크

1. 보스 실드 취약 상태 이후 실드 복구 흐름이 불명확함.
2. 실제 씬/프리팹 연결 상태는 코드 테스트만으로 충분히 보장되지 않음.
3. `MonsterDeathHandler`가 붙은 몬스터는 XP 오브 프리팹 연결이 비어 있으면 XP를 지급하지 않을 수 있음.
4. UI, 오디오, VFX, 밸런스는 릴리즈 품질 기준에서 추가 작업 필요.
5. 특수 몬스터 Split/Shield/Boss 패턴 완성도와 자동화 테스트가 부족함.

## 다음 우선순위

1. GameScene 실제 프리팹 연결 점검
2. 보스/특수 몬스터별 PlayMode 테스트 추가
3. XP 오브 프리팹 연결 상태와 보상 정책 검증
4. UI 흐름 테스트: 시작, 라운드, 업그레이드, 게임오버, 엔딩
5. 오디오/VFX 연결과 최소 밸런스 패스
6. 실제 플레이 기준 조작감, 카메라, 몬스터 스폰 밀도 튜닝

## 세부 작업 분리

세부 todoList와 테스트 실행 메모는 [docs/implementation-backlog.md](docs/implementation-backlog.md)로 분리했습니다. 이 문서는 상태 판단, 지표, 우선순위만 유지합니다.

## 판정

현재 상태는 “핵심 로직 MVP 구현 후 안정화 단계”입니다. 2026-06-03 기준으로 주요 상태 전환/보상 중복/사망 흐름 리스크 일부는 해결되었고 자동화 테스트도 27개까지 늘었습니다. 다만 릴리즈 품질로 보기에는 씬 연결, 보스/특수 몬스터, UI/연출, 밸런스, 실제 플레이 검증이 여전히 부족합니다.
