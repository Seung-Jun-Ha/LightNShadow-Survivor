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
        public float damagePerSecond = 66.7f;
        public float range = 15f;
        public float angle = 45f;
        public LayerMask targetLayer;
        public LayerMask obstacleLayer;

        [Header("Debug")]
        [SerializeField] private bool showGizmos = true;

        void Update()
        {
            DetectAndDamage();
        }

        private void DetectAndDamage()
        {
            // Use OverlapSphere to get potential targets
            Collider[] targets = Physics.OverlapSphere(transform.position, range, targetLayer);

            foreach (var col in targets)
            {
                // Get the center of the collider for better accuracy
                Vector3 targetPoint = col.bounds.center;
                Vector3 dirToTarget = (targetPoint - transform.position).normalized;
                float distToTarget = Vector3.Distance(transform.position, targetPoint);
                
                // 1. Distance check (already partially handled by OverlapSphere, but good for precise bounds)
                if (distToTarget > range) continue;

                // 2. Angle check
                float angleToTarget = Vector3.Angle(transform.forward, dirToTarget);
                if (angleToTarget < angle * 0.5f)
                {
                    // 3. Obstacle check
                    // We raycast to the target point. 
                    // Note: We use distToTarget - 0.1f to avoid hitting the ground behind the monster
                    if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleLayer))
                    {
                        if (col.TryGetComponent(out LightDamageReceiver receiver))
                        {
                            receiver.TakeLightDamage(damagePerSecond * Time.deltaTime);
                            
                            #if UNITY_EDITOR
                            Debug.DrawLine(transform.position, targetPoint, Color.red);
                            #endif
                        }
                        else
                        {
                            // Try parent in case script is on root but collider on child
                            var parentReceiver = col.GetComponentInParent<LightDamageReceiver>();
                            if (parentReceiver != null)
                            {
                                parentReceiver.TakeLightDamage(damagePerSecond * Time.deltaTime);
                                #if UNITY_EDITOR
                                Debug.DrawLine(transform.position, targetPoint, Color.red);
                                #endif
                            }
                        }
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (!showGizmos) return;
            
            Gizmos.color = new Color(1, 1, 0, 0.2f);
            Gizmos.DrawWireSphere(transform.position, range);
            
            // Draw cone
            Vector3 forward = transform.forward * range;
            Quaternion leftRayRotation = Quaternion.AngleAxis(-angle * 0.5f, transform.up);
            Quaternion rightRayRotation = Quaternion.AngleAxis(angle * 0.5f, transform.up);
            Vector3 leftRayDirection = leftRayRotation * forward;
            Vector3 rightRayDirection = rightRayRotation * forward;

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, leftRayDirection);
            Gizmos.DrawRay(transform.position, rightRayDirection);
            Gizmos.DrawLine(transform.position + leftRayDirection, transform.position + rightRayDirection);
        }
    }
}
