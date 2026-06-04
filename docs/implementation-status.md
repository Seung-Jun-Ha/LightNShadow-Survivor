# 구현 현황 및 전체 구현율

작성일: 2026-06-04
대상 프로젝트: Light & Shadow Survivor
평가 기준: 현재 `Assets/_Project/Scripts`, PlayMode 테스트, 프로젝트 문서, 최근 검토 결과 기준

## 요약

현재 프로젝트는 핵심 생존 액션 루프가 코드 레벨에서 상당 부분 연결된 상태입니다. 손전등 공격, 빛 피해 수신, 몬스터 체력/사망, 라운드 진행, 스폰, 경험치, 업그레이드, 게임오버, 엔딩 흐름의 기본 골격은 구현되어 있습니다. 2026-06-04 최신 검증에서 `dotnet build` 오류 0개, PlayMode 테스트 28/28 통과, GameScene 핵심 매니저 참조 및 대표 몬스터 프리팹 구성이 정상임을 확인했습니다. XP 보상 누락 방지, Unity 6 테스트 API 경고 정리, 플레이어 수직 조준 제한 연결까지 완료했습니다. 다만 실제 씬 전체 플레이 루프 수동 검증, 보스/특수 몬스터 완성도, UI/연출, 밸런스, 세이브/설정은 아직 보강이 필요합니다.

- 핵심 로직 구현률: 약 80%
- 플레이 가능한 MVP 완성도: 약 74%
- 릴리즈 후보 완성도: 약 66%
- 자동화 테스트 커버리지: 약 54%

전체 구현율은 릴리즈 기준으로 약 70%로 봅니다. 핵심 상태 전환, 사망, 보상, 씬/프리팹 참조, XP 보상 누락, 프로젝트 소유 코드 경고 일부를 해소했습니다. 실제 플레이 품질과 안정성을 보장하려면 수동 전체 루프 검증, 보스/특수 몬스터 검증, UI/연출 완성도 확인이 더 필요합니다. 세부 실행 항목과 테스트 메모는 [docs/implementation-backlog.md](docs/implementation-backlog.md)로 분리했습니다.

## 목표 대비 진행률 업데이트

프로젝트 목표는 "어둠 속에서 빛으로 유령 계열 몬스터를 막아내고, 3라운드를 버틴 뒤 Sunrise 엔딩에 도달하는 생존 액션"입니다. 현재 기준으로 목표 대비 진행률은 다음과 같습니다.

- 목표 1: 이동, 조준, 손전등 공격, 생존 기본 루프 구현 - 약 85% 달성
  - 플레이어 이동, 충돌체 보정, 마우스 조준, 손전등 피해 판정, 사망 후 게임오버 흐름이 구현되어 있고 PlayMode 테스트로 핵심 판정이 검증되었습니다.
  - GameScene 핵심 매니저와 UI 참조가 연결되어 있음을 Editor 검증기로 확인했습니다.
  - 남은 작업은 실제 플레이 조작감 튜닝, 카메라/입력 예외 처리, 수동 전체 루프 검증입니다.
- 목표 2: 라운드가 진행될수록 몬스터 속도와 위험 요소가 자연스럽게 증가 - 약 75% 달성
  - 라운드 타이머, 라운드별 스폰 조절, 몬스터 사망/보상, 3라운드 후 엔딩 흐름은 연결되어 있습니다.
  - 중간 레벨업 업그레이드 후 라운드 타이머가 리셋되는 문제와 상태 전환 시간 배율 리스크는 수정되었습니다.
  - 대표 몬스터 프리팹의 스폰/사망 필수 컴포넌트와 XP 오브 참조를 검증했습니다.
  - 남은 작업은 실제 웨이브 밸런스, 보스 페이즈, 특수 몬스터별 압박감 검증입니다.
- 목표 3: 업그레이드가 체감되는 성장 구조 - 약 71% 달성
  - 이동 속도, 내구도, 빛 강도, 빛 범위, 오라 위습 업그레이드 구조가 있고 null 선택/복귀 흐름을 보강했습니다.
  - XP 오브 누락 시 직접 XP 지급 fallback을 추가하고 정상 오브 생성 시 중복 지급되지 않음을 검증했습니다.
  - 남은 작업은 업그레이드 카드 풀의 밸런스, 성장 체감 플레이테스트입니다.
- 목표 4: Sunrise 엔딩에서 어둠이 걷히는 변화를 명확히 전달 - 약 60% 달성
  - `EndingManager`, `TimeOfDayManager`, 결과 UI 흐름의 코드 골격과 엔딩 진입 시 플레이어 액션 비활성화 테스트가 있습니다.
  - 남은 작업은 연출 품질, 사운드/VFX, 실제 씬 조명 전환 검증입니다.

## 영역별 구현률

| 영역               | 구현률 | 상태      | 근거                                                                                                                                                         |
| ------------------ | -----: | --------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| 플레이어 이동/체력 |    84% | 부분 완료 | `PlayerController`, `PlayerHealth`, `PlayerExperience` 구현. 충돌체 보정, 사망 연출 후 GameOver, 레벨업 null-safe, 수직 조준 제한 회귀 테스트 통과.                          |
| 손전등 전투        |    85% | 높음      | 거리, 각도, 장애물 차단, 지속 피해 구현. PlayMode 테스트 3개 통과.                                                                                           |
| 빛 피해 반응       |    75% | 부분 완료 | `LightDamageReceiver`가 몬스터 타입별 피해/슬로우 배율 적용. Split 등 일부 반응은 미완.                                                                      |
| 몬스터 기본 로직   |    80% | 부분 완료 | `MonsterBase`, `GhostAI`, 특수 몬스터 스크립트 존재. 사망 처리, XP 오브 드롭, XP fallback/중복 방지 검증 완료. 대표 프리팹 필수 구성 통과. 보스 실드 복구는 추가 검증 필요. |
| 몬스터 스폰/라운드 |    80% | 부분 완료 | `MonsterSpawner`, `RoundManager` 구현. 라운드 타이머/상태 전환 회귀 테스트와 GameScene 스포너 참조 검증 통과. 실제 웨이브 밸런스 검증 필요.                          |
| 보스/특수 몬스터   |    55% | 보강 필요 | Boss/Shield/Teleport/Blob 구조는 있으나 패턴 완성도와 테스트 부족.                                                                                           |
| 업그레이드/성장    |    71% | 부분 완료 | XP, 레벨업, 업그레이드 매니저/데이터 구조 존재. null 선택/중간 업그레이드 복귀와 XP 보상 fallback 테스트 통과. 세이브/히스토리/밸런스 검증 부족.                         |
| 맵/충돌/상호작용   |    65% | 부분 완료 | Ground/Obstacle/Player 물리 검증 통과. 유령 몬스터는 Player와 물리 충돌하지 않는 설계.                                                                       |
| UI/게임 흐름       |    65% | 보강 필요 | HUD/업그레이드/게임오버/결과 UI 스크립트 존재. 상태별 timeScale/커서 정책 보강 및 GameScene 핵심 UI 참조 검증 통과. 수동 흐름/해상도/연출 검증 필요.                                               |
| 엔딩/시간대 연출   |    60% | 보강 필요 | `EndingManager`, `TimeOfDayManager` 존재. 엔딩 진입 흐름 테스트 통과. 연출 품질과 씬 상태 전환 테스트 부족.                                                  |
| 오디오/VFX         |    35% | 초기      | 매니저와 일부 효과는 있으나 전투/피격/환경 SFX 연결 부족.                                                                                                    |
| 자동화 테스트      |    54% | 진행 중   | 손전등, 충돌, 조준, 라운드/엔딩, 핵심 회귀 PlayMode 테스트 28개 통과. GameScene 핵심 참조 및 대표 몬스터 프리팹 검증기 추가. 특수 몬스터 패턴 테스트는 부족.                                             |

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

### 2026-06-04 회귀 및 참조 검증

결과: 전체 PlayMode 테스트 28개 중 28개 통과

- `MidRoundUpgradeResumeDoesNotResetRoundTimer`: 통과
- `PlayerDeathEventFiresOnlyOnce`: 통과
- `MonsterDeathHandlerGrantsDirectXpWhenOrbPrefabIsMissing`: 통과
- `PlayerDeathHandlerTriggersGameOverAfterDeathRoutine`: 통과
- `MonsterDeathHandlerCountsKillAndDropsXpOrb`: 통과
- `PlayerExperienceLevelUpDoesNotRequireGameManager`: 통과
- `StartRoundFromTerminalStateResetsStatsAndResumesTime`: 통과
- `VerticalAimTargetIsClampedRelativeToPlayer`: 통과

추가 검증:

- `dotnet build LightNShadow-Survivor.sln`: 성공, 오류 0개
- Unity PlayMode TestRunner: 성공, 실패 0개
- `GameSceneValidator.ValidateBatch`: 핵심 매니저 6종 및 필수 직렬화 참조 통과
- `GameSceneValidator.ValidateEnemyPrefabsBatch`: 대표 몬스터 프리팹 3종 필수 컴포넌트 및 XP 오브 참조 통과

최신 실행:

- 2026-06-04 KST 기준 `dotnet build LightNShadow-Survivor.sln`: 성공, 오류 0개
- 2026-06-04 KST 기준 Unity PlayMode TestRunner: 28개 중 28개 통과
- 결과 XML: `C:/Users/tjdgns/AppData/LocalLow/DefaultCompany/LightNShadowSurviver/TestResults.xml`

### 2026-06-04 구현도 개선 패스

결과: P0 자동 검증 항목 3개와 P2 릴리즈 경고 정리 항목 2개 완료. 핵심 로직 구현률은 약 80%, 플레이 가능한 MVP 완성도는 약 74%, 릴리즈 기준 전체 구현율은 약 70%로 산정합니다.

- GameScene의 `GameManager`, `RoundManager`, `MonsterSpawner`, `UpgradeManager`, `UIManager`, `EndingManager` 존재와 필수 직렬화 참조 검증 완료.
- `Boss_Ghost`, `Ghost_Shielded`, `Ghost_Teleport` 프리팹의 필수 몬스터 컴포넌트와 XP 오브 참조 검증 완료.
- XP 오브 프리팹 누락 또는 잘못된 프리팹 지정 시 직접 XP를 지급하는 fallback 추가.
- 정상 XP 오브 생성 시 직접 XP가 중복 지급되지 않는 회귀 테스트 유지.
- P0 자동 검증 항목 4개 중 3개 완료. 실제 씬 수동 smoke test만 남음.
- 프로젝트 소유 테스트 코드의 Unity 6 obsolete API 경고 제거.
- `PlayerAim.verticalLimit`을 실제 수직 조준 제한에 연결하고 회귀 테스트 추가.

## 남은 핵심 리스크

1. 보스 실드 취약 상태 이후 실드 복구 흐름이 불명확함.
2. 실제 씬에서 시작부터 엔딩/게임오버까지 이어지는 전체 플레이 루프의 수동 검증이 필요함.
3. UI, 오디오, VFX, 밸런스는 릴리즈 품질 기준에서 추가 작업 필요.
4. 특수 몬스터 Split/Shield/Boss 패턴 완성도와 자동화 테스트가 부족함.
5. 옵션/설정 저장과 빌드 타깃별 입력/해상도 검증이 없음.

## 다음 우선순위

1. 실제 씬 수동 smoke test: 시작, 라운드, 업그레이드, 다음 라운드, 게임오버, 엔딩
2. 보스/특수 몬스터별 PlayMode 테스트 추가
3. UI 해상도 대응 및 전체 흐름 검증
4. 오디오/VFX 연결과 최소 밸런스 패스
5. 실제 플레이 기준 조작감, 카메라, 몬스터 스폰 밀도 튜닝
6. 옵션/설정 저장 범위와 빌드 타깃별 예외 정의

## 세부 작업 분리

세부 todoList와 테스트 실행 메모는 [docs/implementation-backlog.md](docs/implementation-backlog.md)로 분리했습니다. 이 문서는 상태 판단, 지표, 우선순위만 유지합니다.

## 판정

현재 상태는 “핵심 로직 MVP 구현 후 안정화 단계”입니다. 2026-06-04 기준으로 주요 상태 전환, 사망, XP 보상, GameScene/대표 몬스터 프리팹 참조 리스크를 자동 검증했고 프로젝트 소유 코드의 릴리즈 경고 두 항목을 해결했습니다. P0 자동 검증 항목은 4개 중 3개가 완료되었고 전체 구현율은 약 70%입니다. 다만 릴리즈 품질로 보기에는 수동 전체 루프, 보스/특수 몬스터, UI/연출, 밸런스, 설정/플랫폼 검증이 여전히 부족합니다.
