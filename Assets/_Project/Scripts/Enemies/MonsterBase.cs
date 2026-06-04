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
        [SerializeField] protected float experienceReward = 25f;
        [SerializeField] private bool grantExperienceDirectly = true;

        protected float currentHealth;
        protected float currentShield;
        protected bool isDead = false;

        public event Action OnDeath;

        public GhostReactionType ReactionType => reactionType;
        public bool IsDead => isDead;
        public float CurrentHealth => currentHealth;
        public float CurrentShield => currentShield;

        protected virtual void Awake()
        {
            currentHealth = maxHealth;
            currentShield = shieldHealth;
        }

        public virtual void ModifyHealth(float amount)
        {
            if (isDead) return;

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
                Die();
            }
        }

        protected virtual void Die()
        {
            if (isDead) return;
            isDead = true;
            if (grantExperienceDirectly && !TryGetComponent<MonsterDeathHandler>(out _))
            {
                GrantExperience();
            }

            OnDeath?.Invoke();
            Destroy(gameObject, 5f);
        }
        public virtual void StopBehavior()
        {
            if (TryGetComponent(out GhostAI ai)) ai.StopAI();
            if (TryGetComponent(out UnityEngine.AI.NavMeshAgent agent) && agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }
        }

        private void GrantExperience()
        {
            if (experienceReward <= 0f || PlayerExperience.Instance == null) return;
            PlayerExperience.Instance.AddXP(experienceReward);
        }
    }
}

