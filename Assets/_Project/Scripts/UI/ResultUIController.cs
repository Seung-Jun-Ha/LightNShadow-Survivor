using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LightNShadowSurvivor
{
    public class ResultUIController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI killText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Button restartButton;

        private void Start()
        {
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartGame);
            }
        }

        private void OnEnable()
        {
            UpdateStats();
        }

        public void UpdateStats()
        {
            if (GameStatsManager.Instance != null)
            {
                if (killText != null) killText.text = $"Ghosts Banished: {GameStatsManager.Instance.Kills}";
                
                float time = GameStatsManager.Instance.TimeSurvived;
                int minutes = (int)time / 60;
                int seconds = (int)time % 60;
                if (timeText != null) timeText.text = $"Time Survived: {minutes:00}:{seconds:00}";
                
                if (levelText != null) levelText.text = $"Level Reached: {GameStatsManager.Instance.LevelReached}";
            }
        }

        private void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
