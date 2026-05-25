using UnityEngine;

namespace LightNShadowSurvivor
{
    public class ExperienceOrb : MonoBehaviour
    {
        [SerializeField] private float xpAmount = 10f;
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float collectDistance = 1.5f;
        [SerializeField] private float magnetDistance = 5f;

        private Transform player;
        private bool isCollecting = false;

        private void Start()
        {
            if (PlayerController.Instance != null) player = PlayerController.Instance.transform;
        }

        private void Update()
        {
            if (player == null)
            {
                if (PlayerController.Instance != null) player = PlayerController.Instance.transform;
                if (player == null) return;
            }

            float distance = Vector3.Distance(transform.position, player.position);

            if (distance < magnetDistance)
            {
                isCollecting = true;
            }

            if (isCollecting)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

                if (distance < collectDistance)
                {
                    Collect();
                }
            }
        }

        private void Collect()
        {
            if (PlayerExperience.Instance != null)
            {
                PlayerExperience.Instance.AddXP(xpAmount);
            }
            Destroy(gameObject);
        }
        
        public void SetXP(float amount)
        {
            xpAmount = amount;
        }
    }
}
