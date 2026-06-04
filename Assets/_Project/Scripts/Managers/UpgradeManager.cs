using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace LightNShadowSurvivor
{
    public class UpgradeManager : MonoBehaviour
    {
        public static UpgradeManager Instance { get; private set; }

        [SerializeField] private List<UpgradeData> allUpgrades;
        [SerializeField] private int cardsToDisplay = 3;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public List<UpgradeData> GetRandomUpgrades()
        {
            if (allUpgrades == null || allUpgrades.Count == 0)
            {
                Debug.LogWarning("[UpgradeManager] No upgrades found in the list!");
                return new List<UpgradeData>();
            }

            // Shuffle and pick 3
            return allUpgrades.OrderBy(x => Random.value).Take(cardsToDisplay).ToList();
        }

        public void ApplyUpgrade(UpgradeData data)
        {
            if (data == null)
            {
                Debug.LogWarning("[UpgradeManager] Tried to apply a null upgrade.");
                ResumeAfterUpgradeSelection();
                return;
            }

            Debug.Log($"[UpgradeManager] Applying upgrade: {data.upgradeName}");

            switch (data.upgradeType)
            {
                case UpgradeType.MoveSpeed:
                    if (PlayerController.Instance != null) PlayerController.Instance.IncreaseMoveSpeedPercent(data.increaseValue);
                    break;
                case UpgradeType.LightIntensity:
                    var attack = PlayerController.Instance != null ? PlayerController.Instance.GetComponentInChildren<FlashLightAttack>() : null;
                    if (attack != null) attack.IncreaseDamage(data.increaseValue);
                    break;
                case UpgradeType.Durability:
                    var health = PlayerController.Instance != null ? PlayerController.Instance.GetComponent<PlayerHealth>() : null;
                    if (health != null) health.IncreaseDurability(data.increaseValue);
                    break;
                case UpgradeType.LightRadius:
                    var attackRad = PlayerController.Instance != null ? PlayerController.Instance.GetComponentInChildren<FlashLightAttack>() : null;
                    if (attackRad != null) attackRad.IncreaseDiameterPercent(data.increaseValue);
                    break;
                case UpgradeType.AuraWisp:
                    if (AuraController.Instance != null) AuraController.Instance.AddWisp();
                    break;
            }

            ResumeAfterUpgradeSelection();
        }

        public void ResumeAfterUpgradeSelection()
        {
            if (GameManager.Instance == null || RoundManager.Instance == null) return;

            if (GameManager.Instance.CurrentState == GameState.Upgrade)
            {
                if (RoundManager.Instance.TimeRemaining <= 0)
                {
                    RoundManager.Instance.ProceedToNextRound();
                }
                else
                {
                    // If it was a mid-round level up
                    GameManager.Instance.StartRound();
                }
            }
        }
    }
}
