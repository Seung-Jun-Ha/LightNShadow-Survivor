using UnityEngine;
using System;

namespace LightNShadowSurvivor
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        private float currentHealth;

        public void IncreaseMaxHealth(float amount)
        {
            maxHealth += amount;
            currentHealth += amount; // Also heal for that amount
            OnHealthChanged?.Invoke(currentHealth);
        }

        public event Action<float> OnHealthChanged;
        public event Action OnPlayerDeath;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(float amount)
        {
            currentHealth -= amount;
            currentHealth = Mathf.Max(currentHealth, 0);
            
            OnHealthChanged?.Invoke(currentHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            currentHealth += amount;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            OnHealthChanged?.Invoke(currentHealth);
        }

        private void Die()
        {
            Debug.Log("Player Died!");
            OnPlayerDeath?.Invoke();
            if (GameManager.Instance != null) GameManager.Instance.TriggerGameOver();
        }
    }
}

