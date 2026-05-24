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

        private void Start()
        {
            ChangeState(GameState.Intro);
        }

        public void ChangeState(GameState newState)
        {
            currentState = newState;
            Debug.Log($"Game State Changed to: {newState}");
            
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
                    Time.timeScale = 0f;
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
