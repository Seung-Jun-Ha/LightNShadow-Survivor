/*
2026-05-16 AI-Tag
This was created with the help of Assistant, a Unity Artificial Intelligence product.
*/
using UnityEngine;
using System.Collections.Generic;

using UnityEngine;
using System.Collections.Generic;

namespace LightNShadowSurvivor
{
    public class FlashLightAttack : MonoBehaviour
    {
        public float damagePerSecond = 100.0f;
        public float range = 25f;
        public float angle = 90f;
        public LayerMask targetLayer;

        private Light lightComponent;

        void Awake()
        {
            lightComponent = GetComponent<Light>();
            if (lightComponent == null) lightComponent = GetComponentInChildren<Light>();
            targetLayer = 1 << LayerMask.NameToLayer("Enemy");
        }

        void Update()
        {
            Vector3 attackDir = lightComponent != null ? lightComponent.transform.forward : transform.forward;
            Vector3 attackPos = lightComponent != null ? lightComponent.transform.position : transform.position;

            Collider[] targets = Physics.OverlapSphere(attackPos, range, targetLayer);

            foreach (var col in targets)
            {
                Vector3 targetPoint = col.bounds.center;
                Vector3 dirToTarget = (targetPoint - attackPos).normalized;
                
                if (Vector3.Angle(attackDir, dirToTarget) < angle * 0.5f)
                {
                    var receiver = col.GetComponent<LightDamageReceiver>();
                    if (receiver == null) receiver = col.GetComponentInParent<LightDamageReceiver>();
                    
                    if (receiver != null)
                    {
                        receiver.TakeLightDamage(damagePerSecond * Time.deltaTime);
                    }
                }
            }
        }
    }
}

