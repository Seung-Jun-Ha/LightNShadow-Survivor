# 저장소 구조

이 문서는 GitHub에서 프로젝트 구조를 빠르게 이해할 수 있도록 Unity 폴더와 핵심 코드 배치를 정리합니다.

## 루트 구성

- `Assets/`: 게임 코드, 씬, 프리팹, 데이터, 아트 리소스
- `Packages/`: Unity 패키지 의존성
- `ProjectSettings/`: Unity 프로젝트 설정
- `GameStudio/`: 개발 보조 문서와 Codex Game Studio 자료
- `docs/`: 프로젝트 설명 문서
- `README.md`: 저장소 소개와 실행 안내

## 핵심 구현 경로

게임 본문은 `Assets/_Project`를 중심으로 구성됩니다.

```text
Assets/_Project
├─ Scripts
│  ├─ Core
│  ├─ Managers
│  ├─ Player
│  ├─ Enemies
│  ├─ Items
│  ├─ UI
│  └─ Upgrades
├─ Scenes
├─ Prefabs
├─ Data
└─ Tests
```

## Scripts

### `Scripts/Core`

- `CameraFollow.cs`: 플레이어 추적 카메라
- `StageManager.cs`: 스테이지 구성 관리
- `TimeOfDayManager.cs`: 밤과 새벽 사이의 조명/시간대 전환

### `Scripts/Managers`

- `GameManager.cs`: 전체 게임 상태와 주요 흐름 제어
- `GameCycleManager.cs`: 게임 사이클 상태 관리
- `RoundManager.cs`: 라운드 타이머와 라운드 전환
- `MonsterSpawner.cs`: 몬스터 생성은 `Enemies`에 위치하며 라운드 흐름과 연동
- `UpgradeManager.cs`: 업그레이드 후보 생성과 선택 처리
- `UIManager.cs`: HUD와 화면 전환 관리
- `AudioManager.cs`: BGM과 SFX 재생
- `EndingManager.cs`: 엔딩 진입과 결과 흐름
- `GameStatsManager.cs`: 플레이 통계 집계

### `Scripts/Player`

- `PlayerController.cs`: 이동 입력과 이동 처리
- `PlayerAim.cs`: 마우스 기반 조준
- `PlayerHealth.cs`: 체력과 피해 처리
- `PlayerExperience.cs`: 경험치와 레벨업
- `PlayerDeathHandler.cs`: 사망 이벤트와 게임 오버 연결
- `AuraController.cs`, `WillOTheWisp.cs`: 보조 공격 시스템

### `Scripts/Enemies`

- `MonsterBase.cs`: 몬스터 공통 체력과 상태
- `GhostAI.cs`: 추적과 공격 거리 기반 행동
- `MonsterSpawner.cs`: 라운드별 몬스터 생성
- `MonsterDeathHandler.cs`: 사망 처리, 보상, 효과 연결
- `ShieldGhost.cs`: 보호막형 적
- `TeleportGhost.cs`: 순간이동형 적
- `BossBase.cs`: 보스 공통 패턴 기반
- `BlobShadow.cs`: 그림자 계열 보조 오브젝트

### `Scripts/Items`

- `ExperienceOrb.cs`: 경험치 오브 수집
- `HealthPack.cs`: 체력 회복 아이템
- `SpeedBoost.cs`: 일시적 이동 속도 강화
- `LightBoost.cs`: 빛 공격 강화
- `ItemBase.cs`: 아이템 공통 처리

### `Scripts/UI`

- `MainMenuController.cs`: 시작 화면
- `UpgradeUIController.cs`: 업그레이드 패널
- `UpgradeCardUI.cs`: 업그레이드 카드 표시
- `GameOverUIController.cs`: 게임 오버 화면
- `ResultUIController.cs`: 결과 화면
- `ButtonSoundTrigger.cs`: 버튼 SFX 트리거

### `Scripts/Upgrades`

- `UpgradeData.cs`: 업그레이드 ScriptableObject 데이터 구조

## Scenes

- `Assets/_Project/Scenes/GameScene.unity`: 메인 플레이 씬
- `Assets/_Project/Scenes/UIScene.unity`: UI 구성 씬
- `Assets/_Project/Scenes/SkillScene.unity`: 기능 확인용 씬
- `Assets/_Project/Scenes/gameOver_Scene.unity`: 게임 오버 화면 관련 씬

## Prefabs와 Data

- `Prefabs/Enemies`: 보스와 특수 몬스터 프리팹
- `Prefabs/Items`: 경험치 오브와 아이템 프리팹
- `Prefabs/UI`: 업그레이드 카드 등 UI 프리팹
- `Prefabs/VFX`: 빛 타격 효과
- `Data/Upgrades`: 이동, 체력, 빛 공격, 빛 범위, 보조 공격 업그레이드 데이터

## Tests

- `Tests/PlayMode`: 플레이 흐름과 주요 게임 로직 검증
- `Tests/Editor`: 배치 실행과 씬/프리팹 검증 유틸리티
