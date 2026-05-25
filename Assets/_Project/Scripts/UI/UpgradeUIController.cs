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

            upgradePanel.SetActive(false);
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
                upgradePanel.SetActive(false);
                ClearCards();
            }
        }

        private void ShowUpgradeSelection()
        {
            upgradePanel.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            ClearCards();

            if (UpgradeManager.Instance != null)
            {
                var upgrades = UpgradeManager.Instance.GetRandomUpgrades();
                foreach (var data in upgrades)
                {
                    GameObject cardObj = Instantiate(cardPrefab, cardContainer);
                    spawnedCards.Add(cardObj);

                    if (cardObj.TryGetComponent<UpgradeCardUI>(out var cardUI))
                    {
                        cardUI.Setup(data, (selectedData) => {
                            UpgradeManager.Instance.ApplyUpgrade(selectedData);
                        });
                    }
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
