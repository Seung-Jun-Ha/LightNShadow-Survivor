# Light & Shadow Survivor

Unity 기반 3인칭 쿼터뷰 생존 액션 프로젝트입니다. 플레이어는 밤의 숲에서 손전등 빛을 무기로 사용해 적을 막아내고, 라운드 진행과 업그레이드를 통해 생존 루프를 이어갑니다.

## 프로젝트 개요

- 엔진: Unity
- 장르: 생존 액션
- 핵심 메커니즘: 손전등의 방향, 거리, 각도를 이용한 지속 피해 전투
- 주요 흐름: 시작 화면 -> 라운드 전투 -> 업그레이드 선택 -> 다음 라운드 -> 엔딩 또는 게임 오버

## 주요 시스템

- 플레이어 조작: WASD 이동, 마우스 조준, 손전등 방향 제어
- 빛 전투: 거리, 각도, 장애물 판정을 기반으로 적에게 지속 피해 적용
- 몬스터 AI: 기본 추적형 적, 보호막형, 순간이동형, 보스형 적 구조
- 라운드 진행: 라운드 타이머, 스폰 제어, 게임 상태 전환
- 성장 시스템: 경험치 오브, 레벨업, 업그레이드 카드 선택
- UI 흐름: 메인 메뉴, HUD, 업그레이드 패널, 게임 오버, 결과 화면
- 연출 시스템: 시간대 변화, 오디오 매니저, 타격 VFX, 엔딩 흐름

## 문서

- [게임 개요](docs/game-overview.md)
- [저장소 구조](docs/repository-structure.md)
- [코드 아키텍처](docs/code-architecture.md)
- [시스템 구성](docs/system-overview.md)
- [개발 및 검증 가이드](docs/development-guide.md)

## 코드 위치

핵심 게임 코드는 `Assets/_Project/Scripts` 아래에 있습니다.

- `Core`: 카메라, 스테이지, 시간대 전환
- `Managers`: 게임 상태, 라운드, UI, 오디오, 업그레이드, 엔딩 관리
- `Player`: 이동, 조준, 체력, 경험치, 사망 처리, 보조 공격
- `Enemies`: 몬스터 공통 로직, 추적 AI, 스폰, 사망 처리, 특수 적
- `Items`: 경험치 오브와 회복/속도/빛 강화 아이템
- `UI`: 메뉴, 업그레이드 카드, 게임 오버, 결과 화면
- `Upgrades`: 업그레이드 데이터 정의

## 실행

1. Unity Hub에서 프로젝트 폴더 `LightNShadow-Survivor`를 엽니다.
2. `Assets/_Project/Scenes/GameScene.unity`를 엽니다.
3. Unity Editor의 Play 버튼으로 실행합니다.

## 테스트

테스트 코드는 `Assets/_Project/Tests` 아래에 있습니다.

- `PlayMode`: 손전등 판정, 이동, 충돌, 라운드/엔딩 흐름, 회귀 테스트
- `Editor`: 배치 테스트 실행기와 씬/프리팹 검증기

Unity Test Runner 또는 배치 실행기를 사용해 PlayMode 테스트를 실행할 수 있습니다.
