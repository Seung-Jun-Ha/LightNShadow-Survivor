using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace LightNShadowSurvivor
{
    public class TeleportGhost : MonsterBase
    {
        [Header("Teleport Settings")]
        [SerializeField] private float teleportCooldown = 5f;
        [SerializeField] private float teleportDistance = 5f;
        [SerializeField] private GameObject teleportEffect;

        private Transform player;
        private NavMeshAgent agent;
        private float teleportTimer;

        protected override void Awake()
        {
            base.Awake();
            if (PlayerController.Instance != null) player = PlayerController.Instance.transform;
            agent = GetComponent<NavMeshAgent>();
            teleportTimer = teleportCooldown;
        }

        void Update()
        {
            if (isDead) return;
            
            if (player == null)
            {
                if (PlayerController.Instance != null) player = PlayerController.Instance.transform;
                if (player == null) return;
            }

            teleportTimer -= Time.deltaTime;
            if (teleportTimer <= 0)
            {
                Teleport();
                teleportTimer = teleportCooldown;
            }
        }

        private void Teleport()
        {
            Debug.Log($"[TeleportGhost] {gameObject.name} teleporting!");
            
            if (teleportEffect != null)
                Instantiate(teleportEffect, transform.position, Quaternion.identity);

            // Teleport to a random position closer to the player
            Vector3 randomDir = Random.insideUnitSphere;
            randomDir.y = 0;
            if (randomDir.sqrMagnitude < 0.0001f) randomDir = transform.forward;

            Vector3 targetPos = player.position + randomDir.normalized * teleportDistance;

            // NavMesh check
            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                if (agent != null && agent.isOnNavMesh)
                {
                    agent.Warp(hit.position);
                    agent.ResetPath();
                }
                else
                {
                    transform.position = hit.position;
                }
                
                if (teleportEffect != null)
                    Instantiate(teleportEffect, transform.position, Quaternion.identity);
            }
        }
    }
}
