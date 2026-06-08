# 저장소 구조

이 문서는 GitHub에서 프로젝트 구조를 빠르게 이해할 수 있도록 Unity 폴더와 핵심 코드 배치를 정리합니다.

## 루트 구성

- `Assets/`: 게임 코드, 씬, 프리팹, 데이터, 아트 리소스
- `Packages/`: Unity 패키지 의존성
- `ProjectSettings/`: Unity 프로젝트 설정
- `GameStudio/`: 개발 보조 문서와 Codex Game Studio 자료
- `docs/`: 프로젝트 설명 문서
- `README.md`: 저장소 소개와 실행 안내

## Unity 버전

- Unity 6 `6000.4.7f1`
- 버전 기준 파일: `ProjectSettings/ProjectVersion.txt`
- 패키지 기준 파일: `Packages/manifest.json`

주요 사용 패키지는 URP, Input System, AI Navigation, Unity UI, Unity Test Framework입니다.

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

## Assets

### 프로젝트 전용 에셋

- `Assets/_Project/Scenes`: 게임 플레이와 UI 씬
- `Assets/_Project/Prefabs`: 게임에서 직접 사용하는 적, 아이템, UI, VFX 프리팹
- `Assets/_Project/Data/Upgrades`: 업그레이드 ScriptableObject
- `Assets/_Project/Materials`: 플레이어, 적, 지형, 이펙트용 머티리얼
- `Assets/_Project/Textures`: 프로젝트 전용 텍스처
- `Assets/_Project/Settings`: 물리 재질과 렌더링 관련 설정

### 외부 에셋

- `Assets/ThirdParty/GhostCharacter_Free`: 유령 캐릭터 모델, 머티리얼, 프리팹
- `Assets/ThirdParty/Monster_Ghosts_FREE`: 추가 유령 몬스터 에셋
- `Assets/ThirdParty/Free-FlashLight`: 손전등 모델/머티리얼
- `Assets/ThirdParty/Pure Poly/Free Low Poly Nature Pack`: 숲, 지형, 자연물 에셋
- `Assets/ThirdParty/Darth_Artisan/Free_Trees`: 나무 에셋
- `Assets/ThirdParty/Low Poly Stones`: 돌/경계 오브젝트 에셋
- `Assets/ThirdParty/Gentleland/SteampunkUI`: UI 그래픽, 프리팹, 폰트 리소스
- `Assets/ThirdParty/Mini Simple Characters Demo`: 캐릭터 모델 참고 에셋
- `Assets/OccaSoftware/Fantasy Music Pack`: BGM 오디오 파일
- `Assets/Monster_Orc (Troll)`: Orc/Troll 모델, 머티리얼, 텍스처, 애니메이터, 프리팹

외부 에셋의 라이선스와 원본 문서는 각 에셋 폴더 안의 문서와 라이선스 파일을 우선 확인합니다.

## Tests

- `Tests/PlayMode`: 플레이 흐름과 주요 게임 로직 검증
- `Tests/Editor`: 배치 실행과 씬/프리팹 검증 유틸리티
