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
        [SerializeField] private int maxLevel = 5;
        [SerializeField] private float[] xpRequirements = { 10f, 15f, 20f, 25f, 30f };
        [SerializeField] private float xpToNextLevel = 10f;

        public event Action<int> OnLevelUp;
        public event Action<float, float> OnXPChanged;

        public int CurrentLevel => currentLevel;
        public int MaxLevel => maxLevel;
        public float CurrentXP => currentXP;
        public float XPToNextLevel => xpToNextLevel;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            currentLevel = Mathf.Clamp(currentLevel, 1, maxLevel);
            xpToNextLevel = GetXPRequirement(currentLevel);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void AddXP(float amount)
        {
            if (amount <= 0f) return;

            currentXP += amount;

            while (currentLevel < maxLevel && currentXP >= xpToNextLevel)
            {
                LevelUp();
            }

            if (currentLevel >= maxLevel)
            {
                currentXP = Mathf.Min(currentXP, xpToNextLevel);
            }

            OnXPChanged?.Invoke(currentXP, xpToNextLevel);
        }

        private void LevelUp()
        {
            currentXP -= xpToNextLevel;
            currentLevel++;
            xpToNextLevel = GetXPRequirement(currentLevel);

            Debug.Log($"Level Up! Current Level: {currentLevel}");
            OnLevelUp?.Invoke(currentLevel);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OpenUpgrade();
            }
        }

        private float GetXPRequirement(int level)
        {
            if (xpRequirements == null || xpRequirements.Length == 0)
            {
                return 10f;
            }

            int index = Mathf.Clamp(level - 1, 0, xpRequirements.Length - 1);
            return Mathf.Max(1f, xpRequirements[index]);
        }
    }
}
