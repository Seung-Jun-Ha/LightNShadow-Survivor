/*
2026-05-16 AI-Tag
This was created with the help of Assistant, a Unity Artificial Intelligence product.
*/
using System;
using UnityEngine;

namespace LightNShadowSurvivor
{
    public class LightDamageReceiver : MonoBehaviour
    {
        private MonsterBase monsterBase;
        private GhostAI ai;
        [SerializeField] private GameObject hitVfxPrefab;
        private float vfxTimer = 0f;
        private float vfxInterval = 0.1f;

        private Renderer[] renderers;
        private Color[] originalColors;
        private float hitEffectTimer = 0f;
        private float hitEffectDuration = 0.15f;
        private bool isBeingHit = false;

        void Awake()
        {
            monsterBase = GetComponent<MonsterBase>();
            ai = GetComponent<GhostAI>();
            renderers = GetComponentsInChildren<Renderer>();
            
            // Store original colors
            var colorList = new System.Collections.Generic.List<Color>();
            foreach (var r in renderers)
            {
                if (r.material.HasProperty("_BaseColor")) colorList.Add(r.material.GetColor("_BaseColor"));
                else if (r.material.HasProperty("_Color")) colorList.Add(r.material.GetColor("_Color"));
                else colorList.Add(Color.white);
            }
            originalColors = colorList.ToArray();
        }

        void Update()
        {
            if (isBeingHit)
            {
                hitEffectTimer -= Time.deltaTime;
                if (hitEffectTimer <= 0)
                {
                    ResetColors();
                    isBeingHit = false;
                }
            }
        }

        private void ResetColors()
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i].material.HasProperty("_BaseColor")) renderers[i].material.SetColor("_BaseColor", originalColors[i]);
                else if (renderers[i].material.HasProperty("_Color")) renderers[i].material.SetColor("_Color", originalColors[i]);
            }
        }

        private void SetHitColor()
        {
            Color hitColor = new Color(1f, 0.2f, 0.2f, 1f); // Intense red
            foreach (var r in renderers)
            {
                if (r == null) continue;
                if (r.material.HasProperty("_BaseColor")) r.material.SetColor("_BaseColor", hitColor);
                else if (r.material.HasProperty("_Color")) r.material.SetColor("_Color", hitColor);
                else if (r.material.HasProperty("_BaseMap")) r.material.SetColor("_BaseColor", hitColor);
            }
        }

        public void TakeLightDamage(float damage)
        {
            // Find monster base if lost
            if (monsterBase == null)
            {
                monsterBase = GetComponent<MonsterBase>();
                if (monsterBase == null) monsterBase = GetComponentInParent<MonsterBase>();
            }

            if (monsterBase == null) return;

            isBeingHit = true;
            hitEffectTimer = hitEffectDuration;
            SetHitColor();

            monsterBase.ModifyHealth(-damage);
            
            if (ai == null) ai = GetComponent<GhostAI>();
            if (ai != null) ai.ApplySlow(); 

            // Spawn VFX
            vfxTimer -= Time.deltaTime;
            if (vfxTimer <= 0 && hitVfxPrefab != null)
            {
                Instantiate(hitVfxPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                vfxTimer = vfxInterval;
            }
        }
    }
}
