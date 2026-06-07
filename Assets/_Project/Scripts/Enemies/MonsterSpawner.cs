using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LightNShadowSurvivor
{
    public class MonsterSpawner : MonoBehaviour
    {
        private const float Round1SpawnInterval = 2f;
        private const float Round2SpawnInterval = 4f;
        private const float Round1Health = 2f;
        private const float Round2Health = 4f;
        private const float BossHealth = 30f;
        private const float Round1Experience = 3f;
        private const float Round2Experience = 5f;
        private const float Round1MoveSpeed = 3.5f;
        private const float Round2MoveSpeed = 4f;
        private const float BossMoveSpeed = 4.5f;
        private const float MonsterMoveSpeedMultiplier = 0.7f;
        private const float MonsterDamageMultiplier = 0.7f;
        private const float MonsterAttackRangeMultiplier = 0.5f;
        private const float MinimumSpawnRadius = 20f;

        [Header("Spawn Settings")]
        [SerializeField] private List<GameObject> round1Monsters;
        [SerializeField] private List<GameObject> round2Monsters;
        [SerializeField] private List<GameObject> round3Monsters;
        [SerializeField] private bool autoUseCuteGhostsForRound2 = true;
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
        private static readonly Dictionary<Material, Material> RuntimeMaterialCache = new Dictionary<Material, Material>();
        private static Material fallbackVisibleMaterial;

        private void Start()
        {
            PopulateRound2CuteGhostsIfNeeded();
            spawnRadius = Mathf.Max(spawnRadius, MinimumSpawnRadius);

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
            RepairMonsterVisibility(boss);
        }

        private void ConfigureBoss(GameObject boss)
        {
            var bossBase = boss.GetComponentInChildren<BossBase>();
            if (bossBase == null) return;

            bossBase.ConfigureBoss(BossHealth);
            GhostAI bossAI = boss.GetComponentInChildren<GhostAI>();
            if (bossAI != null)
            {
                bossAI.ConfigureMoveSpeed(BossMoveSpeed * MonsterMoveSpeedMultiplier);
                bossAI.ConfigureDamageMultiplier(MonsterDamageMultiplier);
                bossAI.ConfigureAttackRangeMultiplier(MonsterAttackRangeMultiplier);
            }
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
            RepairMonsterVisibility(spawned);
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
                default: return 2f;
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

            GhostAI ghostAI = monster.GetComponentInChildren<GhostAI>();
            if (ghostAI == null)
            {
                ghostAI = monster.AddComponent<GhostAI>();
            }
            float moveSpeed = currentRound == 1 ? Round1MoveSpeed : Round2MoveSpeed;
            ghostAI.ConfigureMoveSpeed(moveSpeed * MonsterMoveSpeedMultiplier);
            ghostAI.ConfigureDamageMultiplier(MonsterDamageMultiplier);
            ghostAI.ConfigureAttackRangeMultiplier(MonsterAttackRangeMultiplier);

            if (monster.GetComponentInChildren<LightDamageReceiver>() == null)
            {
                monster.AddComponent<LightDamageReceiver>();
            }

            MonsterDeathHandler deathHandler = monster.GetComponentInChildren<MonsterDeathHandler>();
            if (deathHandler == null) deathHandler = monster.AddComponent<MonsterDeathHandler>();
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

        private void PopulateRound2CuteGhostsIfNeeded()
        {
            if (!autoUseCuteGhostsForRound2) return;

#if UNITY_EDITOR
            round2Monsters ??= new List<GameObject>();
            round2Monsters.Clear();

            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/ThirdParty/Monster_Ghosts_FREE/Prefabs" });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.Contains("Little_Ghost_ZOMbi")) continue;

                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null && !round2Monsters.Contains(prefab))
                {
                    round2Monsters.Add(prefab);
                }
            }

            if (round2Monsters.Count == 0)
            {
                Debug.LogWarning("[MonsterSpawner] Could not auto-populate Cute Monster Ghost prefabs for round 2.");
            }
#endif
        }

        private static void RepairMonsterVisibility(GameObject monster)
        {
            if (monster == null) return;

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) return;

            foreach (Renderer renderer in monster.GetComponentsInChildren<Renderer>(true))
            {
                renderer.enabled = true;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                renderer.receiveShadows = true;

                Material[] sourceMaterials = renderer.sharedMaterials;
                if (sourceMaterials == null || sourceMaterials.Length == 0)
                {
                    renderer.sharedMaterial = GetFallbackVisibleMaterial(shader);
                    continue;
                }

                Material[] fixedMaterials = new Material[sourceMaterials.Length];

                for (int i = 0; i < sourceMaterials.Length; i++)
                {
                    Material source = sourceMaterials[i];
                    if (source != null && source.shader != null && source.shader.name != "Hidden/InternalErrorShader")
                    {
                        fixedMaterials[i] = source;
                        continue;
                    }

                    fixedMaterials[i] = source != null
                        ? GetRuntimeReplacementMaterial(source, shader)
                        : GetFallbackVisibleMaterial(shader);
                }

                renderer.sharedMaterials = fixedMaterials;
            }
        }

        private static Material GetRuntimeReplacementMaterial(Material source, Shader shader)
        {
            if (RuntimeMaterialCache.TryGetValue(source, out Material cached) && cached != null)
            {
                return cached;
            }

            Material replacement = new Material(shader)
            {
                name = $"{source.name}_RuntimeURP"
            };

            CopyTexture(source, replacement, "_MainTex", "_BaseMap");
            CopyTexture(source, replacement, "_BaseMap", "_BaseMap");
            CopyColor(source, replacement, "_Color", "_BaseColor");
            CopyColor(source, replacement, "_BaseColor", "_BaseColor");
            CopyColor(source, replacement, "_EmissionColor", "_EmissionColor");

            RuntimeMaterialCache[source] = replacement;
            return replacement;
        }

        private static Material GetFallbackVisibleMaterial(Shader shader)
        {
            if (fallbackVisibleMaterial != null && fallbackVisibleMaterial.shader == shader)
            {
                return fallbackVisibleMaterial;
            }

            fallbackVisibleMaterial = new Material(shader) { name = "Monster_RuntimeVisible" };
            if (fallbackVisibleMaterial.HasProperty("_BaseColor")) fallbackVisibleMaterial.SetColor("_BaseColor", Color.white);
            else if (fallbackVisibleMaterial.HasProperty("_Color")) fallbackVisibleMaterial.SetColor("_Color", Color.white);
            return fallbackVisibleMaterial;
        }

        private static void CopyTexture(Material source, Material target, string sourceProperty, string targetProperty)
        {
            if (!source.HasProperty(sourceProperty) || !target.HasProperty(targetProperty)) return;

            Texture texture = source.GetTexture(sourceProperty);
            if (texture != null) target.SetTexture(targetProperty, texture);
        }

        private static void CopyColor(Material source, Material target, string sourceProperty, string targetProperty)
        {
            if (!source.HasProperty(sourceProperty) || !target.HasProperty(targetProperty)) return;

            target.SetColor(targetProperty, source.GetColor(sourceProperty));
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

