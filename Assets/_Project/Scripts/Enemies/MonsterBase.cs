using UnityEngine;
using System;

namespace LightNShadowSurvivor
{
    public enum GhostReactionType
    {
        Normal,
        Weak,
        Fast,
        Tank,
        Teleport,
        Split,
        Shield,
        Boss
    }

    public class MonsterBase : MonoBehaviour
    {
        [Header("Base Stats")]
        [SerializeField] protected float maxHealth = 100f;
        [SerializeField] protected float shieldHealth = 0f;
        [SerializeField] protected GhostReactionType reactionType = GhostReactionType.Normal;

        protected float currentHealth;
        protected float currentShield;
        protected bool isDead = false;

        public event Action OnDeath;

        public GhostReactionType ReactionType => reactionType;

        protected virtual void Awake()
        {
            currentHealth = maxHealth;
            currentShield = shieldHealth;
        }

        public virtual void ModifyHealth(float amount)
        {
            if (isDead) return;

            Debug.Log($"[MonsterBase] {gameObject.name} modified by {amount}. Current HP: {currentHealth}");

            // Damage is negative amount
            if (amount < 0)
            {
                float damage = -amount;
                if (currentShield > 0)
                {
                    float shieldDamage = Mathf.Min(currentShield, damage);
                    currentShield -= shieldDamage;
                    damage -= shieldDamage;
                }

                if (damage > 0)
                {
                    currentHealth -= damage;
                }
            }
            else
            {
                currentHealth += amount;
            }

            if (currentHealth <= 0)
            {
                Debug.Log($"[MonsterBase] {gameObject.name} HP reached 0. Triggering Die().");
                Die();
            }
        }

        protected virtual void Die()
        {
            if (isDead) return;
            isDead = true;
            Debug.Log($"[MonsterBase] {gameObject.name} logic marked as DEAD. Invoking OnDeath.");
            OnDeath?.Invoke();
            
            // Fallback destroy if no handler takes over
            Destroy(gameObject, 5f);
        }
    }
}
