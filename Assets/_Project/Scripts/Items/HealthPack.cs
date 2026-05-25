using UnityEngine;

namespace LightNShadowSurvivor
{
    public class HealthPack : ItemBase
    {
        [SerializeField] private float healAmount = 20f;

        protected override void OnCollect()
        {
            var health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.Heal(healAmount);
                Debug.Log($"[HealthPack] Player healed by {healAmount}");
            }
        }
    }
}
