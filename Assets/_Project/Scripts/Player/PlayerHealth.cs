using UnityEngine;
using System;

namespace LightNShadowSurvivor
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField, Range(0f, 0.9f)] private float damageReduction;
        private float currentHealth;
        private bool isDead;

        public void IncreaseDurability(float reductionPercent)
        {
            if (reductionPercent <= 0f) return;
            damageReduction = 1f - ((1f - damageReduction) * (1f - Mathf.Clamp01(reductionPercent)));
        }

        public event Action<float> OnHealthChanged;
        public event Action OnPlayerDeath;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public float DamageReduction => damageReduction;
        public bool IsDead => isDead;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(float amount)
        {
            if (isDead || amount <= 0f) return;

            currentHealth -= amount * (1f - damageReduction);
            currentHealth = Mathf.Max(currentHealth, 0);
            
            OnHealthChanged?.Invoke(currentHealth);

            if (AudioManager.Instance != null && currentHealth > 0)
            {
                AudioManager.Instance.PlayPlayerHitSFX();
            }

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

        public void RestoreFullHealth()
        {
            isDead = false;
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth);
        }

        private void Die()
        {
            if (isDead) return;

            isDead = true;
            Debug.Log("Player Died!");

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPlayerDeathSFX();
            }

            OnPlayerDeath?.Invoke();

            if (GetComponent<PlayerDeathHandler>() == null && GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameOver();
            }
        }
    }
}

