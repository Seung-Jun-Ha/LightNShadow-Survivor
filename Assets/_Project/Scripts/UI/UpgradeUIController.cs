using UnityEngine;
using UnityEngine.UI;

namespace LightNShadowSurvivor
{
    public class UpgradeUIController : MonoBehaviour
    {
        [SerializeField] private GameObject upgradePanel;
        [SerializeField] private Button speedButton;
        [SerializeField] private Button damageButton;
        [SerializeField] private Button healthButton;

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += HandleStateChanged;
            }

            speedButton.onClick.AddListener(() => UpgradeStat("Speed"));
            damageButton.onClick.AddListener(() => UpgradeStat("Damage"));
            healthButton.onClick.AddListener(() => UpgradeStat("Health"));

            upgradePanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state)
        {
            upgradePanel.SetActive(state == GameState.Upgrade);
            if (state == GameState.Upgrade)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        private void UpgradeStat(string statName)
        {
            Debug.Log($"Upgrading {statName}");
            
            switch (statName)
            {
                case "Speed":
                    var pc = FindFirstObjectByType<PlayerController>();
                    if (pc != null) pc.MoveSpeed += 1f;
                    break;
                case "Damage":
                    var attack = FindFirstObjectByType<FlashLightAttack>();
                    if (attack != null) attack.damagePerSecond += 5f;
                    break;
                case "Health":
                    var health = FindFirstObjectByType<PlayerHealth>();
                    if (health != null) health.IncreaseMaxHealth(20f);
                    break;
            }

            RoundManager.Instance.ProceedToNextRound();
        }
}
}
