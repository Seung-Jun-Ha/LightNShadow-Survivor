using UnityEngine;
using System.Collections;

namespace LightNShadowSurvivor
{
    public class SpeedBoost : ItemBase
    {
        [SerializeField] private float boostAmount = 4f;
        [SerializeField] private float duration = 5f;

        protected override void OnCollect()
        {
            var controller = player.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.StartCoroutine(SpeedBoostRoutine(controller));
            }
        }

        private IEnumerator SpeedBoostRoutine(PlayerController controller)
        {
            float originalSpeed = controller.MoveSpeed;
            controller.MoveSpeed += boostAmount;
            Debug.Log($"[SpeedBoost] Speed increased to {controller.MoveSpeed}");

            yield return new WaitForSeconds(duration);

            controller.MoveSpeed -= boostAmount;
            Debug.Log($"[SpeedBoost] Speed returned to {controller.MoveSpeed}");
        }
    }
}
