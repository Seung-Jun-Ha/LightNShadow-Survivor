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
            float originalDmg = attack.damagePerSecond;
            float originalRange = attack.range;
            
            attack.damagePerSecond *= damageMultiplier;
            attack.range *= rangeMultiplier;
            
            // Visual feedback: change light color or intensity
            if (attack.lightComponent != null)
            {
                Color originalColor = attack.lightComponent.color;
                attack.lightComponent.color = Color.yellow;
                attack.lightComponent.intensity *= 1.5f;

                yield return new WaitForSeconds(duration);

                attack.lightComponent.color = originalColor;
                attack.lightComponent.intensity /= 1.5f;
            }
            else
            {
                yield return new WaitForSeconds(duration);
            }

            attack.damagePerSecond = originalDmg;
            attack.range = originalRange;
        }
    }
}
