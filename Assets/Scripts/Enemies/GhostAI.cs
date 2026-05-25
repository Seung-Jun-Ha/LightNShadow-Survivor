using UnityEngine;
using UnityEngine.AI;

namespace LightNShadowSurvivor
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class GhostAI : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackCooldown = 2.0f;
        [SerializeField] private float damageAmount = 10f;
        [SerializeField] private float moveSpeed = 3.5f;

        [Header("Floating Animation")]
        [SerializeField] private float floatHeight = 0.5f;
        [SerializeField] private float floatAmplitude = 0.2f;
        [SerializeField] private float floatFrequency = 1.5f;

        private NavMeshAgent agent;
        private Animator animator;
        private Transform visualRoot;
        private Transform player;
        private PlayerHealth playerHealth;
        private float lastAttackTime;
        private bool isSlowing = false;
        private float slowTimer = 0f;
        private float originalSpeed;
        private float randomOffset;

        private static readonly int MoveState = Animator.StringToHash("move");
        private static readonly int AttackState = Animator.StringToHash("attack_shift");
        private static readonly int IdleState = Animator.StringToHash("idle");

        private MonsterBase monsterBase;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();
            monsterBase = GetComponent<MonsterBase>();
            
            // Disable CharacterController if it exists as it conflicts with NavMeshAgent
            if (TryGetComponent(out CharacterController cc))
            {
                cc.enabled = false;
            }

            // --- FIX: Ensure active collider and rigidbody for hit detection ---
            if (GetComponent<Collider>() == null || !GetComponent<Collider>().enabled)
            {
                var col = gameObject.AddComponent<CapsuleCollider>();
                col.center = new Vector3(0, 1, 0);
                col.height = 2f;
                col.radius = 0.5f;
            }

            if (GetComponent<Rigidbody>() == null)
            {
                var rb = gameObject.AddComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            // Force layer to Enemy
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            // ------------------------------------------------------------------

            // Usually the animator is on the visual object
            if (animator != null) visualRoot = animator.transform;
            
            originalSpeed = moveSpeed;
            agent.speed = moveSpeed;
            randomOffset = Random.value * Mathf.PI * 2f;
        }

        private void Start()
        {
            GameObject playerObj = GameObject.Find("Player_Main");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerHealth = playerObj.GetComponent<PlayerHealth>();
            }

            // Ensure agent is on NavMesh
            if (agent != null && !agent.isOnNavMesh)
            {
                if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                {
                    agent.Warp(hit.position);
                }
            }
        }

        private void Update()
        {
            if (player == null) return;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange)
            {
                AttackPlayer();
            }
            else
            {
                MoveToPlayer();
            }

            HandleSlowEffect();
            HandleFloatingMotion();
            UpdateAnimation();
        }

        private void HandleFloatingMotion()
        {
            if (visualRoot != null)
            {
                float yOffset = floatHeight + Mathf.Sin(Time.time * floatFrequency + randomOffset) * floatAmplitude;
                Vector3 targetLocalPos = visualRoot.localPosition;
                targetLocalPos.y = yOffset;
                visualRoot.localPosition = targetLocalPos;
            }
        }

        private void MoveToPlayer()
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
        }

        private void AttackPlayer()
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = true;
            }
            
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                if (animator != null)
                {
                    animator.CrossFade(AttackState, 0.1f);
                }
                
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damageAmount);
                }
            }
        }

        public void ApplySlow()
        {
            isSlowing = true;
            slowTimer = 0.2f; // Short duration, refreshed by FlashLightAttack
            
            float slowFactor = 0.5f; // Default 50% slow
            if (monsterBase != null)
            {
                if (monsterBase.ReactionType == GhostReactionType.Fast)
                {
                    slowFactor = 0.2f; // 80% slow for Fast ghosts
                }
            }
            
            agent.speed = originalSpeed * slowFactor;
        }

        private void HandleSlowEffect()
        {
            if (isSlowing)
            {
                slowTimer -= Time.deltaTime;
                if (slowTimer <= 0)
                {
                    isSlowing = false;
                    agent.speed = originalSpeed;
                }
            }
        }

        private void UpdateAnimation()
        {
            if (animator == null) return;

            bool isMoving = agent.velocity.magnitude > 0.1f && !agent.isStopped;
            
            if (isMoving)
            {
                var state = animator.GetCurrentAnimatorStateInfo(0);
                if (!state.IsName("attack_shift") && !state.IsName("move"))
                {
                    animator.CrossFade(MoveState, 0.1f);
                }
            }
            else if (agent.isStopped && !animator.GetCurrentAnimatorStateInfo(0).IsName("attack_shift"))
            {
                animator.CrossFade(IdleState, 0.1f);
            }
        }
    }
}
