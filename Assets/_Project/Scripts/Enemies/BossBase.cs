using UnityEngine;
using System;
using System.Collections;

namespace LightNShadowSurvivor
{
    public class BossBase : MonsterBase
    {
        [Header("Boss Settings")]
        [SerializeField] private bool enableBehavior;
        [SerializeField] private float vulnerabilityTime = 5f;
        [SerializeField] private GameObject shieldVisual;
        [SerializeField] private float dashCooldown = 8f;
        [SerializeField] private float dashSpeedMultiplier = 3f;
        [SerializeField] private float dashDuration = 1.5f;
        [SerializeField] private GameObject minionPrefab;
        [SerializeField] private float minionSpawnInterval = 10f;

        [Header("Slam Attack")]
        [SerializeField] private float slamRange = 3.5f;
        [SerializeField] private float slamCooldown = 4f;
        [SerializeField] private float slamDamage = 18f;
        [SerializeField] private float slamWindup = 0.55f;
        [SerializeField] private float slamImpactRadius = 3f;
        // The orc's authored overhead hand-slam animation state (Base Layer).
        [SerializeField] private string slamAnimationState = "Monster_anim|Atack_3";

        public event Action OnShieldBroken;
        public event Action OnVulnerableStarted;
        public event Action OnVulnerableEnded;

        private bool isVulnerable = false;
        private float vulnerabilityTimer = 0f;
        private float dashTimer = 0f;
        private float spawnTimer = 0f;
        private UnityEngine.AI.NavMeshAgent agent;
        private float originalSpeed;

        private Animator bossAnimator;
        private GhostAI ghostAI;
        private Transform player;
        private PlayerHealth playerHealth;
        private float slamTimer;
        private bool isSlamming;

        protected override void Awake()
        {
            base.Awake();
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null) originalSpeed = agent.speed;
            if (shieldVisual != null) shieldVisual.SetActive(currentShield > 0);
            
            dashTimer = dashCooldown;
            spawnTimer = minionSpawnInterval;

            bossAnimator = GetComponentInChildren<Animator>();
            ghostAI = GetComponentInChildren<GhostAI>();
            slamTimer = slamCooldown;
        }

        public void ConfigureBoss(float health)
        {
            maxHealth = Mathf.Max(1f, health);
            currentHealth = maxHealth;
            shieldHealth = 0f;
            currentShield = 0f;
            reactionType = GhostReactionType.Boss;

            if (shieldVisual != null) shieldVisual.SetActive(false);
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

            // The slam attack is always active when the player gets close.
            HandleSlamAttack();

            if (!enableBehavior) return;

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

        private void HandleSlamAttack()
        {
            if (isSlamming) return;

            if (player == null)
            {
                if (PlayerController.Instance != null)
                {
                    player = PlayerController.Instance.transform;
                    playerHealth = PlayerController.Instance.GetComponent<PlayerHealth>();
                }
                if (player == null) return;
            }

            slamTimer -= Time.deltaTime;
            if (slamTimer > 0f) return;

            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= slamRange)
            {
                slamTimer = slamCooldown;
                StartCoroutine(SlamRoutine());
            }
        }

        private IEnumerator SlamRoutine()
        {
            isSlamming = true;

            // Pause chase/melee AI for the duration of the slam.
            bool aiWasEnabled = ghostAI != null && ghostAI.enabled;
            if (ghostAI != null) ghostAI.enabled = false;

            bool agentWasStopped = false;
            if (agent != null && agent.isOnNavMesh)
            {
                agentWasStopped = agent.isStopped;
                agent.isStopped = true;
            }

            // Face the player before slamming.
            if (player != null)
            {
                Vector3 facing = player.position - transform.position;
                facing.y = 0f;
                if (facing.sqrMagnitude > 0.01f)
                {
                    transform.rotation = Quaternion.LookRotation(facing);
                }
            }

            // Play the orc's authored overhead hand-slam attack animation so the
            // boss strikes down with its arm/hand (not by dropping its whole body).
            if (bossAnimator != null && !string.IsNullOrEmpty(slamAnimationState))
            {
                bossAnimator.CrossFade(slamAnimationState, 0.1f);
            }

            // Wind-up: wait for the hand to come down before the strike connects.
            float elapsed = 0f;
            while (elapsed < slamWindup && !isDead)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Impact: damage the player if still within the impact radius.
            if (!isDead && player != null && playerHealth != null)
            {
                float impactDistance = Vector3.Distance(transform.position, player.position);
                if (impactDistance <= slamImpactRadius)
                {
                    playerHealth.TakeDamage(slamDamage);
                }
            }

            // Recovery before resuming the chase.
            yield return new WaitForSeconds(0.5f);

            if (agent != null && agent.isOnNavMesh) agent.isStopped = agentWasStopped;
            if (ghostAI != null && aiWasEnabled) ghostAI.enabled = true;

            isSlamming = false;
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

        protected override void Die()
        {
            if (isDead) return;

            base.Die();
            TimeOfDayManager timeOfDayManager = FindAnyObjectByType<TimeOfDayManager>();
            if (timeOfDayManager != null)
            {
                timeOfDayManager.ClearAllStages();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerEnding();
            }
        }
    }
}
