/*
2026-05-16 AI-Tag
This was created with the help of Assistant, a Unity Artificial Intelligence product.
*/
using UnityEngine;
using UnityEngine.InputSystem;

namespace LightNShadowSurvivor
{
    public class FlashLightAttack : MonoBehaviour
    {
        private const float InitialDamagePerSecond = 1f;
        private const float InitialRange = 10f;

        [Header("Light Attack")]
        [SerializeField] private bool requireFireInput = false;
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private int maxTargets = 64;

        [SerializeField] private float damagePerSecond = 1f;
        [SerializeField] private float range = 10f;
        [SerializeField] private float angle = 30f;
        [SerializeField] private LayerMask targetLayer;

        [SerializeField] private Light lightComponent;
        private Collider[] hitBuffer;

        public float DamagePerSecond => damagePerSecond;
        public float Range => range;
        public float Angle => angle;
        public Light LightComponent => lightComponent;

        private void Awake()
        {
            damagePerSecond = InitialDamagePerSecond;
            range = InitialRange;
            hitBuffer = new Collider[Mathf.Max(1, maxTargets)];
        }

        private void Start()
        {
            Setup();
        }

        private void Setup()
        {
            if (lightComponent == null) lightComponent = GetComponent<Light>();
            if (lightComponent == null) lightComponent = GetComponentInChildren<Light>();

            int enemyLayer = LayerMask.NameToLayer("Enemy");
            targetLayer = enemyLayer != -1 ? 1 << enemyLayer : ~0;

            if (lightComponent != null)
            {
                lightComponent.enabled = true;
                lightComponent.range = range;
                lightComponent.spotAngle = angle;
            }
        }

        private void Update()
        {
            if (lightComponent == null)
            {
                lightComponent = GetComponentInChildren<Light>();
                if (lightComponent == null) return;
            }

            bool isActive = !requireFireInput || Mouse.current == null || Mouse.current.leftButton.isPressed;
            lightComponent.enabled = isActive;
            lightComponent.range = range;
            lightComponent.spotAngle = angle;

            if (!isActive) return;

            Vector3 attackPos = lightComponent.transform.position;
            Vector3 forward2D = Vector3.ProjectOnPlane(lightComponent.transform.forward, Vector3.up).normalized;
            if (forward2D.sqrMagnitude < 0.01f)
            {
                forward2D = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            }

            Debug.DrawRay(attackPos, forward2D * range, Color.green);

            int hitCount = Physics.OverlapSphereNonAlloc(attackPos, range, hitBuffer, targetLayer, QueryTriggerInteraction.Collide);
            for (int i = 0; i < hitCount; i++)
            {
                Collider col = hitBuffer[i];
                if (col == null) continue;

                Vector3 toTarget = col.transform.position - attackPos;
                float distance = toTarget.magnitude;
                if (distance <= 0.001f) continue;

                Vector3 toTarget2D = Vector3.ProjectOnPlane(toTarget, Vector3.up).normalized;
                if (Vector3.Angle(forward2D, toTarget2D) > angle * 0.5f) continue;

                if (obstacleLayer.value != 0 && Physics.Raycast(attackPos, toTarget.normalized, distance, obstacleLayer, QueryTriggerInteraction.Ignore))
                {
                    continue;
                }

                var receiver = col.GetComponent<LightDamageReceiver>();
                if (receiver == null) receiver = col.GetComponentInParent<LightDamageReceiver>();

                if (receiver != null)
                {
                    receiver.TakeLightDamage(damagePerSecond * Time.deltaTime);
                }
            }
        }

        public void IncreaseDamage(float amount)
        {
            damagePerSecond = Mathf.Max(0f, damagePerSecond + amount);
        }

        public void IncreaseDiameterPercent(float percent)
        {
            range *= 1f + Mathf.Max(0f, percent);
        }

        public FlashlightStats CaptureStats()
        {
            return new FlashlightStats(damagePerSecond, range);
        }

        public void ApplyMultipliers(float damageMultiplier, float rangeMultiplier)
        {
            damagePerSecond *= Mathf.Max(0f, damageMultiplier);
            range *= Mathf.Max(0f, rangeMultiplier);
        }

        public void RestoreStats(FlashlightStats stats)
        {
            damagePerSecond = Mathf.Max(0f, stats.DamagePerSecond);
            range = Mathf.Max(0f, stats.Range);
        }

        public readonly struct FlashlightStats
        {
            public readonly float DamagePerSecond;
            public readonly float Range;

            public FlashlightStats(float damagePerSecond, float range)
            {
                DamagePerSecond = damagePerSecond;
                Range = range;
            }
        }
    }
}
