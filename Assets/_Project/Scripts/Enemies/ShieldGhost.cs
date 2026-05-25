using UnityEngine;

namespace LightNShadowSurvivor
{
    public class ShieldGhost : MonsterBase
    {
        [Header("Shield Settings")]
        [SerializeField] private GameObject shieldVisual;

        protected override void Awake()
        {
            base.Awake();
            if (shieldVisual != null) shieldVisual.SetActive(currentShield > 0);
        }

        public override void ModifyHealth(float amount)
        {
            if (isDead) return;

            base.ModifyHealth(amount);

            if (currentShield <= 0 && shieldVisual != null && shieldVisual.activeSelf)
            {
                shieldVisual.SetActive(false);
                Debug.Log($"[ShieldGhost] {gameObject.name} shield broken!");
            }
        }
    }
}
