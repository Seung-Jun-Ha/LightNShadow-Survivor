using UnityEngine;
using System.Collections;

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
        private bool isTimerRunning = false;

        public int CurrentRound => currentRound;
        public float TimeRemaining => timer;

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
                // Handle initial state
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
            if (state == GameState.Intro)
            {
                // Auto start round 1 for now
                StartCoroutine(DelayedStart());
            }
            else if (state == GameState.Round)
            {
                StartRoundTimer();
            }
        }

        private IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(2f);
            GameManager.Instance.StartRound();
        }

        private void StartRoundTimer()
        {
            switch (currentRound)
            {
                case 1: timer = round1Duration; break;
                case 2: timer = round2Duration; break;
                case 3: timer = round3Duration; break;
            }
            isTimerRunning = true;
            
            // Update Spawner
            var spawner = FindFirstObjectByType<MonsterSpawner>();
            if (spawner != null) spawner.SetRound(currentRound);
        }

        private void Update()
        {
            if (!isTimerRunning) return;

            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0;
                EndRound();
            }
        }

        private void EndRound()
        {
            isTimerRunning = false;

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
            currentRound++;
            GameManager.Instance.StartRound();
        }
    }
}
