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
        public Button[] additionalSelectButtons;

        private UpgradeData currentData;
        private Action<UpgradeData> onSelected;
        private bool hasSelected;

        public void Setup(UpgradeData data, Action<UpgradeData> callback)
        {
            currentData = data;
            onSelected = callback;
            hasSelected = false;

            ApplyTextStyle();

            if (data == null)
            {
                if (nameText != null) nameText.text = "Unavailable";
                if (descriptionText != null) descriptionText.text = string.Empty;
                if (iconImage != null) iconImage.enabled = false;
                if (selectButton != null) selectButton.interactable = false;
                SetAdditionalButtonsInteractable(false);
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

            if (additionalSelectButtons != null)
            {
                foreach (Button button in additionalSelectButtons)
                {
                    if (button == null || button == selectButton) continue;
                    button.interactable = true;
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(Select);
                }
            }
        }

        private void Select()
        {
            if (hasSelected || currentData == null) return;

            hasSelected = true;
            if (selectButton != null) selectButton.interactable = false;
            SetAdditionalButtonsInteractable(false);
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySkillSelectSFX();
            Debug.Log($"[UpgradeCardUI] Selected upgrade: {currentData.upgradeName}");
            onSelected?.Invoke(currentData);
        }

        private void OnDisable()
        {
            if (selectButton != null) selectButton.onClick.RemoveListener(Select);
            if (additionalSelectButtons == null) return;

            foreach (Button button in additionalSelectButtons)
            {
                if (button != null) button.onClick.RemoveListener(Select);
            }
        }

        private void ApplyTextStyle()
        {
            if (nameText != null)
            {
                nameText.fontSize = Mathf.RoundToInt(nameText.fontSize * 1.3f);
                nameText.fontStyle |= FontStyles.Bold;
            }

            if (descriptionText != null)
            {
                descriptionText.fontSize = Mathf.RoundToInt(descriptionText.fontSize * 1.3f);
                descriptionText.fontStyle |= FontStyles.Bold;
            }
        }

        private void SetAdditionalButtonsInteractable(bool interactable)
        {
            if (additionalSelectButtons == null) return;

            foreach (Button button in additionalSelectButtons)
            {
                if (button != null) button.interactable = interactable;
            }
        }
    }
}
