# LightNShadowSurviver 프로젝트 요약

본 문서는 현재까지 개발된 **LightNShadowSurviver** 프로젝트의 주요 시스템 및 파일 구조를 요약합니다.

## 1. 핵심 시스템 (Core Systems)
*   **TimeOfDayManager**: 게임 내 시간 흐름 및 낮/밤 주기를 관리합니다.
*   **StageManager**: 각 스테이지의 구성 및 설정을 담당합니다.
*   **CameraFollow**: 플레이어를 추적하는 카메라 로직을 제공합니다.

## 2. 매니저 시스템 (Managers)
*   **GameManager**: 전반적인 게임 상태(시작, 진행, 일시정지 등)를 제어합니다.
*   **RoundManager**: 전투 라운드와 웨이브 시스템을 관리합니다.
*   **EndingManager**: 게임의 종료(승리/패배) 조건을 확인하고 연출합니다.

## 3. 플레이어 (Player)
*   **PlayerController**: 플레이어의 이동 및 전반적인 입력을 처리합니다. (New Input System 사용)
*   **PlayerHealth**: 플레이어의 체력 및 대미지 판정을 관리합니다.
*   **PlayerAim**: 마우스 또는 패드를 이용한 조준 로직을 담당합니다.
*   **FlashLightAttack**: 손전등 빛을 이용한 주 공격 메커니즘입니다.

## 4. 적 시스템 (Enemies)
*   **MonsterBase**: 모든 몬스터의 공통 기능을 담은 기본 클래스입니다.
*   **GhostAI**: 유령 타입 적의 추적 및 행동 로직입니다.
*   **MonsterSpawner**: 게임 진행에 맞춰 몬스터를 생성합니다.
*   **MonsterDeathHandler**: 몬스터 사망 시의 처리(이펙트, 데이터 갱신 등)를 담당합니다.
*   **BlobShadow**: 적 캐릭터 아래의 그림자 효과를 처리합니다.

## 5. UI 시스템
*   **HUDController**: 게임 화면상의 실시간 정보(체력, 라운드 등)를 표시합니다.
*   **UpgradeUIController**: 플레이어의 능력치를 강화하는 업그레이드 화면을 관리합니다.

## 6. 기타 (Interaction)
*   **LightDamageReceiver**: 빛에 반응하여 대미지를 입는 인터페이스 또는 컴포넌트입니다.

## 향후 과제 (To-Do)
*   [ ] 신규 스테이지 밸런싱
*   [ ] 추가 업그레이드 항목 구현
*   [ ] 사운드 이펙트 및 배경음악 통합
*   [ ] 폴리싱 및 최적화
