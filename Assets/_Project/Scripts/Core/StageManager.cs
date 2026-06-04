using System;
using UnityEngine;

namespace LightNShadowSurvivor
{
    public class StageManager : MonoBehaviour
    {
        [SerializeField] private int totalStages = 3;
        [SerializeField] private TimeOfDayManager timeOfDayManager;
        [SerializeField] private bool completeStageWhenRoundEnds = true;

        private int currentStage;
        private bool allStagesCleared;

        public event Action<int> OnStageCompleted;
        public event Action OnAllStagesCleared;

        public int CurrentStage => currentStage;
        public int TotalStages => totalStages;
        public bool AllStagesCleared => allStagesCleared;

        private void Start()
        {
            if (timeOfDayManager == null) timeOfDayManager = FindAnyObjectByType<TimeOfDayManager>();

            if (completeStageWhenRoundEnds && RoundManager.Instance != null)
            {
                RoundManager.Instance.OnRoundEnded += HandleRoundEnded;
            }
        }

        private void OnDestroy()
        {
            if (RoundManager.Instance != null)
            {
                RoundManager.Instance.OnRoundEnded -= HandleRoundEnded;
            }
        }

        public void CompleteStage()
        {
            if (allStagesCleared) return;

            currentStage = Mathf.Clamp(currentStage + 1, 0, Mathf.Max(1, totalStages));
            Debug.Log($"Stage {currentStage} cleared!");
            OnStageCompleted?.Invoke(currentStage);

            if (currentStage >= totalStages)
            {
                allStagesCleared = true;
                if (timeOfDayManager != null)
                {
                    timeOfDayManager.ClearAllStages();
                }

                Debug.Log("All stages cleared! Transitioning to morning...");
                OnAllStagesCleared?.Invoke();
            }
        }

        public void ResetProgress()
        {
            currentStage = 0;
            allStagesCleared = false;
        }

        private void HandleRoundEnded(int round)
        {
            CompleteStage();
        }
    }
}
