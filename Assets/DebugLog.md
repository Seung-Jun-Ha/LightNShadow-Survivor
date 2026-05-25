# 🐞 Light & Shadow Survivor - Debug & Bug Tracking

## 🚩 실시간 시뮬레이션 기반 오류 추적

### 1. [잠재적 버그] UI 로딩 경로 중복 및 누락
- **현상**: `GameManager`에서 `UIScene`을 로드할 때 경로 오타(`Assets/Scenes` vs `Assets/_Project/Scenes`)로 인해 UI가 나타나지 않았던 기록이 있음.
- **상태**: 수정 완료. 단, Build Settings에 해당 씬이 반드시 포함되어 있어야 함.

### 2. [로직 오류] 다중 레벨업 시 업그레이드 창 중복
- **현상**: 한 번에 많은 경험치를 얻어 레벨이 2단계 이상 상승할 경우, `UpgradeManager`가 동시에 여러 번 호출될 가능성이 있음.
- **해결책**: 레벨업 처리를 큐(Queue) 방식으로 하거나, 업그레이드 창이 열려 있을 때는 레벨업 로직을 일시 중단하는 처리가 필요함.

### 3. [성능/경고] Obsolete API 사용
- **현상**: `FindFirstObjectByType<T>`가 다수의 스크립트(`RoundManager`, `EndingManager`, `UpgradeManager` 등)에서 사용됨. Unity 2023+ 버전에서는 성능 상 `FindAnyObjectByType<T>` 사용을 권장함.
- **상태**: 현재 작동에는 문제없으나 최적화를 위해 교체 권장.

### 4. [충돌 판정] 플레이어-몬스터 통과 로직
- **현상**: 플레이어를 Layer 3으로 설정하여 몬스터와 충돌을 무시하게 했으나, 만약 Layer 3에 다른 물리 오브젝트(예: 아이템)가 있을 경우 원치 않는 물리 반응이 일어날 수 있음.
- **권장**: 전용 Layer 이름을 'Player'로 명명하고 다른 레이어와의 충돌 행렬(Collision Matrix)을 전수 검사해야 함.

### 5. [AI 오류] NavMesh 접근성
- **현상**: 맵 경계에 돌을 배치하면서 일부 스폰 지점이 NavMesh 바깥으로 잡힐 경우, 몬스터가 제자리에서 움직이지 못할 수 있음.
- **상태**: `MonsterSpawner`에서 `SamplePosition`을 통해 보정하고 있으나, 맵 확장에 따라 스폰 반경 재조정 필요.

### 6. [연출 오류] Wasted 시퀀스 시간 정지
- **현상**: `Time.timeScale = 0`인 상태에서 UI 애니메이션이 멈출 수 있음.
- **해결책**: `GameOverUIController`에서 `Time.unscaledDeltaTime`을 사용하도록 수정 완료.

---

## 🧪 2차 시뮬레이션 및 검증 테스트 (2025-05-16)

### [검증 완료] 핵심 시스템
- **플레이어 설정**: Layer 3 할당 및 필수 컴포넌트(Controller, Aim, Health, Exp, Attack) 정상 부착 확인.
- **매니저 오브젝트**: `Managers` 오브젝트 산하의 모든 싱글톤 클래스(Game, Round, Upgrade, Audio, Stats) 로드 확인.
- **스폰 시스템**: 라운드별 몬스터 풀(Pool) 및 보스 프리팹 할당 정상 확인.
- **UI 시스템**: 메인 메뉴, 게임 오버, 결과창, 업그레이드 패널 계층 구조 및 컨트롤러 부착 확인.

### [발견된 버그 및 수정 필요]
1. **[수정 완료] UIScene 경로 불일치**: `GameManager`에서 `Assets/Scenes`를 참조하던 문제를 `Assets/_Project/Scenes`로 수정하여 UI 정상 출력 보장.
2. **[수정 완료] Build Settings 누락**: `UIScene`이 빌드 설정에 포함되지 않아 런타임 오류가 발생할 수 있었던 점을 수정.
3. **[알림] ResultUIController 감지 이슈**: `FindAnyObjectByType`으로 비활성화된 결과창 컨트롤러를 찾지 못하는 경우가 있었으나, 실제 계층 구조에는 정상 부착되어 있음을 확인. (런타임 시 `GameManager` 연동에는 문제 없음)

### [성능 최적화 권장]
- **NavMesh**: 대량의 돌 장벽 추가로 인해 NavMesh 데이터가 무거워질 수 있으므로, 장애물이 없는 중앙 구역의 베이킹 설정을 최적화할 것.
- **Object Pooling**: 현재는 `Instantiate/Destroy` 방식을 사용 중이나, 몬스터 스폰량이 많아질 경우 가비지 컬렉션(GC) 부하를 방지하기 위해 오브젝트 풀링 도입 권장.
