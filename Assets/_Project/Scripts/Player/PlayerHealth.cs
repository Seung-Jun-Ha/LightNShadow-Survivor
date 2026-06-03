using UnityEngine;
using System;

namespace LightNShadowSurvivor
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        private float currentHealth;
        private bool isDead;

        public void IncreaseMaxHealth(float amount)
        {
            if (amount <= 0f) return;

            maxHealth += amount;
            if (!isDead)
            {
                currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            }

            OnHealthChanged?.Invoke(currentHealth);
        }

        public event Action<float> OnHealthChanged;
        public event Action OnPlayerDeath;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsDead => isDead;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(float amount)
        {
            if (isDead || amount <= 0f) return;

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
            if (isDead || amount <= 0f) return;

            currentHealth += amount;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            OnHealthChanged?.Invoke(currentHealth);
        }

        private void Die()
        {
            if (isDead) return;

            isDead = true;
            Debug.Log("Player Died!");
            OnPlayerDeath?.Invoke();

            if (GetComponent<PlayerDeathHandler>() == null && GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameOver();
            }
        }
    }
}

