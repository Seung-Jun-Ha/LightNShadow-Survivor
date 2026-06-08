# 코드 아키텍처

이 문서는 코드가 어떤 방식으로 구성되어 있는지 설명합니다. GitHub에서 프로젝트를 읽는 사람이 주요 시스템의 책임과 연결 방식을 빠르게 파악하는 것을 목표로 합니다.

## 전체 흐름

게임은 매니저 계층이 상태를 관리하고, 플레이어/몬스터/아이템 스크립트가 실제 행동을 처리하는 구조입니다.

```text
GameManager
  -> RoundManager
  -> MonsterSpawner
  -> UpgradeManager
  -> UIManager
  -> EndingManager
```

플레이어와 적은 직접 UI를 제어하지 않습니다. 상태 변화는 매니저를 통해 화면, 오디오, 통계, 엔딩 흐름으로 전달됩니다.

## 플레이어 시스템

`PlayerController`는 이동을 담당하고, `PlayerAim`은 마우스 위치를 기준으로 조준 방향을 계산합니다. 체력, 경험치, 사망 처리는 각각 `PlayerHealth`, `PlayerExperience`, `PlayerDeathHandler`로 분리되어 있습니다.

이 분리 덕분에 이동, 조준, 성장, 사망 로직을 독립적으로 테스트하고 수정할 수 있습니다.

## 손전등 전투

`FlashLightAttack`은 빛 공격의 중심 스크립트입니다.

판정 순서:

1. 대상과의 거리 확인
2. 손전등 방향 기준 각도 확인
3. 장애물 레이어를 이용한 시야 차단 확인
4. `LightDamageReceiver`를 통해 지속 피해 적용

`LightDamageReceiver`는 빛 피해를 받는 대상의 공통 진입점입니다. 몬스터 타입별 반응은 이 컴포넌트와 몬스터 스크립트의 조합으로 처리됩니다.

## 몬스터 시스템

`MonsterBase`는 체력과 사망 상태를 관리합니다. `GhostAI`는 NavMeshAgent 기반 추적과 거리 기반 공격 흐름을 담당합니다.

특수 적은 공통 베이스 위에 개별 동작을 얹는 방식으로 구성됩니다.

- `ShieldGhost`: 보호막 반응
- `TeleportGhost`: 순간이동 행동
- `BossBase`: 보스 패턴의 공통 기반
- `BlobShadow`: 그림자 계열 보조 오브젝트

`MonsterDeathHandler`는 사망 시 통계, 경험치 보상, 이펙트 생성을 연결합니다.

## 라운드와 게임 상태

`RoundManager`는 라운드 시간과 진행 상태를 관리합니다. `GameCycleManager`와 `GameManager`는 시작, 진행, 업그레이드, 엔딩, 게임 오버 상태를 연결합니다.

몬스터 생성은 `MonsterSpawner`가 담당하며, 라운드별 프리팹 목록과 생성 규칙을 사용합니다.

## 업그레이드 시스템

업그레이드 데이터는 `UpgradeData` ScriptableObject로 정의됩니다. 실제 자산은 `Assets/_Project/Data/Upgrades`에 저장됩니다.

`UpgradeManager`는 업그레이드 후보를 선택하고, `UpgradeUIController`와 `UpgradeCardUI`는 화면 표시와 선택 입력을 처리합니다.

대표 업그레이드:

- 이동 속도 증가
- 최대 체력 증가
- 손전등 피해량 증가
- 손전등 범위 증가
- Will-O-The-Wisp 보조 공격

## UI와 오디오

UI는 `Scripts/UI`의 컨트롤러들이 각 화면을 담당하고, `UIManager`가 게임 상태에 맞는 화면 전환을 관리합니다. 버튼 효과음은 `ButtonSoundTrigger`를 통해 `AudioManager`로 연결됩니다.

`AudioManager`는 라운드 BGM, 엔딩 음악, 전투 및 UI SFX를 중앙에서 재생하는 구조입니다.

## 테스트 구성

PlayMode 테스트는 주요 런타임 동작을 검증합니다.

- 손전등 범위/각도/장애물 판정
- 플레이어 이동과 조준
- 충돌 규칙
- 라운드에서 엔딩까지의 흐름
- 보상과 사망 처리 회귀 테스트

Editor 테스트 유틸리티는 씬과 프리팹의 직렬화 참조를 확인하는 데 사용됩니다.
