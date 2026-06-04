using UnityEngine;
using System;

namespace LightNShadowSurvivor
{
    public class GameStatsManager : MonoBehaviour
    {
        public static GameStatsManager Instance { get; private set; }

        public int Kills { get; private set; }
        public float TimeSurvived { get; private set; }
        public int LevelReached => PlayerExperience.Instance != null ? PlayerExperience.Instance.CurrentLevel : 1;

        public event Action OnStatsUpdated;

        private bool isTracking = false;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged += HandleStateChanged;
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged -= HandleStateChanged;

            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.Round) isTracking = true;
            else if (state == GameState.Ending || state == GameState.GameOver) isTracking = false;
        }

        private void Update()
        {
            if (isTracking)
            {
                TimeSurvived += Time.deltaTime;
            }
        }

        public void AddKill()
        {
            Kills++;
            OnStatsUpdated?.Invoke();
        }

        public void ResetStats()
        {
            Kills = 0;
            TimeSurvived = 0;
            OnStatsUpdated?.Invoke();
        }
    }
}
