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

        public void Setup(UpgradeData data, Action<UpgradeData> callback)
        {
            currentData = data;
            onSelected = callback;

            if (nameText != null) nameText.text = data.upgradeName;
            if (descriptionText != null) descriptionText.text = data.description;
            if (iconImage != null && data.icon != null) iconImage.sprite = data.icon;

            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(() => onSelected?.Invoke(currentData));
            }
        }
    }
}
