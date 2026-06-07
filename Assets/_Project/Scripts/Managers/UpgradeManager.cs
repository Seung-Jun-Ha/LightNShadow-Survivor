using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LightNShadowSurvivor
{
    public class UpgradeManager : MonoBehaviour
    {
        public static UpgradeManager Instance { get; private set; }

        [SerializeField] private List<UpgradeData> allUpgrades;
        [SerializeField] private int cardsToDisplay = 3;
        [SerializeField] private bool autoPopulateUpgrades = true;

        private readonly List<UpgradeData> previousSelection = new List<UpgradeData>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            PopulateUpgradesIfNeeded();
        }

        public List<UpgradeData> GetRandomUpgrades()
        {
            PopulateUpgradesIfNeeded();

            if (allUpgrades == null || allUpgrades.Count == 0)
            {
                Debug.LogWarning("[UpgradeManager] No upgrades found in the list!");
                return new List<UpgradeData>();
            }

            List<UpgradeData> pool = new List<UpgradeData>(allUpgrades.Count);
            foreach (var upgrade in allUpgrades)
            {
                if (upgrade != null) pool.Add(upgrade);
            }

            if (pool.Count > cardsToDisplay)
            {
                pool.RemoveAll(upgrade => previousSelection.Contains(upgrade));
                if (pool.Count < cardsToDisplay)
                {
                    foreach (var upgrade in allUpgrades)
                    {
                        if (upgrade != null && !pool.Contains(upgrade))
                        {
                            pool.Add(upgrade);
                        }
                    }
                }
            }

            Shuffle(pool);

            int count = Mathf.Min(cardsToDisplay, pool.Count);
            List<UpgradeData> selected = new List<UpgradeData>(count);

            for (int i = 0; i < count; i++)
            {
                selected.Add(pool[i]);
            }

            previousSelection.Clear();
            previousSelection.AddRange(selected);
            return selected;
        }

        public void ApplyUpgrade(UpgradeData data)
        {
            HideOpenUpgradeUI();

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

        private static void HideOpenUpgradeUI()
        {
            UpgradeUIController[] controllers = FindObjectsByType<UpgradeUIController>(FindObjectsInactive.Include);
            foreach (UpgradeUIController controller in controllers)
            {
                if (controller != null) controller.HideUpgradeSelection();
            }
        }

        private void PopulateUpgradesIfNeeded()
        {
            if (!autoPopulateUpgrades) return;
            if (allUpgrades == null) allUpgrades = new List<UpgradeData>();
            allUpgrades.RemoveAll(upgrade => upgrade == null);

#if UNITY_EDITOR
            string[] guids = AssetDatabase.FindAssets("t:UpgradeData", new[] { "Assets/_Project/Data/Upgrades" });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                UpgradeData upgrade = AssetDatabase.LoadAssetAtPath<UpgradeData>(path);
                if (upgrade != null && !allUpgrades.Contains(upgrade))
                {
                    allUpgrades.Add(upgrade);
                }
            }
#endif

            if (allUpgrades.Count == 0)
            {
                Debug.LogWarning("[UpgradeManager] Auto-populate found no UpgradeData assets.");
            }
        }

        private static void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public void ResumeAfterUpgradeSelection()
        {
            Time.timeScale = 1f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (GameManager.Instance == null) return;

            if (RoundManager.Instance == null)
            {
                GameManager.Instance.StartRound();
                return;
            }

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
