using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace LightNShadowSurvivor
{
    public class GameCycleManager : MonoBehaviour
    {
        public static GameCycleManager Instance { get; private set; }

        [Header("Timer Settings")]
        public float stageTime = 60f;
        private float currentTimer;
        private bool isTimerRunning = true;

        [Header("UI References")]
        public GameObject upgradePanel;
        public Transform cardContainer;
        public GameObject upgradeCardPrefab;

        [Header("Upgrade Assets")]
        public List<UpgradeData> allUpgrades;
        
        [Header("Stage Progression")]
        public GameObject[] stageAssets;
        private int currentStageIndex = 0;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);
        }

        private void Start()
        {
            if (RoundManager.Instance != null)
            {
                isTimerRunning = false;
                enabled = false;
                return;
            }

            currentTimer = stageTime;
            if (upgradePanel != null) upgradePanel.SetActive(false);
            
            foreach (var asset in stageAssets) if(asset != null) asset.SetActive(false);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += HandleStateChanged;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged -= HandleStateChanged;

            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.Round)
            {
                isTimerRunning = true;
            }
            else
            {
                isTimerRunning = false;
            }
        }

        private void Update()
        {
            if (!isTimerRunning) return;

            currentTimer -= Time.deltaTime;
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateTimer(currentTimer);
            }

            if (currentTimer <= 0)
            {
                EndStage();
            }
        }

        private void EndStage()
        {
            isTimerRunning = false;
            Time.timeScale = 0f;
            OpenUpgradeUI();
        }

        private void OpenUpgradeUI()
        {
            if (upgradePanel == null || cardContainer == null || upgradeCardPrefab == null) return;

            upgradePanel.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            foreach (Transform child in cardContainer)
            {
                Destroy(child.gameObject);
            }

            List<UpgradeData> selectedUpgrades = GetRandomUpgrades(4);

            foreach (var data in selectedUpgrades)
            {
                GameObject cardObj = Instantiate(upgradeCardPrefab, cardContainer);
                var cardUI = cardObj.GetComponent<UpgradeCardUI>();
                if (cardUI != null)
                {
                    cardUI.Setup(data, OnUpgradeSelected);
                }
            }
        }

        private List<UpgradeData> GetRandomUpgrades(int count)
        {
            List<UpgradeData> pool = new List<UpgradeData>(allUpgrades);
            List<UpgradeData> result = new List<UpgradeData>();

            for (int i = 0; i < count && pool.Count > 0; i++)
            {
                int index = Random.Range(0, pool.Count);
                result.Add(pool[index]);
                pool.RemoveAt(index);
            }
            return result;
        }

        private void OnUpgradeSelected(UpgradeData data)
        {
            if (data == null)
            {
                AdvanceStage();
                ResumeGame();
                return;
            }

            ApplyUpgrade(data);
            AdvanceStage();
            ResumeGame();
        }

        private void ApplyUpgrade(UpgradeData data)
        {
            if (data == null) return;

            GameObject player = GameObject.Find("Player_Main");
            if (player == null) return;

            switch (data.upgradeType)
            {
                case UpgradeType.MoveSpeed:
                    var controller = player.GetComponent<PlayerController>();
                    if (controller != null) controller.IncreaseMoveSpeedPercent(data.increaseValue);
                    break;

                case UpgradeType.Durability:
                    var health = player.GetComponent<PlayerHealth>();
                    if (health != null) health.IncreaseDurability(data.increaseValue);
                    break;

                case UpgradeType.LightIntensity:
                    var lightAttack = player.GetComponentInChildren<FlashLightAttack>();
                    if (lightAttack != null) lightAttack.IncreaseDamage(data.increaseValue);
                    break;

                case UpgradeType.LightRadius:
                    var attack = player.GetComponentInChildren<FlashLightAttack>();
                    if (attack != null) attack.IncreaseDiameterPercent(data.increaseValue);
                    break;
            }
        }

        private void AdvanceStage()
        {
            if (currentStageIndex < stageAssets.Length)
            {
                if (stageAssets[currentStageIndex] != null)
                {
                    stageAssets[currentStageIndex].SetActive(true);
                }
                currentStageIndex++;
            }
        }

        private void ResumeGame()
        {
            if (upgradePanel != null) upgradePanel.SetActive(false);
            currentTimer = stageTime;
            isTimerRunning = true;
            Time.timeScale = 1f;
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ChangeState(GameState.Round);
            }
        }
    }
}
