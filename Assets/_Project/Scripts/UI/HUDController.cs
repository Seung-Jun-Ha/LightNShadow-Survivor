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
            Debug.Log("[HUDController] Start called.");
            GameObject player = GameObject.Find("Player_Main");
            if (player != null)
            {
                playerHealth = player.GetComponent<PlayerHealth>();
                Debug.Log("[HUDController] Player_Main found.");
            }
            else
            {
                Debug.LogWarning("[HUDController] Player_Main NOT found in scene!");
            }

            if (healthText == null) Debug.LogError("[HUDController] healthText reference missing!");
            if (roundText == null) Debug.LogError("[HUDController] roundText reference missing!");
            if (timerText == null) Debug.LogError("[HUDController] timerText reference missing!");
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
