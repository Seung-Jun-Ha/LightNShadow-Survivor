using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace LightNShadowSurvivor
{
    public class UpgradeUIController : MonoBehaviour
    {
        [SerializeField] private GameObject upgradePanel;
        [SerializeField] private Transform cardContainer;
        [SerializeField] private GameObject cardPrefab;

        private List<GameObject> spawnedCards = new List<GameObject>();

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += HandleStateChanged;
            }

            if (upgradePanel != null) upgradePanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.Upgrade)
            {
                ShowUpgradeSelection();
            }
            else
            {
                if (upgradePanel != null) upgradePanel.SetActive(false);
                ClearCards();
            }
        }

        private void ShowUpgradeSelection()
        {
            if (upgradePanel == null)
            {
                Debug.LogWarning("[UpgradeUIController] Upgrade panel is not assigned.");
                return;
            }

            if (cardContainer == null || cardPrefab == null)
            {
                Debug.LogWarning("[UpgradeUIController] Card container or card prefab is not assigned.");
                upgradePanel.SetActive(false);
                return;
            }

            upgradePanel.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            ClearCards();

            if (UpgradeManager.Instance == null)
            {
                Debug.LogWarning("[UpgradeUIController] UpgradeManager instance was not found.");
                upgradePanel.SetActive(false);
                return;
            }

            var upgrades = UpgradeManager.Instance.GetRandomUpgrades();
            if (upgrades.Count == 0)
            {
                Debug.LogWarning("[UpgradeUIController] No upgrades available. Resuming round.");
                UpgradeManager.Instance.ResumeAfterUpgradeSelection();
                return;
            }

            foreach (var data in upgrades)
            {
                GameObject cardObj = Instantiate(cardPrefab, cardContainer);
                spawnedCards.Add(cardObj);

                if (cardObj.TryGetComponent<UpgradeCardUI>(out var cardUI))
                {
                    cardUI.Setup(data, selectedData => UpgradeManager.Instance.ApplyUpgrade(selectedData));
                }
                else
                {
                    Debug.LogWarning($"[UpgradeUIController] Upgrade card prefab {cardPrefab.name} has no UpgradeCardUI component.");
                }
            }
        }

        private void ClearCards()
        {
            foreach (var card in spawnedCards)
            {
                if (card != null) Destroy(card);
            }
            spawnedCards.Clear();
        }
    }
}
