using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LightNShadowSurvivor
{
    public class UpgradeUIController : MonoBehaviour
    {
        [SerializeField] private GameObject upgradePanel;
        [SerializeField] private Transform cardContainer;
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private bool useSteampunkSkillWindow = true;

        private List<GameObject> spawnedCards = new List<GameObject>();
        private bool isShowing;
        private bool isResolvingSelection;
        private Canvas runtimeCanvas;

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += HandleStateChanged;
                HandleStateChanged(GameManager.Instance.CurrentState);
            }

            if (upgradePanel != null && (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Upgrade))
            {
                upgradePanel.SetActive(false);
            }
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
                isResolvingSelection = false;
                ShowForCurrentUpgrade();
            }
            else
            {
                isShowing = false;
                isResolvingSelection = false;
                if (upgradePanel != null) upgradePanel.SetActive(false);
                ClearCards();
            }
        }

        private void ShowUpgradeSelection()
        {
            if (isShowing || isResolvingSelection) return;
            EnsureUpgradeUI();
            EnsureEventSystem();

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

            Time.timeScale = 0f;
            upgradePanel.SetActive(true);
            isShowing = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            ClearCards();

            if (UpgradeManager.Instance == null)
            {
                Debug.LogWarning("[UpgradeUIController] UpgradeManager instance was not found.");
                HideUpgradeSelection();
                return;
            }

            var upgrades = UpgradeManager.Instance.GetRandomUpgrades();
            if (upgrades.Count == 0)
            {
                Debug.LogWarning("[UpgradeUIController] No upgrades available. Resuming round.");
                HideUpgradeSelection();
                UpgradeManager.Instance.ResumeAfterUpgradeSelection();
                return;
            }

            foreach (var data in upgrades)
            {
                GameObject cardObj = Instantiate(cardPrefab, cardContainer);
                cardObj.SetActive(true);
                spawnedCards.Add(cardObj);

                if (cardObj.TryGetComponent<UpgradeCardUI>(out var cardUI))
                {
                    cardUI.Setup(data, SelectUpgrade);
                }
                else
                {
                    Debug.LogWarning($"[UpgradeUIController] Upgrade card prefab {cardPrefab.name} has no UpgradeCardUI component.");
                }
            }
        }

        public void ShowForCurrentUpgrade()
        {
            ShowUpgradeSelection();
        }

        public void HideUpgradeSelection()
        {
            isShowing = false;
            if (upgradePanel != null) upgradePanel.SetActive(false);
            ClearCards();

            if (runtimeCanvas != null)
            {
                DestroyRuntimeObject(runtimeCanvas.gameObject);
                runtimeCanvas = null;
                upgradePanel = null;
                cardContainer = null;
                cardPrefab = null;
            }

            TimeOfDayManager timeOfDay = FindAnyObjectByType<TimeOfDayManager>();
            if (timeOfDay != null) timeOfDay.RestorePlayableLighting();
        }

        public static void HideAllOpenUpgradeUI()
        {
            foreach (UpgradeUIController controller in FindObjectsByType<UpgradeUIController>(FindObjectsInactive.Include))
            {
                if (controller != null) controller.HideUpgradeSelection();
            }

            DestroyNamedRuntimeObject("RuntimeSkillSceneCanvas");
            DestroyNamedRuntimeObject("RuntimeUpgradeUIController");
            DestroyNamedRuntimeObject("SteampunkSkillPanel");
        }

        private void SelectUpgrade(UpgradeData selectedData)
        {
            if (isResolvingSelection) return;

            isResolvingSelection = true;
            HideUpgradeSelection();

            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.ApplyUpgrade(selectedData);
            }
            else
            {
                Time.timeScale = 1f;
                Debug.LogWarning("[UpgradeUIController] UpgradeManager instance was not found while applying selected upgrade.");
            }
        }

        private void EnsureUpgradeUI()
        {
            if (!useSteampunkSkillWindow && upgradePanel != null && cardContainer != null && cardPrefab != null) return;
            if (useSteampunkSkillWindow && runtimeCanvas != null && upgradePanel != null && cardContainer != null && cardPrefab != null) return;

            if (useSteampunkSkillWindow)
            {
                if (upgradePanel != null) upgradePanel.SetActive(false);
                upgradePanel = null;
                cardContainer = null;
                cardPrefab = null;
            }

            if (runtimeCanvas == null)
            {
                GameObject canvasObject = new GameObject("RuntimeSkillSceneCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                runtimeCanvas = canvasObject.GetComponent<Canvas>();
                runtimeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                runtimeCanvas.sortingOrder = 200;

                CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920f, 1080f);
                scaler.matchWidthOrHeight = 0.5f;
            }

            if (upgradePanel == null)
            {
                upgradePanel = new GameObject("SteampunkSkillPanel", typeof(RectTransform), typeof(Image));
                upgradePanel.transform.SetParent(runtimeCanvas.transform, false);

                RectTransform panelRect = upgradePanel.GetComponent<RectTransform>();
                panelRect.anchorMin = Vector2.zero;
                panelRect.anchorMax = Vector2.one;
                panelRect.offsetMin = Vector2.zero;
                panelRect.offsetMax = Vector2.zero;

                Image background = upgradePanel.GetComponent<Image>();
                background.color = new Color(0.035f, 0.025f, 0.018f, 0.42f);

                CreateText("SkillTitle", upgradePanel.transform, "SELECT SKILL", 70, new Vector2(0.5f, 1f), new Vector2(0f, -90f), new Vector2(860f, 110f));
            }

            if (cardContainer == null)
            {
                GameObject container = new GameObject("SteampunkSkillCardContainer", typeof(RectTransform), typeof(HorizontalLayoutGroup));
                container.transform.SetParent(upgradePanel.transform, false);
                cardContainer = container.transform;

                RectTransform rect = container.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = new Vector2(0f, -20f);
                rect.sizeDelta = new Vector2(1260f, 430f);

                HorizontalLayoutGroup layout = container.GetComponent<HorizontalLayoutGroup>();
                layout.spacing = 32f;
                layout.padding = new RectOffset(20, 20, 20, 20);
                layout.childAlignment = TextAnchor.MiddleCenter;
                layout.childForceExpandWidth = false;
                layout.childForceExpandHeight = false;
            }

            if (cardPrefab == null)
            {
                cardPrefab = CreateRuntimeCardPrefab();
            }
        }

        private GameObject CreateRuntimeCardPrefab()
        {
            GameObject card = new GameObject("RuntimeSteampunkSkillCard", typeof(RectTransform), typeof(Image), typeof(Button));
            card.SetActive(false);
            RectTransform cardRect = card.GetComponent<RectTransform>();
            cardRect.sizeDelta = new Vector2(360f, 360f);

            Image cardBackground = card.GetComponent<Image>();
            cardBackground.color = new Color(0.12f, 0.08f, 0.045f, 0.96f);
            cardBackground.raycastTarget = true;

            Button cardButton = card.GetComponent<Button>();
            cardButton.targetGraphic = cardBackground;

            card.AddComponent<LayoutElement>().preferredWidth = 360f;
            card.GetComponent<LayoutElement>().preferredHeight = 360f;

            GameObject frame = CreateSteampunkFrame("SteampunkFrameVisual");
            frame.transform.SetParent(card.transform, false);
            RectTransform frameRect = frame.GetComponent<RectTransform>();
            if (frameRect != null)
            {
                frameRect.anchorMin = Vector2.zero;
                frameRect.anchorMax = Vector2.one;
                frameRect.offsetMin = Vector2.zero;
                frameRect.offsetMax = Vector2.zero;
            }
            SetRaycastTargets(frame, false);

            TextMeshProUGUI nameText = CreateText("NameText", card.transform, "Skill", 39, new Vector2(0.5f, 1f), new Vector2(0f, -60f), new Vector2(320f, 72f));
            TextMeshProUGUI descText = CreateText("DescriptionText", card.transform, "Description", 29, new Vector2(0.5f, 0.56f), Vector2.zero, new Vector2(300f, 170f));
            descText.enableWordWrapping = true;

            GameObject buttonObject = CreateSteampunkButton("SelectButton", card.transform);
            Button button = buttonObject.GetComponent<Button>();

            UpgradeCardUI cardUI = card.AddComponent<UpgradeCardUI>();
            cardUI.nameText = nameText;
            cardUI.descriptionText = descText;
            cardUI.selectButton = button;
            cardUI.additionalSelectButtons = new[] { cardButton };

            return card;
        }

        private GameObject CreateSteampunkFrame(string name)
        {
#if UNITY_EDITOR
            GameObject framePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ThirdParty/Gentleland/SteampunkUI/Prefabs/Frames/Frame 1.prefab");
            if (framePrefab != null)
            {
                GameObject frame = Instantiate(framePrefab);
                frame.name = name;
                return frame;
            }
#endif
            GameObject fallback = new GameObject(name, typeof(RectTransform), typeof(Image));
            fallback.GetComponent<Image>().color = new Color(0.18f, 0.13f, 0.08f, 0.96f);
            return fallback;
        }

        private GameObject CreateSteampunkButton(string name, Transform parent)
        {
#if UNITY_EDITOR
            GameObject buttonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ThirdParty/Gentleland/SteampunkUI/Prefabs/Buttons/Button 3.prefab");
            if (buttonPrefab != null)
            {
                GameObject steampunkButton = Instantiate(buttonPrefab, parent, false);
                steampunkButton.name = name;
                RectTransform rect = steampunkButton.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.5f, 0f);
                rect.anchorMax = new Vector2(0.5f, 0f);
                rect.anchoredPosition = new Vector2(0f, 60f);
                rect.sizeDelta = new Vector2(220f, 72f);
                Image buttonImage = steampunkButton.GetComponent<Image>();
                if (buttonImage == null) buttonImage = steampunkButton.AddComponent<Image>();
                buttonImage.raycastTarget = true;

                Button button = steampunkButton.GetComponent<Button>();
                if (button == null) button = steampunkButton.AddComponent<Button>();
                button.targetGraphic = buttonImage;

                SetButtonText(steampunkButton.transform, "SELECT");
                SetChildRaycastTargetsExceptRoot(steampunkButton, false);
                return steampunkButton;
            }
#endif
            GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.5f, 0f);
            buttonRect.anchorMax = new Vector2(0.5f, 0f);
            buttonRect.anchoredPosition = new Vector2(0f, 60f);
            buttonRect.sizeDelta = new Vector2(220f, 72f);
            buttonObject.GetComponent<Image>().color = new Color(0.38f, 0.25f, 0.11f, 1f);
            Button fallbackButton = buttonObject.GetComponent<Button>();
            fallbackButton.targetGraphic = buttonObject.GetComponent<Image>();
            CreateText("Label", buttonObject.transform, "SELECT", 31, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(190f, 58f));
            return buttonObject;
        }

        private static void EnsureEventSystem()
        {
            if (EventSystem.current != null)
            {
                EventSystem.current.gameObject.SetActive(true);
                EnsureInputModule(EventSystem.current.gameObject);
                return;
            }

            GameObject eventSystemObject = new GameObject("RuntimeEventSystem", typeof(EventSystem));
            EnsureInputModule(eventSystemObject);
            Debug.Log("[UpgradeUIController] Created RuntimeEventSystem for skill selection clicks.");
        }

        private static void EnsureInputModule(GameObject eventSystemObject)
        {
            BaseInputModule inputModule = eventSystemObject.GetComponent<BaseInputModule>();
            if (inputModule != null)
            {
                inputModule.enabled = true;
                return;
            }

#if ENABLE_INPUT_SYSTEM
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
#else
            eventSystemObject.AddComponent<StandaloneInputModule>();
#endif
        }

        private static TextMeshProUGUI CreateText(string name, Transform parent, string text, float fontSize, Vector2 anchor, Vector2 position, Vector2 size)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(parent, false);

            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            TextMeshProUGUI label = textObject.GetComponent<TextMeshProUGUI>();
            label.text = text;
            label.fontSize = fontSize;
            label.alignment = TextAlignmentOptions.Center;
            label.color = new Color(0.95f, 0.78f, 0.42f, 1f);
            label.fontStyle |= FontStyles.Bold;
            label.raycastTarget = false;
            return label;
        }

        private static void SetRaycastTargets(GameObject root, bool enabled)
        {
            foreach (Graphic graphic in root.GetComponentsInChildren<Graphic>(true))
            {
                graphic.raycastTarget = enabled;
            }
        }

        private static void SetChildRaycastTargetsExceptRoot(GameObject root, bool enabled)
        {
            Graphic rootGraphic = root.GetComponent<Graphic>();
            foreach (Graphic graphic in root.GetComponentsInChildren<Graphic>(true))
            {
                if (graphic == rootGraphic) continue;
                graphic.raycastTarget = enabled;
            }
        }

        private static void SetButtonText(Transform root, string text)
        {
            TextMeshProUGUI tmp = root.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp != null)
            {
                tmp.text = text;
                return;
            }

            Text legacy = root.GetComponentInChildren<Text>(true);
            if (legacy != null) legacy.text = text;
        }

        private void ClearCards()
        {
            for (int i = spawnedCards.Count - 1; i >= 0; i--)
            {
                var card = spawnedCards[i];
                if (card != null) DestroyRuntimeObject(card);
            }
            spawnedCards.Clear();
        }

        private static void DestroyRuntimeObject(Object target)
        {
            if (target == null) return;

            if (Application.isPlaying)
            {
                Destroy(target);
            }
            else
            {
                DestroyImmediate(target);
            }
        }

        private static void DestroyNamedRuntimeObject(string objectName)
        {
            GameObject obj = GameObject.Find(objectName);
            if (obj != null) DestroyRuntimeObject(obj);
        }
    }
}
