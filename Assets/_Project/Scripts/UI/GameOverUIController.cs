using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace LightNShadowSurvivor
{
    public class GameOverUIController : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private TextMeshProUGUI wastedText;

        private void Start()
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (retryButton != null) retryButton.onClick.AddListener(RetryGame);
            if (quitButton != null) quitButton.onClick.AddListener(QuitGame);

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
                bool isGameOver = (state == GameState.GameOver);
                gameOverPanel.SetActive(isGameOver);
                
                if (isGameOver)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    StartCoroutine(WastedSequence());
                }
            }
        }

        private IEnumerator WastedSequence()
        {
            if (wastedText != null)
            {
                wastedText.transform.localScale = Vector3.one * 5f;
                wastedText.alpha = 0f;
                
                float elapsed = 0f;
                float duration = 1.5f;
                
                while (elapsed < duration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    float t = elapsed / duration;
                    
                    // GTA-style zoom in
                    wastedText.transform.localScale = Vector3.Lerp(Vector3.one * 3f, Vector3.one, t);
                    wastedText.alpha = Mathf.Clamp01(t * 2f);
                    
                    yield return null;
                }
            }
        }

        private void RetryGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("GameScene");
        }

        private void QuitGame()
        {
            Debug.Log("[GameOver] Quitting to Menu...");
            Time.timeScale = 1f;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ChangeState(GameState.MainMenu);
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
