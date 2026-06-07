using UnityEngine;
using System.Collections;

namespace LightNShadowSurvivor
{
    public class LightBoost : ItemBase
    {
        [SerializeField] private float damageMultiplier = 2f;
        [SerializeField] private float rangeMultiplier = 1.5f;
        [SerializeField] private float duration = 8f;

        protected override void OnCollect()
        {
            var attack = player.GetComponentInChildren<FlashLightAttack>();
            if (attack != null)
            {
                attack.StartCoroutine(LightBoostRoutine(attack));
            }
        }

        private IEnumerator LightBoostRoutine(FlashLightAttack attack)
        {
            FlashLightAttack.FlashlightStats originalStats = attack.CaptureStats();
            attack.ApplyMultipliers(damageMultiplier, rangeMultiplier);
            
            // Visual feedback: change light color or intensity
            Light lightComponent = attack.LightComponent;
            if (lightComponent != null)
            {
                Color originalColor = lightComponent.color;
                lightComponent.color = Color.yellow;
                lightComponent.intensity *= 1.5f;

                yield return new WaitForSeconds(duration);

                lightComponent.color = originalColor;
                lightComponent.intensity /= 1.5f;
            }
            else
            {
                yield return new WaitForSeconds(duration);
            }

            attack.RestoreStats(originalStats);
        }
    }
}
