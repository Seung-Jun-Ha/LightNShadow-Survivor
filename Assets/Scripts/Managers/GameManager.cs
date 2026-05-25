using UnityEngine;
using System;

namespace LightNShadowSurvivor
{
    public enum GameState { Intro, Round, Upgrade, Ending, GameOver }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public event Action<GameState> OnStateChanged;
        
        private GameState currentState;
        public GameState CurrentState => currentState;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private System.Collections.IEnumerator Start()
        {
            yield return StartCoroutine(LoadUISceneRoutine());
            ChangeState(GameState.Intro);
        }

        private System.Collections.IEnumerator LoadUISceneRoutine()
        {
            string scenePath = "Assets/Scenes/UIScene.unity";
            if (!UnityEngine.SceneManagement.SceneManager.GetSceneByPath(scenePath).isLoaded)
            {
                Debug.Log($"[GameManager] Loading {scenePath} additively...");
                var op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scenePath, UnityEngine.SceneManagement.LoadSceneMode.Additive);
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
            
            currentState = newState;
            Debug.Log($"[GameManager] State changed to: {newState}");
            
            switch (newState)
            {
                case GameState.Intro:
                    // Initialize or show intro UI
                    break;
                case GameState.Round:
                    Time.timeScale = 1f;
                    break;
                case GameState.Upgrade:
                    Time.timeScale = 0f;
                    break;
                case GameState.Ending:
                    break;
                case GameState.GameOver:
                    // Time.timeScale = 0f; // Let the death handler finish its work
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
