# 시스템 구성

이 문서는 작업 진행 상황이 아니라, 저장소에 포함된 주요 시스템과 코드 역할을 설명합니다.

## 게임 루프

`Light & Shadow Survivor`의 기본 루프는 다음과 같습니다.

```text
메인 메뉴
  -> 라운드 시작
  -> 몬스터 스폰과 전투
  -> 경험치 수집
  -> 업그레이드 선택
  -> 다음 라운드 또는 종료 흐름
```

이 흐름은 `GameManager`, `RoundManager`, `MonsterSpawner`, `UpgradeManager`, `UIManager`, `EndingManager`가 함께 구성합니다.

## 전투 시스템

전투는 손전등 기반 지속 피해 구조입니다. `FlashLightAttack`이 공격 범위를 계산하고, `LightDamageReceiver`가 피해를 받는 대상의 공통 인터페이스 역할을 합니다.

주요 판정 요소:

- 손전등과 대상 사이의 거리
- 손전등 방향과 대상 위치 사이의 각도
- 장애물 레이어에 의한 시야 차단
- 초당 피해량 기반의 지속 피해

## 플레이어 시스템

플레이어 관련 스크립트는 기능별로 분리되어 있습니다.

- 이동: `PlayerController`
- 조준: `PlayerAim`
- 체력: `PlayerHealth`
- 경험치: `PlayerExperience`
- 사망 처리: `PlayerDeathHandler`
- 보조 공격: `AuraController`, `WillOTheWisp`

## 몬스터 시스템

몬스터는 공통 상태와 개별 행동을 나누어 관리합니다.

- 공통 상태: `MonsterBase`
- 기본 AI: `GhostAI`
- 생성: `MonsterSpawner`
- 사망 처리: `MonsterDeathHandler`
- 특수 타입: `ShieldGhost`, `TeleportGhost`, `BossBase`, `BlobShadow`

몬스터 프리팹은 `Assets/_Project/Prefabs/Enemies`에 배치됩니다.

## 성장 시스템

경험치 오브는 `ExperienceOrb`가 담당하며, 플레이어 경험치는 `PlayerExperience`가 관리합니다. 업그레이드 데이터는 `UpgradeData`로 정의되고 `Assets/_Project/Data/Upgrades`에 저장됩니다.

업그레이드 UI는 `UpgradeUIController`와 `UpgradeCardUI`가 담당합니다.

## 아이템 시스템

아이템은 `ItemBase`를 중심으로 구성됩니다.

- `HealthPack`: 체력 회복
- `SpeedBoost`: 이동 속도 강화
- `LightBoost`: 손전등 강화
- `ExperienceOrb`: 경험치 수집

아이템 프리팹은 `Assets/_Project/Prefabs/Items`에 있습니다.

## UI 시스템

UI는 메뉴, HUD, 업그레이드, 게임 오버, 결과 화면으로 나뉩니다.

- `MainMenuController`
- `UpgradeUIController`
- `UpgradeCardUI`
- `GameOverUIController`
- `ResultUIController`
- `ButtonSoundTrigger`

화면 전환은 `UIManager`와 게임 상태 매니저가 연결합니다.

## 연출 시스템

시간대와 엔딩 연출은 `TimeOfDayManager`와 `EndingManager`가 담당합니다. 오디오는 `AudioManager`, 타격 효과는 `Prefabs/VFX` 아래의 프리팹과 전투 스크립트가 연결합니다.

## 에셋 사용

프로젝트 전용 게임 자산은 `Assets/_Project` 아래에 모여 있습니다. 외부 에셋은 별도 폴더에 보관해 원본 출처와 프로젝트 적용 범위를 구분합니다.

- 환경/자연물: `Assets/ThirdParty/Pure Poly`, `Assets/ThirdParty/Darth_Artisan`, `Assets/ThirdParty/Low Poly Stones`
- 적 캐릭터: `Assets/ThirdParty/GhostCharacter_Free`, `Assets/ThirdParty/Monster_Ghosts_FREE`, `Assets/Monster_Orc (Troll)`
- 손전등: `Assets/ThirdParty/Free-FlashLight`
- UI: `Assets/ThirdParty/Gentleland/SteampunkUI`, `Assets/_Project/Prefabs/UI`
- 음악: `Assets/OccaSoftware/Fantasy Music Pack`
- 프로젝트 전용 데이터와 프리팹: `Assets/_Project/Data`, `Assets/_Project/Prefabs`

## 테스트 시스템

테스트는 `Assets/_Project/Tests`에 있습니다.

- `FlashLightAttackPlayModeTests`: 손전등 공격 판정
- `CharacterMovementPlayModeTests`: 이동 관련 검증
- `PlayerAimPlayModeTests`: 조준 관련 검증
- `MapPlayerMonsterCollisionPlayModeTests`: 맵/플레이어/몬스터 충돌 규칙
- `RoundToEndingFlowPlayModeTests`: 라운드와 엔딩 흐름
- `GameplayRegressionPlayModeTests`: 주요 회귀 검증
- `ReleaseBalancePlayModeTests`: 게임 밸런스 관련 검증
- `GameSceneValidator`: 씬과 프리팹 참조 검증
- `BatchPlayModeTestRunner`: 배치 환경용 테스트 실행기
