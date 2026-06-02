# 구현 방식

이 문서는 기능을 어떤 순서와 기준으로 구현할지 정리한 문서입니다. 핵심은 시나리오를 한 번에 다 만들지 않고, 플레이 루프부터 엔딩까지 작은 단위로 나눠서 붙이는 것입니다.

## 구현 원칙

- 라운드 흐름과 게임 상태는 한 곳에서만 관리합니다.
- 손전등 공격은 거리, 각도, 시야 조건을 분리해서 계산합니다.
- 피해량, 사거리, 각도, 이동 속도 같은 수치는 데이터화합니다.
- UI는 표시만 담당하고, 게임 로직은 Manager와 Entity Script가 담당합니다.
- 타이머와 지속 피해는 `Time.deltaTime` 기반으로 처리합니다.

## 추천 구현 순서

1. `PlayerController`, `PlayerAim`, `CameraFollow`로 기본 조작과 시점을 먼저 완성합니다.
2. `FlashLightAttack`과 `LightDamageReceiver`로 빛 공격과 피격 판정을 연결합니다.
3. `MonsterBase`, `GhostAI`, `MonsterSpawner`로 기본 적의 생성과 추적을 붙입니다.
4. `RoundManager`, `GameCycleManager`, `GameManager`로 3라운드 흐름을 완성합니다.
5. `UpgradeManager`, `UpgradeUIController`, `UpgradeData`로 성장 선택을 연결합니다.
6. `BossBase`와 특수 몬스터를 추가해 라운드 후반 변화를 만듭니다.
7. `EndingManager`, `TimeOfDayManager`, `ResultUIController`로 Sunrise 엔딩을 마무리합니다.

## 시스템별 구현 방식

### 플레이어

- 이동은 `PlayerController`가 담당합니다.
- 마우스 조준은 `PlayerAim`이 담당합니다.
- 체력, 경험치, 사망은 각각 별도 스크립트로 나눕니다.

### 손전등 공격

- `FlashLightAttack`은 빛의 방향과 범위를 계산합니다.
- 범위 판정은 거리, 각도, 장애물 순서로 검사합니다.
- 판정에 들어온 몬스터만 지속 피해를 받습니다.

### 몬스터

- `MonsterBase`는 HP와 사망 처리를 공통화합니다.
- `GhostAI`는 추적과 기본 행동만 책임집니다.
- 특수 행동은 `ShieldGhost`, `TeleportGhost`처럼 파생 타입으로 분리합니다.

### 라운드와 연출

- `RoundManager`가 라운드 제한 시간과 전환 조건을 관리합니다.
- `GameCycleManager`는 Start, Round, Upgrade, Ending, GameOver 상태를 묶습니다.
- `TimeOfDayManager`는 밤에서 새벽으로 넘어가는 조명 변화를 담당합니다.

### 성장과 업그레이드

- 업그레이드 카드 정보는 `UpgradeData`와 `Assets/_Project/Data/Upgrades/`에 둡니다.
- `UpgradeManager`는 선택 UI를 띄우고, 선택 결과를 플레이어 능력치에 반영합니다.
- 수치 증가는 `flashDamagePerSecond`, `flashRange`, `flashAngle`, `moveSpeed`, `maxHP`처럼 명확한 변수명으로 관리합니다.

## 유지보수 기준

- 한 기능이 두 개 이상의 시스템을 동시에 직접 수정해야 한다면 구조를 다시 나눕니다.
- 라운드별 차이는 스폰 테이블과 데이터 값으로 처리하고, 코드 분기만으로 밀어 넣지 않습니다.
- 보스 패턴은 공통 베이스와 개별 패턴 클래스로 나눕니다.
- 엔딩 연출은 게임 오브젝트 파괴보다 상태 전환과 이펙트 제어를 먼저 적용합니다.

---

## 2026-06-02 구현 현황 문서 연결

최신 구현률과 남은 리스크는 `docs/implementation-status.md`에서 관리합니다.

- 핵심 로직 구현률: 약 72%
- 플레이 가능한 MVP 완성도: 약 65%
- 릴리즈 후보 완성도: 약 55%
- 전체 구현율: 약 60%

새 기능 추가 또는 구조 변경 후에는 해당 문서의 영역별 구현률과 검증 결과를 함께 갱신합니다.
