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
            if (GameStatsManager.Instance != null)
            {
                GameStatsManager.Instance.AddKill();
            }
            StartCoroutine(DeathRoutine());
        }

        private IEnumerator DeathRoutine()
        {
            // 1. Disable AI and Colliders
            if (agent != null) agent.enabled = false;
            if (ai != null) ai.enabled = false;
            
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            // Spawn death particle immediately
            if (deathParticlePrefab != null)
            {
                Instantiate(deathParticlePrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            }

            // Drop XP Orb
            if (xpOrbPrefab != null)
            {
                GameObject orb = Instantiate(xpOrbPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                if (orb.TryGetComponent<ExperienceOrb>(out var xp))
                {
                    xp.SetXP(xpValue);
                }
            }

            // Drop Random Item
            if (itemPrefabs != null && itemPrefabs.Length > 0 && Random.value < itemDropChance)
            {
                GameObject itemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
                Instantiate(itemPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            }

            // Cleanup immediately
            Destroy(gameObject);
            yield break;
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
