# 개발 및 검증 가이드

이 문서는 공개 저장소에서 프로젝트를 실행하고 확인하는 방법을 안내합니다.

## Unity에서 열기

1. Unity Hub를 실행합니다.
2. Unity 6 `6000.4.7f1` 에디터가 설치되어 있는지 확인합니다.
3. `LightNShadow-Survivor` 폴더를 프로젝트로 추가합니다.
4. 프로젝트를 엽니다.
5. `ProjectSettings/ProjectVersion.txt`의 `m_EditorVersion`이 `6000.4.7f1`인지 확인합니다.
6. `Assets/_Project/Scenes/GameScene.unity`를 엽니다.
7. Play 버튼을 눌러 실행합니다.

## Unity 버전 및 패키지

- Unity: Unity 6 `6000.4.7f1`
- URP: `com.unity.render-pipelines.universal` 17.4.0
- Input System: `com.unity.inputsystem` 1.19.0
- AI Navigation: `com.unity.ai.navigation` 2.0.12
- Unity UI: `com.unity.ugui` 2.0.0
- Test Framework: `com.unity.test-framework` 1.6.0

패키지 전체 목록은 `Packages/manifest.json`에서 확인합니다.

## 주요 씬

- `Assets/_Project/Scenes/GameScene.unity`: 메인 플레이 씬
- `Assets/_Project/Scenes/UIScene.unity`: UI 구성 씬
- `Assets/_Project/Scenes/SkillScene.unity`: 기능 확인용 씬
- `Assets/_Project/Scenes/gameOver_Scene.unity`: 게임 오버 화면 관련 씬

## 테스트 실행

Unity Test Runner에서 PlayMode 테스트를 실행할 수 있습니다.

테스트 위치:

```text
Assets/_Project/Tests/PlayMode
Assets/_Project/Tests/Editor
```

대표 테스트 범위:

- 손전등 공격 범위와 장애물 차단
- 플레이어 이동과 조준
- 맵, 플레이어, 몬스터 충돌 규칙
- 라운드 진행과 엔딩 흐름
- 몬스터 사망과 경험치 보상
- 씬과 프리팹 참조 검증

## 배치 테스트 예시

환경에 설치된 Unity 경로에 맞춰 실행 경로를 조정합니다.

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Unity.exe' `
  -batchmode `
  -projectPath 'C:\Users\tjdgns\UnityProj\LNS_seungJun\LightNShadow-Survivor' `
  -executeMethod LightNShadowSurvivor.Tests.BatchPlayModeTestRunner.RunFlashlightTests `
  -testResults 'C:\Users\tjdgns\UnityProj\LNS_seungJun\LightNShadow-Survivor\Temp\flashlight-playmode-test-results.xml' `
  -logFile 'C:\Users\tjdgns\UnityProj\LNS_seungJun\LightNShadow-Survivor\Temp\flashlight-playmode-test.log'
```

## 개발 규칙

- 게임 상태 변경은 매니저 계층에서 처리합니다.
- 플레이어, 몬스터, UI 스크립트는 직접 서로를 과도하게 제어하지 않습니다.
- 밸런스 수치는 ScriptableObject 또는 직렬화 필드로 관리합니다.
- 씬과 프리팹은 직렬화 참조가 명확하게 연결되도록 유지합니다.
- 테스트 코드는 핵심 플레이 규칙과 회귀 케이스를 중심으로 작성합니다.

## 공개 저장소 관리

- `Library/`, `Temp/`, `obj/`, `Logs/` 같은 Unity 생성 폴더는 커밋하지 않습니다.
- 씬, 프리팹, 머티리얼 변경은 Unity 에디터 저장 후 diff를 확인합니다.
- 코드 변경 후 관련 PlayMode 테스트를 실행합니다.
- 문서는 프로젝트 구조나 실행 방법이 바뀔 때 함께 갱신합니다.
- 외부 에셋을 추가하거나 교체할 때는 `docs/repository-structure.md`의 에셋 구성을 함께 갱신합니다.
