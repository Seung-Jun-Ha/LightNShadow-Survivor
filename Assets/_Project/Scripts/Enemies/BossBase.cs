using UnityEngine;
using System;
using System.Collections;
using UnityEngine.AI;

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
        [SerializeField] private float bossHealthMultiplier = 4f;
        [SerializeField] private float obstacleCheckRadius = 2.2f;
        [SerializeField] private float obstacleCheckDistance = 2.5f;
        [SerializeField] private float rockAvoidanceDistance = 4f;
        
        public event Action OnShieldBroken;
        public event Action OnVulnerableStarted;
        public event Action OnVulnerableEnded;

        private bool isVulnerable = false;
        private float vulnerabilityTimer = 0f;
        private float dashTimer = 0f;
        private float spawnTimer = 0f;
        private NavMeshAgent agent;
        private float originalSpeed;
        private Animator animator;
        private float obstacleCheckTimer;

        private static readonly int AttackState = Animator.StringToHash("attack_shift");

        protected override void Awake()
        {
            base.Awake();
            maxHealth *= Mathf.Max(1f, bossHealthMultiplier);
            currentHealth = maxHealth;

            agent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();
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

        private void LateUpdate()
        {
            if (isDead) return;

            obstacleCheckTimer -= Time.deltaTime;
            if (obstacleCheckTimer > 0f) return;
            obstacleCheckTimer = 0.2f;

            HandleMapObstacles();
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

        private void HandleMapObstacles()
        {
            Vector3 center = transform.position + transform.forward * obstacleCheckDistance + Vector3.up;
            Collider[] hits = Physics.OverlapSphere(center, obstacleCheckRadius, ~0, QueryTriggerInteraction.Collide);

            foreach (Collider hit in hits)
            {
                if (hit == null) continue;

                Transform root = hit.transform.root;
                GameObject target = root != null ? root.gameObject : hit.gameObject;
                string targetName = target.name;

                if (IsTree(targetName))
                {
                    PlayTreeDestroyMotion();
                    Destroy(target);
                    continue;
                }

                if (IsRock(targetName))
                {
                    AvoidRock(hit.transform.position);
                    return;
                }
            }
        }

        private void PlayTreeDestroyMotion()
        {
            if (animator != null && animator.HasState(0, AttackState))
            {
                animator.CrossFade(AttackState, 0.08f);
            }
        }

        private void AvoidRock(Vector3 rockPosition)
        {
            if (agent == null || !agent.isOnNavMesh) return;

            Vector3 away = Vector3.ProjectOnPlane(transform.position - rockPosition, Vector3.up).normalized;
            if (away.sqrMagnitude < 0.01f) away = transform.right;

            Vector3 target = transform.position + away * rockAvoidanceDistance;
            if (NavMesh.SamplePosition(target, out NavMeshHit navHit, rockAvoidanceDistance, NavMesh.AllAreas))
            {
                agent.isStopped = false;
                agent.SetDestination(navHit.position);
            }
        }

        private static bool IsTree(string name)
        {
            string lower = name.ToLowerInvariant();
            return lower.Contains("tree") || lower.Contains("fir") || lower.Contains("oak") || lower.Contains("poplar");
        }

        private static bool IsRock(string name)
        {
            string lower = name.ToLowerInvariant();
            return lower.Contains("rock") || lower.Contains("stone");
        }
    }
}
