using UnityEngine;
using System;

namespace LightNShadowSurvivor
{
    public enum GameState { MainMenu, Intro, Round, Upgrade, Ending, GameOver }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

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

        private System.Collections.IEnumerator Start()
        {
            yield return StartCoroutine(LoadUISceneRoutine());
            ChangeState(GameState.MainMenu);
        }

        private System.Collections.IEnumerator LoadUISceneRoutine()
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
            if (currentState == newState && newState != GameState.Round) return; // Allow re-entering Round for next waves
            
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
        }

        public void StartRound() => ChangeState(GameState.Round);
        public void OpenUpgrade() => ChangeState(GameState.Upgrade);
        public void TriggerEnding() => ChangeState(GameState.Ending);
        public void TriggerGameOver() => ChangeState(GameState.GameOver);
    }
}
