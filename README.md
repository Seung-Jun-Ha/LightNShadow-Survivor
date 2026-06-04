# Light & Shadow Survivor

이 저장소는 Unity 기반의 `Light & Shadow Survivor` 프로젝트를 정리한 작업 공간입니다. 핵심 문서는 아래 5개로 나눠서 유지보수하기 쉽게 관리합니다.
파일 이름은 'LightNShadow'로 통일합니다.

## 문서 안내

- [프로젝트 구조](docs/project-structure.md)
- [구현 방식](docs/implementation-method.md)
- [목적 및 목표](docs/purpose-and-goals.md)
- [구현 현황 및 전체 구현율](docs/implementation-status.md)
- [구현 백로그](docs/implementation-backlog.md)

## 한 줄 요약

밤의 숲에서 손전등의 빛으로 유령 계열 몬스터를 막아내며, 3라운드를 버티고 일출까지 생존하는 3인칭 쿼터뷰 생존 액션 게임입니다.

## 현재 구현 상태

- 핵심 로직 구현률: 약 72%
- 플레이 가능한 MVP 완성도: 약 65%
- 릴리즈 후보 완성도: 약 55%
- 전체 구현율: 약 60%

상세 산정 근거와 남은 리스크는 [구현 현황 및 전체 구현율](docs/implementation-status.md)을 기준으로 관리하고, 실제 작업 목록은 [구현 백로그](docs/implementation-backlog.md)로 분리합니다.

## 유지보수 원칙

- 구조 변경은 `docs/project-structure.md`를 먼저 갱신합니다.
- 기능 추가나 밸런스 조정은 `docs/implementation-method.md`의 규칙을 따릅니다.
- 구현률과 남은 리스크는 `docs/implementation-status.md`에, 실행할 작업은 `docs/implementation-backlog.md`에 반영합니다.
- 게임의 방향성 변경은 `docs/purpose-and-goals.md`에서 먼저 정리합니다.
