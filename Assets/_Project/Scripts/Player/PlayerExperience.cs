using UnityEngine;
using System;

namespace LightNShadowSurvivor
{
    public class PlayerExperience : MonoBehaviour
    {
        public static PlayerExperience Instance { get; private set; }

        [Header("Experience Settings")]
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private float currentXP = 0f;
        [SerializeField] private float xpToNextLevel = 100f;
        [SerializeField] private float xpMultiplier = 1.2f;

        public event Action<int> OnLevelUp;
        public event Action<float, float> OnXPChanged;

        public int CurrentLevel => currentLevel;
        public float CurrentXP => currentXP;
        public float XPToNextLevel => xpToNextLevel;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void AddXP(float amount)
        {
            currentXP += amount;
            
            while (currentXP >= xpToNextLevel)
            {
                LevelUp();
            }

            OnXPChanged?.Invoke(currentXP, xpToNextLevel);
        }

        private void LevelUp()
        {
            currentXP -= xpToNextLevel;
            currentLevel++;
            xpToNextLevel = Mathf.Round(xpToNextLevel * xpMultiplier);
            
            Debug.Log($"Level Up! Current Level: {currentLevel}");
            OnLevelUp?.Invoke(currentLevel);
            
            // In a real roguelike survivor game, level up triggers an upgrade.
            // But the scenario says "End of round OR level up".
            // We can trigger an immediate upgrade or just wait until the end of the round.
            // Let's stick to the scenario: "Level up or round end -> Upgrade selection".
            GameManager.Instance.OpenUpgrade();
        }
    }
}
