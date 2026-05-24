using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace LightNShadowSurvivor
{
    public class MonsterDeathHandler : MonoBehaviour
    {
        [SerializeField] private float fadeDuration = 2f;
        [SerializeField] private string dissolveParameter = "_Dissolve";
        
        private MonsterBase monsterBase;
        private GhostAI ai;
        private NavMeshAgent agent;
        private Animator animator;
        private Renderer[] renderers;

        private static readonly int DissolveState = Animator.StringToHash("dissolve");

        private void Awake()
        {
            monsterBase = GetComponent<MonsterBase>();
            ai = GetComponent<GhostAI>();
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();
            renderers = GetComponentsInChildren<Renderer>();
        }

        private void OnEnable()
        {
            if (monsterBase != null) monsterBase.OnDeath += HandleDeath;
        }

        private void OnDisable()
        {
            if (monsterBase != null) monsterBase.OnDeath -= HandleDeath;
        }

        private void HandleDeath()
        {
            StartCoroutine(DeathRoutine());
        }

        private IEnumerator DeathRoutine()
        {
            // 1. Disable AI and Colliders
            if (agent != null) agent.enabled = false;
            if (ai != null) ai.enabled = false;
            
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            // 2. Play Death Animation
            if (animator != null)
            {
                // Try to play 'dissolve' or 'death' state
                if (HasState(animator, "dissolve"))
                {
                    animator.CrossFade(DissolveState, 0.2f);
                }
                else
                {
                    // If no death animation, just rotate to simulate falling
                    StartCoroutine(RotateToFall());
                }
            }

            // 3. Fade Out Visuals
            float elapsed = 0f;
            Vector3 startScale = transform.localScale;
            
            // Collect all materials to fade
            Material[] materials = GetAllMaterials();

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / fadeDuration;
                float alpha = 1f - progress;

                // Scale down as fallback
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, progress);

                foreach (var mat in materials)
                {
                    if (mat.HasProperty(dissolveParameter))
                    {
                        mat.SetFloat(dissolveParameter, alpha);
                    }
                    else if (mat.HasProperty("_Color"))
                    {
                        Color c = mat.color;
                        c.a = alpha;
                        mat.color = c;
                    }
                }
                yield return null;
            }

            // 4. Cleanup
            Destroy(gameObject);
        }

        private Material[] GetAllMaterials()
        {
            var mats = new System.Collections.Generic.List<Material>();
            foreach (var r in renderers)
            {
                // Create instances of materials so we don't modify the assets
                mats.AddRange(r.materials);
            }
            return mats.ToArray();
        }

        private bool HasState(Animator anim, string stateName)
        {
            // This is a bit tricky without checking controller specifically, 
            // but we can rely on Animator.HasState or just try catch or just check known names
            // For simplicity, we know 'dissolve' exists for ghost_free
            return animator.HasState(0, Animator.StringToHash(stateName));
        }

        private IEnumerator RotateToFall()
        {
            float duration = 0.5f;
            float elapsed = 0f;
            Quaternion startRot = transform.rotation;
            Quaternion endRot = startRot * Quaternion.Euler(80, 0, 0); // Fall forward/side

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.rotation = Quaternion.Slerp(startRot, endRot, elapsed / duration);
                yield return null;
            }
        }
    }
}
