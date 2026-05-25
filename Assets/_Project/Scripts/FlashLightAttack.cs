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
        public float damagePerSecond = 85.0f; // ~1.2s to kill 100 HP monster
        public float range = 20f; 
        public float angle = 60f; 
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
            }
        }

        void Update()
        {
            // HandleInput(); // Temporarily disabled to ensure light stays ON

            if (lightComponent == null)
            {
                lightComponent = GetComponentInChildren<Light>();
                if (lightComponent == null) return;
            }
            
            // Force light ON for debug
            lightComponent.enabled = true;
            lightComponent.range = range;
            lightComponent.spotAngle = angle;

            Vector3 attackPos = lightComponent.transform.position;
            // Use player forward if light direction is zero or too vertical
            Vector3 attackDir = lightComponent.transform.forward;
            Vector3 forward2D = Vector3.ProjectOnPlane(attackDir, Vector3.up).normalized;
            if (forward2D.magnitude < 0.1f) forward2D = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

            Debug.DrawRay(attackPos, forward2D * range, Color.green);

            // Broad detection
            Collider[] hits = Physics.OverlapSphere(attackPos, range, targetLayer, QueryTriggerInteraction.Collide);

            foreach (var col in hits)
            {
                if (col == null) continue;

                // Horizontal direction to target
                Vector3 toTarget = col.transform.position - attackPos;
                Vector3 toTarget2D = Vector3.ProjectOnPlane(toTarget, Vector3.up).normalized;
                
                float angleToTarget = Vector3.Angle(forward2D, toTarget2D);
                
                // Be very generous with the angle (using provided angle variable)
                if (angleToTarget < angle * 0.5f)
                {
                    var receiver = col.GetComponent<LightDamageReceiver>();
                    if (receiver == null) receiver = col.GetComponentInParent<LightDamageReceiver>();
                    
                    if (receiver != null)
                    {
                        // Use high damage for testing
                        receiver.TakeLightDamage(1000f * Time.deltaTime);
                        if (Time.frameCount % 10 == 0) 
                            Debug.Log($"[FlashLightAttack] HIT: {col.name}, Angle2D: {angleToTarget:F1}");
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

