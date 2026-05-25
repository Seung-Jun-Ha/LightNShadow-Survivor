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
        [SerializeField] private float spawnRadius = 15f; // Reduced from 40f
        [SerializeField] private float spawnInterval = 2f; // Reduced from 6f

        private Transform player;
        private int currentRound = 1;
        private bool isSpawning = false;
        private Coroutine spawnCoroutine;

        private void Start()
        {
            FindPlayer();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += HandleStateChanged;
                HandleStateChanged(GameManager.Instance.CurrentState);
            }
        }

        private void FindPlayer()
        {
            GameObject playerObj = GameObject.Find("Player_Main");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("[MonsterSpawner] Player found and assigned.");
            }
            else
            {
                Debug.LogWarning("[MonsterSpawner] Player_Main NOT found!");
            }
        }

        private void Update()
        {
            if (player == null) FindPlayer();
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
            currentRound = round;
            // Faster spawn rate: round 1 starts at 3s, decreases significantly
            spawnInterval = Mathf.Max(0.5f, 3f - (round - 1) * 1.0f);
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
            while (isSpawning)
            {
                if (player != null)
                {
                    SpawnMonster();
                }
                yield return new WaitForSeconds(spawnInterval);
            }
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
            Debug.Log($"[MonsterSpawner] Successfully instantiated {spawned.name}");
            
            // Force layer to Enemy
            int enemyLayer = LayerMask.NameToLayer("Enemy");
            if (enemyLayer != -1)
            {
                spawned.layer = enemyLayer;
                foreach (Transform t in spawned.GetComponentsInChildren<Transform>(true))
                {
                    t.gameObject.layer = enemyLayer;
                }
            }
        }

        private Vector3 GetRandomPositionAroundPlayer()
        {
            float angle = Random.Range(0f, Mathf.PI * 2);
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * spawnRadius;
            Vector3 spawnPos = player.position + offset;
            
            // Try to find a valid position on the NavMesh near the target point
            if (UnityEngine.AI.NavMesh.SamplePosition(spawnPos, out UnityEngine.AI.NavMeshHit hit, 10f, UnityEngine.AI.NavMesh.AllAreas))
            {
                return hit.position;
            }
            
            // Fallback: spawn at player height if mesh not found
            spawnPos.y = player.position.y; 
            return spawnPos;
        }
}
}
