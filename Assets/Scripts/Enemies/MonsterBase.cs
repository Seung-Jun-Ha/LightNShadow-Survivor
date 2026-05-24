using UnityEngine;
using System;

namespace LightNShadowSurvivor
{
    public class MonsterBase : MonoBehaviour
    {
        [SerializeField] protected float maxHealth = 100f;
        protected float currentHealth;
        protected bool isDead = false;

        public event Action OnDeath;

        protected virtual void Awake()
        {
            currentHealth = maxHealth;
        }

        public virtual void ModifyHealth(float amount)
        {
            if (isDead) return;

            currentHealth += amount;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            if (isDead) return;
            isDead = true;
            OnDeath?.Invoke();
        }
    }
}
