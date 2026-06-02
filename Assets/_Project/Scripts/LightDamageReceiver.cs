/*
2026-05-16 AI-Tag
This was created with the help of Assistant, a Unity Artificial Intelligence product.
*/
using UnityEngine;

namespace LightNShadowSurvivor
{
    public class LightDamageReceiver : MonoBehaviour
    {
        [SerializeField] private GameObject hitVfxPrefab;
        [SerializeField] private float vfxInterval = 0.1f;
        [SerializeField] private float hitEffectDuration = 0.15f;

        private MonsterBase monsterBase;
        private GhostAI ai;
        private float vfxTimer;
        private Renderer[] renderers;
        private Color[] originalColors;
        private float hitEffectTimer;
        private bool isBeingHit;

        private void Awake()
        {
            monsterBase = GetComponent<MonsterBase>();
            if (monsterBase == null) monsterBase = GetComponentInParent<MonsterBase>();

            ai = GetComponent<GhostAI>();
            if (ai == null) ai = GetComponentInParent<GhostAI>();

            renderers = GetComponentsInChildren<Renderer>();
            originalColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer r = renderers[i];
                if (r == null || r.material == null)
                {
                    originalColors[i] = Color.white;
                }
                else if (r.material.HasProperty("_BaseColor"))
                {
                    originalColors[i] = r.material.GetColor("_BaseColor");
                }
                else if (r.material.HasProperty("_Color"))
                {
                    originalColors[i] = r.material.GetColor("_Color");
                }
                else
                {
                    originalColors[i] = Color.white;
                }
            }
        }

        private void Update()
        {
            if (!isBeingHit) return;

            hitEffectTimer -= Time.deltaTime;
            if (hitEffectTimer <= 0f)
            {
                ResetColors();
                isBeingHit = false;
            }
        }

        public void TakeLightDamage(float damage)
        {
            if (monsterBase == null)
            {
                monsterBase = GetComponent<MonsterBase>();
                if (monsterBase == null) monsterBase = GetComponentInParent<MonsterBase>();
            }

            if (monsterBase == null || monsterBase.IsDead) return;

            LightReaction reaction = GetReaction(monsterBase.ReactionType);
            monsterBase.ModifyHealth(-(damage * reaction.DamageMultiplier));

            if (ai == null)
            {
                ai = GetComponent<GhostAI>();
                if (ai == null) ai = GetComponentInParent<GhostAI>();
            }

            if (ai != null)
            {
                ai.ApplySlow(reaction.SpeedMultiplier, reaction.SlowDuration);
            }

            isBeingHit = true;
            hitEffectTimer = hitEffectDuration;
            SetHitColor(reaction.HitColor);
            TrySpawnVfx();
        }

        private static LightReaction GetReaction(GhostReactionType type)
        {
            switch (type)
            {
                case GhostReactionType.Weak:
                    return new LightReaction(1.5f, 0.05f, 0.3f, new Color(1f, 0.85f, 0.2f, 1f));
                case GhostReactionType.Fast:
                    return new LightReaction(1f, 0.2f, 0.25f, new Color(0.3f, 0.8f, 1f, 1f));
                case GhostReactionType.Tank:
                    return new LightReaction(0.6f, 0.7f, 0.2f, new Color(1f, 0.45f, 0.25f, 1f));
                case GhostReactionType.Shield:
                    return new LightReaction(1f, 0.45f, 0.25f, new Color(0.25f, 0.65f, 1f, 1f));
                case GhostReactionType.Boss:
                    return new LightReaction(0.8f, 0.6f, 0.2f, new Color(1f, 0.2f, 0.8f, 1f));
                default:
                    return new LightReaction(1f, 0.5f, 0.2f, new Color(1f, 0.2f, 0.2f, 1f));
            }
        }

        private void TrySpawnVfx()
        {
            vfxTimer -= Time.deltaTime;
            if (vfxTimer > 0f || hitVfxPrefab == null) return;

            Instantiate(hitVfxPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            vfxTimer = vfxInterval;
        }

        private void ResetColors()
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer r = renderers[i];
                if (r == null || r.material == null) continue;

                if (r.material.HasProperty("_BaseColor")) r.material.SetColor("_BaseColor", originalColors[i]);
                else if (r.material.HasProperty("_Color")) r.material.SetColor("_Color", originalColors[i]);
            }
        }

        private void SetHitColor(Color hitColor)
        {
            foreach (Renderer r in renderers)
            {
                if (r == null || r.material == null) continue;

                if (r.material.HasProperty("_BaseColor")) r.material.SetColor("_BaseColor", hitColor);
                else if (r.material.HasProperty("_Color")) r.material.SetColor("_Color", hitColor);
            }
        }

        private readonly struct LightReaction
        {
            public readonly float DamageMultiplier;
            public readonly float SpeedMultiplier;
            public readonly float SlowDuration;
            public readonly Color HitColor;

            public LightReaction(float damageMultiplier, float speedMultiplier, float slowDuration, Color hitColor)
            {
                DamageMultiplier = damageMultiplier;
                SpeedMultiplier = speedMultiplier;
                SlowDuration = slowDuration;
                HitColor = hitColor;
            }
        }
    }
}
