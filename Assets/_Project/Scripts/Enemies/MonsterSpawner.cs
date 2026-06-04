using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LightNShadowSurvivor
{
    public class MonsterSpawner : MonoBehaviour
    {
        private const float Round1SpawnInterval = 1f;
        private const float Round2SpawnInterval = 2f;
        private const float Round1Health = 2f;
        private const float Round2Health = 4f;
        private const float BossHealth = 30f;
        private const float Round1Experience = 3f;
        private const float Round2Experience = 5f;

        [Header("Spawn Settings")]
        [SerializeField] private List<GameObject> round1Monsters;
        [SerializeField] private List<GameObject> round2Monsters;
        [SerializeField] private List<GameObject> round3Monsters;
        [SerializeField] private GameObject bossPrefab;
        [SerializeField] private float spawnRadius = 10f;
        [SerializeField] private float startDelay = 2f;
        [SerializeField] private int maxMonsters = 50;

        private Transform player;
        private int currentRound = 1;
        private bool isSpawning = false;
        private bool bossSpawned = false;
        private Coroutine spawnCoroutine;
        private List<GameObject> activeMonsters = new List<GameObject>();

        private void Start()
        {
            if (PlayerController.Instance != null)
            {
                player = PlayerController.Instance.transform;
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += HandleStateChanged;
                HandleStateChanged(GameManager.Instance.CurrentState);
            }
        }

        private void Update()
        {
            if (player == null && PlayerController.Instance != null)
            {
                player = PlayerController.Instance.transform;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.Round)
            {
                StartSpawning();
            }
            else
            {
                StopSpawning();
            }
        }

        public void SetRound(int round)
        {
            if (currentRound != round)
            {
                bossSpawned = false;
            }

            currentRound = round;
        }

        public void StartSpawning()
        {
            if (isSpawning) return;
            isSpawning = true;
            spawnCoroutine = StartCoroutine(SpawnRoutine());
        }

        public void StopSpawning()
        {
            isSpawning = false;
            if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        }

        private IEnumerator SpawnRoutine()
        {
            yield return new WaitForSeconds(startDelay);

            while (isSpawning)
            {
                // Cleanup null references (dead monsters)
                activeMonsters.RemoveAll(m => m == null);

                if (player != null && activeMonsters.Count < maxMonsters)
                {
                    if (currentRound == 3)
                    {
                        if (!bossSpawned && bossPrefab != null)
                        {
                            SpawnBoss();
                        }
                    }
                    else
                    {
                        SpawnMonster();
                    }
                }
                yield return new WaitForSeconds(GetCurrentSpawnInterval());
            }
        }

        private void SpawnBoss()
        {
            bossSpawned = true;
            Debug.Log("[MonsterSpawner] Spawning BOSS!");
            Vector3 spawnPos = GetRandomPositionAroundPlayer();
            GameObject boss = Instantiate(bossPrefab, spawnPos, Quaternion.identity);
            activeMonsters.Add(boss);
            SetupEnemyLayer(boss);
            ConfigureBoss(boss);
        }

        private void ConfigureBoss(GameObject boss)
        {
            var bossBase = boss.GetComponentInChildren<BossBase>();
            if (bossBase == null) return;

            bossBase.ConfigureBoss(BossHealth);
            var deathHandler = boss.GetComponentInChildren<MonsterDeathHandler>();
            if (deathHandler != null) deathHandler.ConfigureExperienceReward(0f);
            Debug.Log($"[MonsterSpawner] Boss HP configured to {BossHealth:F0}.");
        }

        private void SpawnMonster()
        {
            Debug.Log("[MonsterSpawner] Attempting to spawn a monster...");
            List<GameObject> monsterPool;
            switch (currentRound)
            {
                case 1: monsterPool = round1Monsters; break;
                case 2: monsterPool = round2Monsters; break;
                default: monsterPool = round3Monsters; break;
            }

            if (monsterPool == null || monsterPool.Count == 0)
            {
                Debug.LogWarning("[MonsterSpawner] Monster pool is empty!");
                return;
            }

            List<GameObject> sanitizedPool = GetSanitizedPool(monsterPool);
            if (sanitizedPool.Count == 0)
            {
                Debug.LogWarning("[MonsterSpawner] No usable regular monster prefab remains after removing special variants.");
                return;
            }

            GameObject prefab = sanitizedPool[Random.Range(0, sanitizedPool.Count)];
            Vector3 spawnPos = GetRandomPositionAroundPlayer();
            Debug.Log($"[MonsterSpawner] Calculated spawn position: {spawnPos}");
            
            GameObject spawned = Instantiate(prefab, spawnPos, Quaternion.identity);
            activeMonsters.Add(spawned);
            Debug.Log($"[MonsterSpawner] Successfully instantiated {spawned.name}");
            
            SetupEnemyLayer(spawned);
            ConfigureRegularMonster(spawned);
        }

        private void SetupEnemyLayer(GameObject obj)
        {
            int enemyLayer = LayerMask.NameToLayer("Enemy");
            if (enemyLayer != -1)
            {
                obj.layer = enemyLayer;
                foreach (Transform t in obj.GetComponentsInChildren<Transform>(true))
                {
                    t.gameObject.layer = enemyLayer;
                }
            }
        }
        private float GetCurrentSpawnInterval()
        {
            switch (currentRound)
            {
                case 1: return Round1SpawnInterval;
                case 2: return Round2SpawnInterval;
                default: return 1f;
            }
        }

        private void ConfigureRegularMonster(GameObject monster)
        {
            DisableRemovedVariantBehaviors(monster);

            MonsterBase monsterBase = monster.GetComponentInChildren<MonsterBase>();
            if (monsterBase == null) monsterBase = monster.AddComponent<MonsterBase>();

            float health = currentRound == 1 ? Round1Health : Round2Health;
            float experience = currentRound == 1 ? Round1Experience : Round2Experience;
            monsterBase.ConfigureForRound(health, experience);

            MonsterDeathHandler deathHandler = monster.GetComponentInChildren<MonsterDeathHandler>();
            if (deathHandler != null) deathHandler.ConfigureExperienceReward(experience);
        }

        private static List<GameObject> GetSanitizedPool(List<GameObject> source)
        {
            var sanitized = new List<GameObject>();
            if (source == null) return sanitized;

            foreach (GameObject prefab in source)
            {
                if (prefab != null && !IsRemovedVariant(prefab))
                {
                    sanitized.Add(prefab);
                }
            }

            if (sanitized.Count == 0)
            {
                GameObject fallback = source.Find(prefab => prefab != null);
                if (fallback != null)
                {
                    sanitized.Add(fallback);
                    Debug.LogWarning($"[MonsterSpawner] Converting removed variant '{fallback.name}' to a normal monster because no normal prefab is assigned.");
                }
            }

            return sanitized;
        }

        private static bool IsRemovedVariant(GameObject prefab)
        {
            string lowerName = prefab.name.ToLowerInvariant();
            return prefab.GetComponentInChildren<ShieldGhost>(true) != null
                || prefab.GetComponentInChildren<TeleportGhost>(true) != null
                || prefab.GetComponentInChildren<BlobShadow>(true) != null
                || lowerName.Contains("shield")
                || lowerName.Contains("teleport")
                || lowerName.Contains("split")
                || lowerName.Contains("blob");
        }

        private static void DisableRemovedVariantBehaviors(GameObject monster)
        {
            foreach (ShieldGhost shield in monster.GetComponentsInChildren<ShieldGhost>(true))
            {
                shield.enabled = false;
            }

            foreach (TeleportGhost teleport in monster.GetComponentsInChildren<TeleportGhost>(true))
            {
                teleport.enabled = false;
            }

            foreach (BlobShadow blob in monster.GetComponentsInChildren<BlobShadow>(true))
            {
                blob.enabled = false;
            }
        }

        public void StopAndClearMonsters()
        {
            StopSpawning();
            activeMonsters.RemoveAll(m => m == null);
            foreach (GameObject monster in activeMonsters)
            {
                if (monster == null) continue;
                if (monster.TryGetComponent(out MonsterBase monsterBase)) monsterBase.StopBehavior();
                Destroy(monster, 0.25f);
            }
            activeMonsters.Clear();
        }

        private Vector3 GetRandomPositionAroundPlayer()
        {
            if (player == null) return transform.position;

            float angle = Random.Range(0f, Mathf.PI * 2);
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * spawnRadius;
            Vector3 spawnPos = player.position + offset;
            
            // Try to find a valid position on the NavMesh near the target point
            if (UnityEngine.AI.NavMesh.SamplePosition(spawnPos, out UnityEngine.AI.NavMeshHit hit, 20f, UnityEngine.AI.NavMesh.AllAreas))
            {
                return hit.position;
            }
            
            Debug.LogWarning($"[MonsterSpawner] Could not find NavMesh position for spawn at {spawnPos}. Falling back to player height.");
            // Fallback: spawn at player height if mesh not found
            spawnPos.y = player.position.y; 
            return spawnPos;
        }
    }
}

