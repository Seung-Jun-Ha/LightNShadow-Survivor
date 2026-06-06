using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

namespace LightNShadowSurvivor
{
    public enum GameState { MainMenu, Intro, RoundIntro, Round, Upgrade, Ending, GameOver }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private string upgradeSceneName = "SkillScene";
        [SerializeField] private string gameOverSceneName = "gameOver_Scene";
        [SerializeField] private float roundIntroDuration = 2f;

        public event Action<GameState> OnStateChanged;
        
        private GameState currentState;
        public GameState CurrentState => currentState;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // DontDestroyOnLoad(gameObject); // We are in a single-scene setup mostly, but good practice
            }
            else Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private IEnumerator Start()
        {
            yield return StartCoroutine(LoadUISceneRoutine());
            ChangeState(GameState.MainMenu);
        }

        private IEnumerator LoadUISceneRoutine()
        {
            string scenePath = "Assets/_Project/Scenes/UIScene.unity";
            if (!UnityEngine.SceneManagement.SceneManager.GetSceneByPath(scenePath).isLoaded)
            {
                Debug.Log($"[GameManager] Loading {scenePath} additively...");
                var op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scenePath, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                if (op == null)
                {
                    Debug.LogError($"[GameManager] Failed to start loading {scenePath}. Check if it's in Build Settings.");
                    yield break;
                }
                yield return op;
                Debug.Log($"[GameManager] {scenePath} loaded successfully.");
            }
            else
            {
                Debug.Log($"[GameManager] {scenePath} was already loaded.");
            }
        }

        public void ChangeState(GameState newState)
        {
            if (currentState == newState && newState != GameState.Round)
            {
                StartCoroutine(SyncStateSceneRoutine(newState));
                return;
            }
            
            GameState previousState = currentState;
            currentState = newState;
            Debug.Log($"[GameManager] State changed to: {newState}");
            
            switch (newState)
            {
                case GameState.MainMenu:
                    Time.timeScale = 0f;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    break;
                case GameState.Intro:
                    // Initialize or show intro UI
                    break;
                case GameState.RoundIntro:
                    Time.timeScale = 0f;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    break;
                case GameState.Round:
                    if (previousState == GameState.MainMenu || previousState == GameState.GameOver || previousState == GameState.Ending)
                    {
                        if (GameStatsManager.Instance != null)
                        {
                            GameStatsManager.Instance.ResetStats();
                        }
                    }

                    Time.timeScale = 1f;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    break;
                case GameState.Upgrade:
                    Time.timeScale = 0f;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    EnsureUpgradeUIController();
                    break;
                case GameState.Ending:
                    Time.timeScale = 1f;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    break;
                case GameState.GameOver:
                    Time.timeScale = 0f;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    break;
            }

            OnStateChanged?.Invoke(newState);
            StartCoroutine(SyncStateSceneRoutine(newState));
        }

        public void StartRound() => ChangeState(GameState.Round);
        public void ShowRoundIntroThenStart(int round) => StartCoroutine(RoundIntroRoutine(round));
        public void OpenUpgrade() => ChangeState(GameState.Upgrade);
        public void TriggerEnding() => ChangeState(GameState.Ending);
        public void TriggerGameOver() => ChangeState(GameState.GameOver);

        private IEnumerator RoundIntroRoutine(int round)
        {
            ChangeState(GameState.RoundIntro);
            GameObject introCanvas = CreateRoundIntroCanvas(round);
            yield return new WaitForSecondsRealtime(Mathf.Max(0.1f, roundIntroDuration));

            if (introCanvas != null)
            {
                Destroy(introCanvas);
            }

            StartRound();
        }

        private GameObject CreateRoundIntroCanvas(int round)
        {
            GameObject canvasObject = new GameObject("RuntimeRoundSceneCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 180;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            GameObject backgroundObject = new GameObject("RoundSceneBackground", typeof(RectTransform), typeof(Image));
            backgroundObject.transform.SetParent(canvasObject.transform, false);
            RectTransform backgroundRect = backgroundObject.GetComponent<RectTransform>();
            backgroundRect.anchorMin = Vector2.zero;
            backgroundRect.anchorMax = Vector2.one;
            backgroundRect.offsetMin = Vector2.zero;
            backgroundRect.offsetMax = Vector2.zero;

            Image background = backgroundObject.GetComponent<Image>();
            background.color = new Color(0.02f, 0.018f, 0.025f, 0.72f);

            GameObject textObject = new GameObject("RoundSceneText", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(backgroundObject.transform, false);
            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.5f, 0.5f);
            textRect.anchorMax = new Vector2(0.5f, 0.5f);
            textRect.anchoredPosition = Vector2.zero;
            textRect.sizeDelta = new Vector2(900f, 180f);

            TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
            text.text = $"ROUND {round}";
            text.fontSize = 96f;
            text.fontStyle = FontStyles.Bold;
            text.alignment = TextAlignmentOptions.Center;
            text.color = new Color(1f, 0.92f, 0.72f, 1f);

            return canvasObject;
        }

        private void EnsureUpgradeUIController()
        {
            UpgradeUIController controller = FindAnyObjectByType<UpgradeUIController>(FindObjectsInactive.Include);
            if (controller == null)
            {
                GameObject controllerObject = new GameObject("RuntimeUpgradeUIController");
                controller = controllerObject.AddComponent<UpgradeUIController>();
                Debug.Log("[GameManager] Created RuntimeUpgradeUIController for upgrade selection.");
            }

            controller.gameObject.SetActive(true);
            controller.ShowForCurrentUpgrade();
        }

        private IEnumerator SyncStateSceneRoutine(GameState state)
        {
            if (state == GameState.Upgrade)
            {
                yield return LoadSceneIfNeeded(upgradeSceneName);
                yield return UnloadSceneIfLoaded(gameOverSceneName);
                yield break;
            }

            if (state == GameState.GameOver)
            {
                yield return LoadSceneIfNeeded(gameOverSceneName);
                yield return UnloadSceneIfLoaded(upgradeSceneName);
                yield break;
            }

            yield return UnloadSceneIfLoaded(upgradeSceneName);
            yield return UnloadSceneIfLoaded(gameOverSceneName);
        }

        private IEnumerator LoadSceneIfNeeded(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName)) yield break;

            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (scene.isLoaded) yield break;

            Debug.Log($"[GameManager] Loading scene '{sceneName}' additively.");
            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (op == null)
            {
                Debug.LogError($"[GameManager] Failed to load scene '{sceneName}'. Check Build Settings.");
                yield break;
            }

            yield return op;
        }

        private IEnumerator UnloadSceneIfLoaded(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName)) yield break;

            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded) yield break;

            var op = SceneManager.UnloadSceneAsync(scene);
            if (op != null) yield return op;
        }
    }
}
