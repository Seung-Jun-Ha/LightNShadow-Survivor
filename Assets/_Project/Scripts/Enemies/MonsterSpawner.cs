using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LightNShadowSurvivor
{
    public class MonsterSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private List<GameObject> round1Monsters;
        [SerializeField] private List<GameObject> round2Monsters;
        [SerializeField] private List<GameObject> round3Monsters;
        [SerializeField] private GameObject bossPrefab;
        [SerializeField] private float spawnRadius = 10f;
        [SerializeField] private float round1SpawnInterval = 0.96f;
        [SerializeField] private float round2SpawnInterval = 0.72f;
        [SerializeField] private float round3SpawnInterval = 0.48f;
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
                    if (currentRound == 3 && !bossSpawned && bossPrefab != null)
                    {
                        SpawnBoss();
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
            EnsureVisibleMonsterRenderers(boss);
            FixMissingMonsterMaterials(boss);
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

            GameObject prefab = monsterPool[Random.Range(0, monsterPool.Count)];
            Vector3 spawnPos = GetRandomPositionAroundPlayer();
            Debug.Log($"[MonsterSpawner] Calculated spawn position: {spawnPos}");
            
            GameObject spawned = Instantiate(prefab, spawnPos, Quaternion.identity);
            activeMonsters.Add(spawned);
            Debug.Log($"[MonsterSpawner] Successfully instantiated {spawned.name}");
            
            SetupEnemyLayer(spawned);
            EnsureVisibleMonsterRenderers(spawned);
            FixMissingMonsterMaterials(spawned);
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
                case 1: return round1SpawnInterval;
                case 2: return round2SpawnInterval;
                default: return round3SpawnInterval;
            }
        }

        private static void EnsureVisibleMonsterRenderers(GameObject monster)
        {
            if (monster == null) return;

            foreach (Renderer renderer in monster.GetComponentsInChildren<Renderer>(true))
            {
                renderer.enabled = true;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                renderer.receiveShadows = true;
            }
        }

        private static void FixMissingMonsterMaterials(GameObject monster)
        {
            if (monster == null) return;

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) return;

            foreach (Renderer renderer in monster.GetComponentsInChildren<Renderer>(true))
            {
                Material[] materials = renderer.sharedMaterials;
                bool changed = false;

                for (int i = 0; i < materials.Length; i++)
                {
                    Material material = materials[i];
                    if (material != null && material.shader != null && material.shader.name != "Hidden/InternalErrorShader") continue;

                    Material fallback = new Material(shader) { name = "Monster_VisibleFallback" };
                    if (fallback.HasProperty("_BaseColor")) fallback.SetColor("_BaseColor", Color.white);
                    else fallback.color = Color.white;
                    materials[i] = fallback;
                    changed = true;
                }

                if (changed) renderer.materials = materials;
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

