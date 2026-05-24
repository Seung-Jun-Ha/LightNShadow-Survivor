using UnityEngine;

namespace LightNShadowSurvivor
{
    public class MonsterBase : MonoBehaviour
    {
        [SerializeField] protected float maxHealth = 100f;
        protected float currentHealth;

        protected virtual void Awake()
        {
            currentHealth = maxHealth;
        }

        public virtual void ModifyHealth(float amount)
        {
            currentHealth += amount;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            Destroy(gameObject);
        }
    }
}
