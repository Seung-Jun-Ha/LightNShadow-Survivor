# 프로젝트 구조

이 문서는 `Light & Shadow Survivor`의 실제 Unity 폴더 구조를 유지보수 관점에서 정리한 기준 문서입니다. 새 기능을 넣을 때는 먼저 어디에 배치할지 이 문서를 기준으로 결정합니다.

## 루트 구성

- `Assembly-CSharp.csproj`, `Assembly-CSharp-Editor.csproj`: Unity 자동 생성 프로젝트 파일
- `LightNShadow-Survivor.sln`, `LightNShadowSurviver.slnx`: 솔루션 진입점
- `Assets/`: 게임 본문 자산과 코드
- `Packages/`: Unity 패키지 의존성
- `ProjectSettings/`: 프로젝트 전역 설정
- `GameStudio/`: Codex Game Studio Framework 보조 문서와 워크플로

## 외부 참고 자료

- 워크스페이스의 `reference/Light_Shadow_Survivor_Scenario_Integrated.md`는 시나리오 원문 보관용 참고 자료입니다. 저장소 본문 구조와는 분리해서 취급합니다.

## Assets/\_Project 기준 구조

실제 게임 구현은 `Assets/_Project/`를 중심으로 정리합니다.

### `Assets/_Project/Scripts/Core`

- `CameraFollow.cs`: 카메라 추적
- `StageManager.cs`: 스테이지/환경 진입 처리
- `TimeOfDayManager.cs`: 밤/새벽 조명 전환

### `Assets/_Project/Scripts/Managers`

- `GameManager.cs`: 전체 게임 상태 관리
- `GameCycleManager.cs`: 라운드와 상태 흐름 제어
- `RoundManager.cs`: 라운드 타이머와 전환
- `AudioManager.cs`: 배경음과 연출 사운드
- `UIManager.cs`: 화면 전환과 UI 흐름
- `UpgradeManager.cs`: 업그레이드 선택과 적용
- `EndingManager.cs`: Sunrise 엔딩 연출
- `GameStatsManager.cs`: 플레이 데이터 집계

### `Assets/_Project/Scripts/Player`

- `PlayerController.cs`: 이동
- `PlayerAim.cs`: 마우스 조준
- `PlayerHealth.cs`: 체력
- `PlayerExperience.cs`: 경험치
- `PlayerDeathHandler.cs`: 사망 처리
- `AuraController.cs`, `WillOTheWisp.cs`: 특수 오오라/보조 효과

### `Assets/_Project/Scripts/Enemies`

- `MonsterBase.cs`: 몬스터 공통 상태
- `GhostAI.cs`: 기본 추적 AI
- `MonsterSpawner.cs`: 몬스터 생성
- `MonsterDeathHandler.cs`: 사망 연출
- `BossBase.cs`: 보스 공통 로직
- `ShieldGhost.cs`, `TeleportGhost.cs`: 특수 몬스터 변형
- `BlobShadow.cs`: 그림자 계열 보조 오브젝트

### `Assets/_Project/Scripts/UI`

- `MainMenuController.cs`: 시작 화면
- `UpgradeUIController.cs`: 업그레이드 화면
- `UpgradeCardUI.cs`: 카드 1개 표시
- `GameOverUIController.cs`: 게임 오버 화면
- `ResultUIController.cs`: 결과 화면

### `Assets/_Project/Scripts/Upgrades`

- `UpgradeData.cs`: 업그레이드 데이터 정의

### 루트 스크립트

- `FlashLightAttack.cs`: 손전등 공격 판정
- `LightDamageReceiver.cs`: 빛 피해 수신 공통 로직

## 데이터와 리소스

- `Assets/_Project/Data/Upgrades/`: 업그레이드 ScriptableObject 자산
- `Assets/_Project/Scenes/GameScene/`: 게임 플레이 씬
- `Assets/_Project/Scenes/UIScene/`: UI 씬
- `Assets/_Project/Prefabs/Enemies/`: 몬스터 프리팹
- `Assets/_Project/Prefabs/Items/`: 아이템 및 보상 프리팹
- `Assets/_Project/Prefabs/UI/`: UI 프리팹
- `Assets/_Project/Prefabs/VFX/`: 연출 효과 프리팹
- `Assets/_Project/Settings/`: 렌더 파이프라인, 볼륨, 물리 재질
- `Assets/_Project/Animations/`, `Materials/`, `Models/`, `Shaders/`, `Textures/`: 아트 리소스

## 유지보수 규칙

- 게임 상태는 `Manager` 계층에서만 바꿉니다.
- 플레이어, 몬스터, UI는 서로 직접 제어하지 않고, 필요한 이벤트와 데이터만 주고받습니다.
- 밸런스 수치는 스크립트에 하드코딩하지 말고 `UpgradeData`나 `Data/Upgrades`로 분리합니다.
- 새 유령 타입을 추가할 때는 `MonsterBase` 또는 `BossBase`를 상속하고, 프리팹과 데이터는 각각 `Enemies/`와 `Data/Upgrades/` 정책에 맞춰 분리합니다.
- 씬은 흐름만 담당하고, 세부 규칙은 `Scripts/` 쪽에 둡니다.
