using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace LightNShadowSurvivor
{
    public class GameOverUIController : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Button retryButton;

        private void Start()
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (retryButton != null) retryButton.onClick.AddListener(RetryGame);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += HandleStateChanged;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state)
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(state == GameState.GameOver);
                if (state == GameState.GameOver)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }

        private void RetryGame()
        {
            // Reset timescale before reloading
            Time.timeScale = 1f;
            // Reload current scene (GameScene usually)
            // But we might need to load from the beginning
            SceneManager.LoadScene("GameScene");
        }
    }
}
