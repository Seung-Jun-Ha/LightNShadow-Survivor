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
        private const float BossDamageMultiplier = MonsterDamageMultiplier * 1.5f; // Boss deals +50% damage.
        private const float MonsterAttackRangeMultiplier = 0.5f;
        private const float MinimumSpawnRadius = 20f;
        private const string RuntimeVisibleBodyName = "RuntimeVisibleMonsterBody";

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
            RebuildMonsterMaterials(boss, Color.magenta);
            EnsureBossVisible(boss);
            ConfigureBoss(boss);
        }

        private void ConfigureBoss(GameObject boss)
        {
            var bossBase = boss.GetComponentInChildren<BossBase>();
            if (bossBase == null)
            {
                EnsureBossVisible(boss);
                return;
            }

            bossBase.ConfigureBoss(BossHealth);
            GhostAI bossAI = boss.GetComponentInChildren<GhostAI>();
            if (bossAI != null)
            {
                bossAI.ConfigureMoveSpeed(BossMoveSpeed * MonsterMoveSpeedMultiplier);
                bossAI.ConfigureDamageMultiplier(BossDamageMultiplier);
                bossAI.ConfigureAttackRangeMultiplier(MonsterAttackRangeMultiplier);
            }
            var deathHandler = boss.GetComponentInChildren<MonsterDeathHandler>();
            if (deathHandler != null) deathHandler.ConfigureExperienceReward(0f);

            // Round 3: the boss model materials use the Built-in "Standard" shader,
            // which is unsupported under URP. Rebuild them as URP Lit + magenta tint.
            RebuildMonsterMaterials(boss, Color.magenta);
            EnsureBossVisible(boss);
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
            if (currentRound == 1)
            {
                FixRound1MonsterMaterials(spawned);
                EnsureMonsterBodyVisible(spawned, Color.white);
            }
            else if (currentRound == 2)
            {
                FixRound2MonsterMaterials(spawned);
                EnsureMonsterBodyVisible(spawned, Color.magenta);
            }
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

            // Disable the third-party demo "GhostScript" (drives the dissolve/stealth
            // effect and expects a demo UI). It must not run on gameplay monsters.
            foreach (MonoBehaviour behaviour in monster.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (behaviour != null && behaviour.GetType().Name == "GhostScript")
                {
                    behaviour.enabled = false;
                }
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

        private static void FixRound1MonsterMaterials(GameObject monster)
        {
            // Remove the round-1 "stealth" look: the ghost uses a semi-transparent
            // dissolve shader. Rebuild materials as opaque URP Lit so it is fully visible.
            RebuildMonsterMaterials(monster, null);
        }

        private static void FixRound2MonsterMaterials(GameObject monster)
        {
            // Round-2 monsters are tinted magenta.
            RebuildMonsterMaterials(monster, Color.magenta);
        }

        private static void RebuildMonsterMaterials(GameObject monster, Color? tint)
        {
            if (monster == null) return;

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) return;

            foreach (Renderer renderer in monster.GetComponentsInChildren<Renderer>(true))
            {
                if (IsShadowRenderer(renderer)) continue;

                Material[] sourceMaterials = renderer.sharedMaterials;
                Material[] fixedMaterials = new Material[sourceMaterials.Length];

                for (int i = 0; i < sourceMaterials.Length; i++)
                {
                    Material source = sourceMaterials[i];
                    Material fixedMaterial = new Material(shader)
                    {
                        name = source != null ? $"{source.name}_RuntimeURP" : "Monster_RuntimeURP"
                    };

                    if (source != null)
                    {
                        CopyTexture(source, fixedMaterial, "_MainTex", "_BaseMap");
                        CopyTexture(source, fixedMaterial, "_BaseMap", "_BaseMap");
                        CopyColor(source, fixedMaterial, "_Color", "_BaseColor");
                        CopyColor(source, fixedMaterial, "_BaseColor", "_BaseColor");
                        CopyColor(source, fixedMaterial, "_EmissionColor", "_EmissionColor");
                    }
                    else if (fixedMaterial.HasProperty("_BaseColor"))
                    {
                        fixedMaterial.SetColor("_BaseColor", Color.white);
                    }

                    // A fresh URP Lit material is opaque by default, which strips any
                    // transparency/dissolve "stealth" effect from the source material.
                    if (tint.HasValue)
                    {
                        if (fixedMaterial.HasProperty("_BaseColor")) fixedMaterial.SetColor("_BaseColor", tint.Value);
                        if (fixedMaterial.HasProperty("_Color")) fixedMaterial.SetColor("_Color", tint.Value);
                    }
                    else if (fixedMaterial.HasProperty("_BaseColor"))
                    {
                        // Force full opacity so the round-1 ghost is no longer see-through.
                        Color baseColor = fixedMaterial.GetColor("_BaseColor");
                        baseColor.a = 1f;
                        fixedMaterial.SetColor("_BaseColor", baseColor);
                    }

                    fixedMaterials[i] = fixedMaterial;
                }

                renderer.materials = fixedMaterials;
            }
        }

        public static void EnsureMonsterBodyVisible(GameObject monster, Color fallbackColor)
        {
            EnsureMonsterBodyVisible(monster, fallbackColor, false);
        }

        public static void EnsureBossVisible(GameObject boss)
        {
            if (boss == null) return;

            RebuildMonsterMaterials(boss, Color.magenta);
            EnsureMonsterBodyVisible(boss, Color.magenta, true);
        }

        private static void EnsureMonsterBodyVisible(GameObject monster, Color fallbackColor, bool forceFallback)
        {
            if (monster == null) return;

            bool hasVisibleBodyRenderer = false;
            foreach (Renderer renderer in monster.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null || IsShadowRenderer(renderer)) continue;

                SetHierarchyActive(renderer.transform, monster.transform);
                renderer.enabled = true;
                renderer.forceRenderingOff = false;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                renderer.receiveShadows = true;
                hasVisibleBodyRenderer = true;
            }

            if (hasVisibleBodyRenderer && !forceFallback) return;
            if (monster.transform.Find(RuntimeVisibleBodyName) != null) return;

            GameObject fallback = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            fallback.name = RuntimeVisibleBodyName;
            fallback.transform.SetParent(monster.transform, false);
            fallback.transform.localPosition = forceFallback ? new Vector3(0f, 1.6f, 0f) : new Vector3(0f, 1f, 0f);
            fallback.transform.localRotation = Quaternion.identity;
            fallback.transform.localScale = forceFallback ? new Vector3(2.2f, 2.8f, 2.2f) : new Vector3(1.2f, 1.2f, 1.2f);

            Collider collider = fallback.GetComponent<Collider>();
            if (collider != null) DestroyRuntimeObject(collider);

            Renderer fallbackRenderer = fallback.GetComponent<Renderer>();
            if (fallbackRenderer != null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) shader = Shader.Find("Standard");
                if (shader != null)
                {
                    Material material = new Material(shader)
                    {
                        name = "RuntimeVisibleMonsterBody_Material"
                    };
                    SetMaterialColor(material, fallbackColor);
                    fallbackRenderer.material = material;
                }
            }

            Debug.LogWarning($"[MonsterSpawner] Added fallback visible body to '{monster.name}' because {(forceFallback ? "boss visibility must be guaranteed" : "no non-shadow renderer was visible")}.");
        }

        private static void SetHierarchyActive(Transform current, Transform stopAt)
        {
            while (current != null && current != stopAt)
            {
                if (!current.gameObject.activeSelf)
                {
                    current.gameObject.SetActive(true);
                }

                current = current.parent;
            }
        }

        private static bool IsShadowRenderer(Renderer renderer)
        {
            if (renderer == null) return false;
            if (renderer.GetComponentInParent<BlobShadow>(true) != null) return true;
            return renderer.name.IndexOf("shadow", System.StringComparison.OrdinalIgnoreCase) >= 0
                || renderer.gameObject.name.IndexOf("shadow", System.StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static void SetMaterialColor(Material material, Color color)
        {
            if (material == null) return;

            color.a = 1f;
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color")) material.SetColor("_Color", color);
        }

        private static void DestroyRuntimeObject(Object target)
        {
            if (target == null) return;

            if (Application.isPlaying)
            {
                Destroy(target);
            }
            else
            {
                DestroyImmediate(target);
            }
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
