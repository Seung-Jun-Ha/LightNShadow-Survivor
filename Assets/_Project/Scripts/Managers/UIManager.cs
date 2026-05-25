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

        [Header("Popups")]
        [SerializeField] private GameObject levelUpPopup;
        [SerializeField] private List<Button> upgradeButtons;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += HandleStateChanged;
                // Handle initial state
                HandleStateChanged(GameManager.Instance.CurrentState);
            }

            // Initialize UI
            if (gameUIPanel != null) gameUIPanel.SetActive(true);
            if (levelUpPopup != null) levelUpPopup.SetActive(false);
            
            // Try to find player and subscribe to health
            StartCoroutine(InitializePlayerRefs());
        }

        private IEnumerator InitializePlayerRefs()
        {
            GameObject player = null;
            while (player == null)
            {
                player = GameObject.Find("Player_Main");
                yield return null;
            }

            var health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.OnHealthChanged += UpdateHP;
                UpdateHP(health.CurrentHealth);
            }
        }

        private void Update()
        {
            if (RoundManager.Instance != null)
            {
                UpdateTimer(RoundManager.Instance.TimeRemaining);
            }
        }

        private void HandleStateChanged(GameState state)
        {
            if (levelUpPopup != null)
            {
                levelUpPopup.SetActive(state == GameState.Upgrade);
            }
            
            if (gameUIPanel != null)
            {
                // Keep HUD visible during Round and Upgrade
                gameUIPanel.SetActive(state == GameState.Round || state == GameState.Upgrade || state == GameState.Intro);
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

        public void UpdateHP(float currentHealth)
        {
            if (hpBar != null)
            {
                var healthComp = FindFirstObjectByType<PlayerHealth>();
                if (healthComp != null)
                {
                    hpBar.maxValue = healthComp.MaxHealth;
                    hpBar.value = currentHealth;
                }
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
