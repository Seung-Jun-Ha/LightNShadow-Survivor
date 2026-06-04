using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace LightNShadowSurvivor
{
    public class UpgradeCardUI : MonoBehaviour
    {
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI descriptionText;
        public Image iconImage;
        public Button selectButton;

        private UpgradeData currentData;
        private Action<UpgradeData> onSelected;
        private bool hasSelected;

        public void Setup(UpgradeData data, Action<UpgradeData> callback)
        {
            currentData = data;
            onSelected = callback;
            hasSelected = false;

            if (data == null)
            {
                if (nameText != null) nameText.text = "Unavailable";
                if (descriptionText != null) descriptionText.text = string.Empty;
                if (iconImage != null) iconImage.enabled = false;
                if (selectButton != null) selectButton.interactable = false;
                return;
            }

            if (nameText != null) nameText.text = data.upgradeName;
            if (descriptionText != null) descriptionText.text = data.description;
            if (iconImage != null)
            {
                iconImage.enabled = data.icon != null;
                if (data.icon != null) iconImage.sprite = data.icon;
            }

            if (selectButton != null)
            {
                selectButton.interactable = true;
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(Select);
            }
        }

        private void Select()
        {
            if (hasSelected || currentData == null) return;

            hasSelected = true;
            if (selectButton != null) selectButton.interactable = false;
            onSelected?.Invoke(currentData);
        }

        private void OnDisable()
        {
            if (selectButton != null) selectButton.onClick.RemoveListener(Select);
        }
    }
}
