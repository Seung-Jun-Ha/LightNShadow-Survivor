# 구현 백로그

이 문서는 [docs/implementation-status.md](docs/implementation-status.md)에서 분리한 실행용 작업 목록입니다. 구현 현황 수치, 리스크 요약, 다음 우선순위는 상태 문서를 먼저 읽고, 여기서는 실제로 손댈 항목만 확인합니다.

## P0 - 출시 전 필수 확인 작업

- [ ] GameScene에서 `GameManager`, `RoundManager`, `MonsterSpawner`, `UpgradeManager`, `UIManager`, `EndingManager` 실제 오브젝트/프리팹 연결 상태 점검. 완료 기준: 필수 참조가 모두 null 없이 연결되고, 새 게임 시작부터 엔딩/게임오버 진입까지 끊김이 없다.
- [ ] 각 몬스터 프리팹에 `MonsterBase`, `GhostAI`, `MonsterDeathHandler`, Collider, NavMeshAgent, XP 오브 프리팹 참조가 올바르게 붙어 있는지 검증. 완료 기준: 대표 몬스터 프리팹이 스폰/사망/보상 과정에서 예외 없이 동작한다.
- [ ] `MonsterDeathHandler`의 XP 오브 프리팹 누락 시 보상 손실 정책 결정: 프리팹 필수 연결로 강제하거나 fallback 직접 XP 지급 구현. 완료 기준: 누락이 발생해도 보상 손실이나 중복 지급이 없다.
- [ ] 실제 씬에서 시작 버튼 -> 라운드 -> 업그레이드 -> 다음 라운드 -> 엔딩/게임오버까지 수동 smoke test 수행. 완료 기준: 한 번의 플레이 루프에서 주요 화면 전환이 끊기지 않고 재현된다.

## P1 - 핵심 재미와 난이도 완성 작업

- [ ] 라운드별 몬스터 스폰 밀도, 속도, 체력, XP 보상 밸런스 1차 튜닝. 완료 기준: 라운드별 체감 난이도 곡선과 보상 속도가 문서화된 목표값에 맞는다.
- [ ] Boss/Shield/Teleport/Blob 몬스터별 패턴 완성도 점검 및 PlayMode 회귀 테스트 추가. 완료 기준: 각 변종마다 최소 1개 이상의 회귀 테스트가 있고, 패턴 전환/피격/사망이 검증된다.
- [ ] 업그레이드 카드 풀을 실제 플레이 기준으로 정리하고 중복/무효 업그레이드 선택 방지. 완료 기준: 중복/무효 카드가 선택 후보에서 제외되고, 빈 풀 상황의 fallback이 정의된다.
- [ ] 아이템 드롭 생성 위치 최적화와 특수 소모품 도입 여부 정리. 완료 기준: 드롭이 플레이 흐름을 방해하지 않고, 특수 소모품의 역할과 수급 규칙이 명확하다.
- [ ] 플레이어 조작감, 카메라 추적, 손전등 회전 반응속도 튜닝. 완료 기준: 이동, 조준, 공격 반응이 실제 플레이에서 일관되게 느껴진다.
- [ ] 게임오버/엔딩 진입 시 남은 몬스터, 아이템, 투사체 정리 정책 확정. 완료 기준: 상태 전환 시 남은 액터의 생존/파괴 규칙이 명확하고 중복 이벤트가 없다.

## P2 - 릴리즈 품질 보강 작업

- [ ] HUD, 업그레이드 패널, 게임오버, 결과 UI의 실제 씬 연결 및 해상도 대응 확인. 완료 기준: 대표 해상도에서 UI가 겹치거나 잘리지 않는다.
- [ ] Sunrise 엔딩의 조명, fog, 포스트프로세싱, 결과 UI 노출 타이밍 연출 개선. 완료 기준: 엔딩 진입 직후 조명 변화와 결과 UI 노출 순서가 연출 스펙과 일치한다.
- [ ] 전투/피격/수집/업그레이드/게임오버/엔딩 SFX와 최소 VFX 연결. 완료 기준: 핵심 액션마다 최소 1개 이상의 피드백이 재생된다.
- [ ] 몬스터 변종별 고유 공격/피격 애니메이션을 연결해 시각적 구분을 강화. 완료 기준: 기본/신속/쉴드/텔레포트/보스가 한눈에 구분된다.
- [ ] 테스트 obsolete API 경고 정리: `FindObjectsByType` 호출에서 deprecated overload 제거. 완료 기준: 빌드 경고가 남지 않는다.
- [ ] `PlayerAim.verticalLimit` 미사용 경고 제거 또는 실제 수직 조준 제한 로직에 연결. 완료 기준: 경고가 제거되거나 기능으로 연결된다.

## P3 - 확장 및 운영 작업

- [ ] 옵션/설정 저장 구조 검토. 완료 기준: 최소 저장 대상과 저장 시점, 복원 시점이 정의된다.
- [ ] 빌드 타깃별 입력/해상도/마우스 커서 동작 확인. 완료 기준: 플랫폼별 차이와 예외가 문서화된다.
- [ ] 프리팹 연결 검증용 Editor 테스트 또는 scene validation 스크립트 추가. 완료 기준: 누락된 참조가 빌드 전에 실패로 드러난다.
- [ ] QA 체크리스트를 실제 플레이 테스트 결과와 연결. 완료 기준: 체크리스트에서 pass/fail과 근거 링크를 남길 수 있다.

## 테스트 실행 메모

Unity 표준 `-runTests`가 현재 환경에서 초기 컴파일 타이밍 때문에 테스트 실행까지 이어지지 않는 경우가 있어, `BatchPlayModeTestRunner`를 통해 TestRunner API를 직접 호출하는 방식으로 검증했습니다.

대표 실행 명령:

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Unity.exe' -batchmode -projectPath 'C:\Users\tjdgns\UnityProj\LNS_seungJun\LightNShadow-Survivor' -executeMethod LightNShadowSurvivor.Tests.BatchPlayModeTestRunner.RunFlashlightTests -testResults 'C:\Users\tjdgns\UnityProj\LNS_seungJun\LightNShadow-Survivor\Temp\collision-playmode-test-results.xml' -logFile 'C:\Users\tjdgns\UnityProj\LNS_seungJun\LightNShadow-Survivor\Temp\collision-playmode-test.log'
```
