using UnityEngine;
using System;

namespace LightNShadowSurvivor
{
    public class RoundManager : MonoBehaviour
    {
        public static RoundManager Instance { get; private set; }

        [Header("Round Settings (Seconds)")]
        [SerializeField] private float round1Duration = 60f;
        [SerializeField] private float round2Duration = 60f;
        [SerializeField] private float round3Duration = 120f;

        private int currentRound = 1;
        private float timer;
        private float elapsedTime;
        private bool isTimerRunning;

        public event Action<int> OnRoundStarted;
        public event Action<int> OnRoundEnded;
        public event Action<float, float> OnRoundTimeChanged;

        public int CurrentRound => currentRound;
        public float TimeRemaining => timer;
        public float TimeElapsed => elapsedTime;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += HandleStateChanged;
                HandleStateChanged(GameManager.Instance.CurrentState);
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.Round)
            {
                StartRoundTimer();
            }
            else
            {
                isTimerRunning = false;
            }
        }

        private void StartRoundTimer()
        {
            timer = GetRoundDuration(currentRound);
            elapsedTime = 0f;
            isTimerRunning = true;

            var spawner = FindAnyObjectByType<MonsterSpawner>();
            if (spawner != null) spawner.SetRound(currentRound);

            OnRoundStarted?.Invoke(currentRound);
            OnRoundTimeChanged?.Invoke(timer, elapsedTime);
        }

        private void Update()
        {
            if (!isTimerRunning) return;

            float delta = Time.deltaTime;
            elapsedTime += delta;
            timer = Mathf.Max(0f, timer - delta);
            OnRoundTimeChanged?.Invoke(timer, elapsedTime);

            if (timer <= 0f)
            {
                EndRound();
            }
        }

        private void EndRound()
        {
            if (!isTimerRunning) return;

            isTimerRunning = false;
            OnRoundEnded?.Invoke(currentRound);

            var spawner = FindAnyObjectByType<MonsterSpawner>();
            if (spawner != null) spawner.StopAndClearMonsters();

            if (GameManager.Instance == null) return;

            if (currentRound < 3)
            {
                GameManager.Instance.OpenUpgrade();
            }
            else
            {
                GameManager.Instance.TriggerEnding();
            }
        }

        public void ProceedToNextRound()
        {
            currentRound = Mathf.Clamp(currentRound + 1, 1, 3);
            if (GameManager.Instance != null) GameManager.Instance.StartRound();
        }

        private float GetRoundDuration(int round)
        {
            switch (round)
            {
                case 1: return round1Duration;
                case 2: return round2Duration;
                default: return round3Duration;
            }
        }
    }
}

