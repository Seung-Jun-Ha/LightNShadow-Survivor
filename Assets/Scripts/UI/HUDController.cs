using UnityEngine;
using UnityEngine.UI;

namespace LightNShadowSurvivor
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private Text healthText;
        [SerializeField] private Text roundText;
        [SerializeField] private Text timerText;
        
        private PlayerHealth playerHealth;

        private void Start()
        {
            GameObject player = GameObject.Find("Player_Main");
            if (player != null)
            {
                playerHealth = player.GetComponent<PlayerHealth>();
            }
        }

        private void Update()
        {
            if (playerHealth != null && healthText != null)
            {
                healthText.text = $"HP: {Mathf.CeilToInt(playerHealth.CurrentHealth)} / {playerHealth.MaxHealth}";
            }

            if (RoundManager.Instance != null)
            {
                if (roundText != null) roundText.text = $"Round: {RoundManager.Instance.CurrentRound}";
                if (timerText != null) timerText.text = $"Time: {Mathf.CeilToInt(RoundManager.Instance.TimeRemaining)}s";
            }
        }
    }
}
