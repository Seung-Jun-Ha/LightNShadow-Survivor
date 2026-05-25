using UnityEngine;
using System.Collections;

namespace LightNShadowSurvivor
{
    public class PlayerDeathHandler : MonoBehaviour
    {
        [SerializeField] private float dissolveDuration = 3f;
        [SerializeField] private Material dissolveMaterialBase;
        
        private PlayerHealth playerHealth;
        private Renderer[] renderers;

        private void Awake()
        {
            playerHealth = GetComponent<PlayerHealth>();
            renderers = GetComponentsInChildren<Renderer>();
        }

        private void OnEnable()
        {
            if (playerHealth != null) playerHealth.OnPlayerDeath += HandleDeath;
        }

        private void OnDisable()
        {
            if (playerHealth != null) playerHealth.OnPlayerDeath -= HandleDeath;
        }

        private void HandleDeath()
        {
            StartCoroutine(DeathRoutine());
        }

        private IEnumerator DeathRoutine()
        {
            // 1. Disable controls
            var pc = GetComponent<PlayerController>();
            if (pc != null) pc.enabled = false;
            
            var pa = GetComponent<PlayerAim>();
            if (pa != null) pa.enabled = false;

            // 2. Switch materials to dissolve
            var dissolveMaterials = new System.Collections.Generic.List<Material>();
            foreach (var r in renderers)
            {
                Material[] newMats = new Material[r.sharedMaterials.Length];
                for (int i = 0; i < r.sharedMaterials.Length; i++)
                {
                    Material oldMat = r.sharedMaterials[i];
                    Material newMat = new Material(dissolveMaterialBase);
                    if (oldMat != null)
                    {
                        if (oldMat.HasProperty("_MainTex")) newMat.SetTexture("_BaseMap", oldMat.GetTexture("_MainTex"));
                        else if (oldMat.HasProperty("_BaseMap")) newMat.SetTexture("_BaseMap", oldMat.GetTexture("_BaseMap"));
                        
                        if (oldMat.HasProperty("_Color")) newMat.SetColor("_Color", oldMat.GetColor("_Color"));
                        else if (oldMat.HasProperty("_BaseColor")) newMat.SetColor("_Color", oldMat.GetColor("_BaseColor"));
                    }
                    newMats[i] = newMat;
                    dissolveMaterials.Add(newMat);
                }
                r.materials = newMats;
            }

            // 3. Animate dissolve
            float elapsed = 0f;
            while (elapsed < dissolveDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = 1f - (elapsed / dissolveDuration); // From 1 to 0
                foreach (var mat in dissolveMaterials)
                {
                    mat.SetFloat("_Dissolve", t);
                }
                yield return null;
            }

            // 4. Trigger Game Over in GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameOver();
            }
        }
    }
}
