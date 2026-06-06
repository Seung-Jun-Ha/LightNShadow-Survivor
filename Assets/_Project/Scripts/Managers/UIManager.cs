using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace LightNShadowSurvivor
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("HUD Elements")]
        [SerializeField] private GameObject gameUIPanel;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Slider hpBar;
        [SerializeField] private Slider expBar;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI roundText;

        [Header("Popups")]
        [SerializeField] private GameObject levelUpPopup;
        [SerializeField] private List<Button> upgradeButtons;

        private PlayerHealth playerHealth;
        private PlayerExperience playerExp;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);

            if (gameUIPanel != null) gameUIPanel.SetActive(false);
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += HandleStateChanged;
                HandleStateChanged(GameManager.Instance.CurrentState);
            }

            // Force HUD inactive at start if in MainMenu
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.MainMenu)
            {
                if (gameUIPanel != null) gameUIPanel.SetActive(false);
            }

            if (levelUpPopup != null) levelUpPopup.SetActive(false);
            
            StartCoroutine(InitializePlayerRefs());
        }

        private IEnumerator InitializePlayerRefs()
        {
            while (PlayerController.Instance == null)
            {
                yield return null;
            }

            playerHealth = PlayerController.Instance.GetComponent<PlayerHealth>();
            playerExp = PlayerController.Instance.GetComponent<PlayerExperience>();

            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged += UpdateHP;
                UpdateHP(playerHealth.CurrentHealth);
            }

            if (playerExp != null)
            {
                playerExp.OnXPChanged += UpdateExp;
                UpdateExp(playerExp.CurrentXP, playerExp.XPToNextLevel);
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged -= HandleStateChanged;

            if (playerHealth != null) playerHealth.OnHealthChanged -= UpdateHP;
            if (playerExp != null) playerExp.OnXPChanged -= UpdateExp;

            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Update()
        {
            if (RoundManager.Instance != null)
            {
                UpdateTimer(RoundManager.Instance.TimeRemaining);
                UpdateRound(RoundManager.Instance.CurrentRound);
            }

            if (playerExp != null)
            {
                UpdateLevel(playerExp.CurrentLevel);
            }
        }

        private void HandleStateChanged(GameState state)
        {
            Debug.Log($"[UIManager] Handling state: {state}");
            
            if (levelUpPopup != null)
            {
                levelUpPopup.SetActive(state == GameState.Upgrade);
            }
            
            if (gameUIPanel != null)
            {
                // Only show HUD during Round or Upgrade
                bool showHUD = state == GameState.Round || state == GameState.Upgrade;
                gameUIPanel.SetActive(showHUD);
                Debug.Log($"[UIManager] HUD visibility set to: {showHUD}");
            }
        }

        public void UpdateTimer(float time)
        {
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(time / 60f);
                int seconds = Mathf.FloorToInt(time % 60f);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
        }

        public void UpdateRound(int round)
        {
            if (roundText != null)
            {
                roundText.text = $"ROUND {round}";
            }
        }

        public void UpdateHP(float currentHealth)
        {
            if (hpBar != null && playerHealth != null)
            {
                hpBar.maxValue = playerHealth.MaxHealth;
                hpBar.value = currentHealth;
            }
        }

        public void UpdateExp(float currentExp, float maxExp)
        {
            if (expBar != null)
            {
                expBar.maxValue = maxExp;
                expBar.value = currentExp;
            }
        }

        public void UpdateLevel(int level)
        {
            if (levelText != null)
            {
                levelText.text = $"LV. {level}";
            }
        }
    }
}
