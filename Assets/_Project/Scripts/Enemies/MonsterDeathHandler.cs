using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace LightNShadowSurvivor
{
    public class MonsterDeathHandler : MonoBehaviour
    {
        [SerializeField] private float fadeDuration = 0.5f; // Shortened
        [SerializeField] private string dissolveParameter = "_Dissolve";
        [SerializeField] private Material dissolveMaterialBase;
        [SerializeField] private GameObject xpOrbPrefab;
        [SerializeField] private float xpValue = 20f;
        [SerializeField] private GameObject deathParticlePrefab; // New particle effect
        [Header("Item Drops")]
        [SerializeField] private GameObject[] itemPrefabs;
        [SerializeField] private float itemDropChance = 0.1f;
        
        private MonsterBase monsterBase;
        private GhostAI ai;
        private NavMeshAgent agent;
        private Animator animator;
        private Renderer[] renderers;
        private bool deathRoutineStarted;

        private static readonly int DissolveState = Animator.StringToHash("dissolve");

        private void Awake()
        {
            monsterBase = GetComponent<MonsterBase>();
            if (monsterBase == null) monsterBase = GetComponentInParent<MonsterBase>();
            
            ai = GetComponent<GhostAI>();
            if (ai == null) ai = GetComponentInParent<GhostAI>();
            
            agent = GetComponent<NavMeshAgent>();
            if (agent == null) agent = GetComponentInParent<NavMeshAgent>();
            
            animator = GetComponentInChildren<Animator>();
            renderers = GetComponentsInChildren<Renderer>();
        }

        private void OnEnable()
        {
            if (monsterBase == null)
            {
                monsterBase = GetComponent<MonsterBase>();
                if (monsterBase == null) monsterBase = GetComponentInParent<MonsterBase>();
            }

            if (monsterBase != null) monsterBase.OnDeath += HandleDeath;
        }

        private void OnDisable()
        {
            if (monsterBase != null) monsterBase.OnDeath -= HandleDeath;
        }

        private void HandleDeath()
        {
            if (deathRoutineStarted) return;
            deathRoutineStarted = true;

            if (GameStatsManager.Instance != null)
            {
                GameStatsManager.Instance.AddKill();
            }
            StartCoroutine(DeathRoutine());
        }

        private IEnumerator DeathRoutine()
        {
            if (agent != null && agent.enabled) agent.enabled = false;
            if (ai != null) ai.enabled = false;

            foreach (Collider col in GetComponentsInChildren<Collider>())
            {
                col.enabled = false;
            }

            if (deathParticlePrefab != null)
            {
                Instantiate(deathParticlePrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayMonsterDeathSFX();
            }

            GrantExperienceReward();

            // Drop Random Item
            if (itemPrefabs != null && itemPrefabs.Length > 0 && Random.value < itemDropChance)
            {
                GameObject itemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
                Instantiate(itemPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            }

            if (animator != null && HasState(animator, "dissolve"))
            {
                animator.CrossFade(DissolveState, 0.05f, 0, 0f);
            }

            yield return StartCoroutine(RotateToFall());
            yield return StartCoroutine(DissolveRoutine());

            Destroy(gameObject);
        }

        private void GrantExperienceReward()
        {
            if (xpValue <= 0f) return;

            if (xpOrbPrefab != null)
            {
                GameObject orb = Instantiate(xpOrbPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                if (orb.TryGetComponent(out ExperienceOrb xp))
                {
                    xp.SetXP(xpValue);
                    return;
                }

                Destroy(orb);
                Debug.LogWarning("[MonsterDeathHandler] XP orb prefab has no ExperienceOrb component. Granting XP directly.");
            }

            if (PlayerExperience.Instance != null)
            {
                PlayerExperience.Instance.AddXP(xpValue);
            }
        }

        public void ConfigureExperienceReward(float amount)
        {
            xpValue = Mathf.Max(0f, amount);
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
            return anim != null && anim.HasState(0, Animator.StringToHash(stateName));
        }

        private IEnumerator DissolveRoutine()
        {
            Material[] materials = PrepareDissolveMaterials();
            float duration = Mathf.Max(0.01f, fadeDuration);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float dissolve = Mathf.Clamp01(elapsed / duration);
                float alpha = 1f - dissolve;

                foreach (Material mat in materials)
                {
                    if (mat == null) continue;
                    if (mat.HasProperty(dissolveParameter)) mat.SetFloat(dissolveParameter, dissolve);
                    if (mat.HasProperty("_BaseColor"))
                    {
                        Color color = mat.GetColor("_BaseColor");
                        color.a = alpha;
                        mat.SetColor("_BaseColor", color);
                    }
                    else if (mat.HasProperty("_Color"))
                    {
                        Color color = mat.GetColor("_Color");
                        color.a = alpha;
                        mat.SetColor("_Color", color);
                    }
                }

                yield return null;
            }
        }

        private Material[] PrepareDissolveMaterials()
        {
            if (dissolveMaterialBase == null) return GetAllMaterials();

            var mats = new System.Collections.Generic.List<Material>();
            foreach (Renderer r in renderers)
            {
                if (r == null) continue;

                Material[] newMats = new Material[r.sharedMaterials.Length];
                for (int i = 0; i < r.sharedMaterials.Length; i++)
                {
                    Material oldMat = r.sharedMaterials[i];
                    Material newMat = new Material(dissolveMaterialBase);
                    if (oldMat != null)
                    {
                        if (oldMat.HasProperty("_MainTex") && newMat.HasProperty("_BaseMap")) newMat.SetTexture("_BaseMap", oldMat.GetTexture("_MainTex"));
                        else if (oldMat.HasProperty("_BaseMap") && newMat.HasProperty("_BaseMap")) newMat.SetTexture("_BaseMap", oldMat.GetTexture("_BaseMap"));

                        if (oldMat.HasProperty("_Color") && newMat.HasProperty("_Color")) newMat.SetColor("_Color", oldMat.GetColor("_Color"));
                        else if (oldMat.HasProperty("_BaseColor") && newMat.HasProperty("_BaseColor")) newMat.SetColor("_BaseColor", oldMat.GetColor("_BaseColor"));
                    }

                    newMats[i] = newMat;
                    mats.Add(newMat);
                }

                r.materials = newMats;
            }

            return mats.ToArray();
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
