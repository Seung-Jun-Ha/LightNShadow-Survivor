/*
2026-05-16 AI-Tag
This was created with the help of Assistant, a Unity Artificial Intelligence product.
*/
using UnityEngine;
using System.Collections.Generic;

namespace LightNShadowSurvivor
{
    public class FlashLightAttack : MonoBehaviour
    {
        public float damagePerSecond = 10f;
        public float range = 15f;
        public float angle = 45f; // Spot Light의 Outer Angle과 맞춤
        public LayerMask targetLayer;
        public LayerMask obstacleLayer;

        void Update()
        {
            DetectAndDamage();
        }

        private void DetectAndDamage()
        {
            // 1. 범위 내 모든 콜라이더 수집
            Collider[] targets = Physics.OverlapSphere(transform.position, range, targetLayer);

            foreach (var col in targets)
            {
                Vector3 dirToTarget = (col.transform.position - transform.position).normalized;
                float angleToTarget = Vector3.Angle(transform.forward, dirToTarget);

                // 2. 각도 검사
                if (angleToTarget < angle * 0.5f)
                {
                    // 3. 장애물(벽/나무) 검사
                    if (!Physics.Raycast(transform.position, dirToTarget, range, obstacleLayer))
                    {
                        if (col.TryGetComponent(out LightDamageReceiver receiver))
                        {
                            receiver.TakeLightDamage(damagePerSecond * Time.deltaTime);
                        }
                    }
                }
            }
        }
    }
}
