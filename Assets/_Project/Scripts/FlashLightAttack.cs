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
        public float damagePerSecond = 5000.0f; // Extremely high for near-instant kill
        public float range = 40f; 
        public float angle = 140f; // Very wide angle
        public LayerMask targetLayer;

        public Light lightComponent;

        void Start()
        {
            Setup();
        }

        void Setup()
        {
            if (lightComponent == null) lightComponent = GetComponent<Light>();
            if (lightComponent == null) lightComponent = GetComponentInChildren<Light>();
            
            int enemyLayer = LayerMask.NameToLayer("Enemy");
            if (enemyLayer != -1) targetLayer = 1 << enemyLayer;
            else targetLayer = ~0;
            
            if (lightComponent != null)
            {
                lightComponent.enabled = true;
                lightComponent.range = range;
                lightComponent.spotAngle = angle;
            }
        }

        void Update()
        {
            HandleInput();

            // Always ensure setup is valid
            if (targetLayer == 0) Setup();

            if (lightComponent == null || !lightComponent.enabled) return;

            Vector3 attackPos = lightComponent.transform.position;
            Vector3 attackDir = lightComponent.transform.forward;

            if (attackDir == Vector3.zero) attackDir = transform.forward;

            Debug.DrawRay(attackPos, attackDir * range, Color.yellow);

            // Using standard OverlapSphere for debugging certainty
            Collider[] hits = Physics.OverlapSphere(attackPos, range, targetLayer, QueryTriggerInteraction.Collide);

            if (hits.Length > 0 && Time.frameCount % 60 == 0) 
                Debug.Log($"[FlashLightAttack] OverlapSphere found {hits.Length} enemies in range.");

            foreach (var col in hits)
            {
                if (col == null) continue;

                Vector3 targetPoint = col.bounds.center;
                Vector3 dirToTarget = (targetPoint - attackPos).normalized;
                float angleToTarget = Vector3.Angle(attackDir, dirToTarget);
                
                // Root-based angle check as backup
                float angleToRoot = Vector3.Angle(attackDir, (col.transform.position - attackPos).normalized);
                float minAngle = Mathf.Min(angleToTarget, angleToRoot);
                
                if (minAngle < angle * 0.5f)
                {
                    var receiver = col.GetComponent<LightDamageReceiver>();
                    if (receiver == null) receiver = col.GetComponentInParent<LightDamageReceiver>();
                    
                    if (receiver != null)
                    {
                        receiver.TakeLightDamage(damagePerSecond * Time.deltaTime);
                        if (Time.frameCount % 15 == 0) 
                            Debug.Log($"[FlashLightAttack] DAMAGING {col.name} (Angle: {minAngle:F1})");
                    }
                    else if (Time.frameCount % 60 == 0)
                    {
                        Debug.LogWarning($"[FlashLightAttack] {col.name} has no LightDamageReceiver!");
                    }
                }
            }
        }

        private void HandleInput()
        {
            if (UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (lightComponent != null)
                {
                    lightComponent.enabled = !lightComponent.enabled;
                }
            }
        }
    }
}

