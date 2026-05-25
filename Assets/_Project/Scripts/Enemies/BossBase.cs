using UnityEngine;
using System;
using System.Collections;

namespace LightNShadowSurvivor
{
    public class BossBase : MonsterBase
    {
        [Header("Boss Settings")]
        [SerializeField] private float vulnerabilityTime = 5f;
        [SerializeField] private GameObject shieldVisual;
        [SerializeField] private float dashCooldown = 8f;
        [SerializeField] private float dashSpeedMultiplier = 3f;
        [SerializeField] private float dashDuration = 1.5f;
        [SerializeField] private GameObject minionPrefab;
        [SerializeField] private float minionSpawnInterval = 10f;
        
        public event Action OnShieldBroken;
        public event Action OnVulnerableStarted;
        public event Action OnVulnerableEnded;

        private bool isVulnerable = false;
        private float vulnerabilityTimer = 0f;
        private float dashTimer = 0f;
        private float spawnTimer = 0f;
        private UnityEngine.AI.NavMeshAgent agent;
        private float originalSpeed;

        protected override void Awake()
        {
            base.Awake();
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null) originalSpeed = agent.speed;
            if (shieldVisual != null) shieldVisual.SetActive(currentShield > 0);
            
            dashTimer = dashCooldown;
            spawnTimer = minionSpawnInterval;
        }

        public override void ModifyHealth(float amount)
        {
            if (isDead) return;

            bool hadShield = currentShield > 0;
            
            base.ModifyHealth(amount);

            if (hadShield && currentShield <= 0)
            {
                BreakShield();
            }
        }

        private void BreakShield()
        {
            Debug.Log("[BossBase] Shield Broken!");
            if (shieldVisual != null) shieldVisual.SetActive(false);
            OnShieldBroken?.Invoke();
            
            StartVulnerability();
        }

        private void StartVulnerability()
        {
            isVulnerable = true;
            vulnerabilityTimer = vulnerabilityTime;
            
            // Slow down or stun while vulnerable
            if (agent != null) agent.speed = originalSpeed * 0.2f;
            
            OnVulnerableStarted?.Invoke();
        }

        void Update()
        {
            if (isDead) return;

            if (isVulnerable)
            {
                vulnerabilityTimer -= Time.deltaTime;
                if (vulnerabilityTimer <= 0)
                {
                    EndVulnerability();
                }
            }
            else
            {
                HandlePatterns();
            }
        }

        private void HandlePatterns()
        {
            // Dash Pattern
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                StartCoroutine(DashRoutine());
                dashTimer = dashCooldown;
            }

            // Minion Spawn Pattern (only while shield is up)
            if (currentShield > 0)
            {
                spawnTimer -= Time.deltaTime;
                if (spawnTimer <= 0)
                {
                    SpawnMinions();
                    spawnTimer = minionSpawnInterval;
                }
            }
        }

        private IEnumerator DashRoutine()
        {
            if (agent == null) yield break;

            Debug.Log("[BossBase] Dashing!");
            agent.speed = originalSpeed * dashSpeedMultiplier;
            agent.acceleration = 100f; // Rapid acceleration

            yield return new WaitForSeconds(dashDuration);

            agent.speed = originalSpeed;
            agent.acceleration = 8f; // Reset to default-ish
        }

        private void SpawnMinions()
        {
            if (minionPrefab == null) return;

            Debug.Log("[BossBase] Spawning Minions!");
            for (int i = 0; i < 3; i++)
            {
                Vector3 spawnOffset = UnityEngine.Random.insideUnitSphere * 3f;
                spawnOffset.y = 0;
                Instantiate(minionPrefab, transform.position + spawnOffset, Quaternion.identity);
            }
        }

        private void EndVulnerability()
        {
            isVulnerable = false;
            if (agent != null) agent.speed = originalSpeed;
            
            OnVulnerableEnded?.Invoke();
        }
    }
}
