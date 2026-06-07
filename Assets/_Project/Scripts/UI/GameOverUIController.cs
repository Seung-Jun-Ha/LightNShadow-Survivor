using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace LightNShadowSurvivor
{
    public class GameOverUIController : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private TextMeshProUGUI wastedText;
        private bool retryListenerBound;
        private bool quitListenerBound;

        private void Start()
        {
            ResolveButtons();
            EnsureEventSystem();

            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            BindButtonListeners();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += HandleStateChanged;
                HandleStateChanged(GameManager.Instance.CurrentState);
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
                    ResolveButtons();
                    BindButtonListeners();
                    EnsureEventSystem();
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
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
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

        private void ResolveButtons()
        {
            if (retryButton != null && quitButton != null) return;

            foreach (Button button in FindObjectsByType<Button>(FindObjectsInactive.Include))
            {
                string lowerName = button.gameObject.name.ToLowerInvariant();
                if (retryButton == null && (lowerName.Contains("retry") || lowerName.Contains("restart")))
                {
                    retryButton = button;
                }
                else if (quitButton == null && lowerName.Contains("quit"))
                {
                    quitButton = button;
                }
            }
        }

        private void BindButtonListeners()
        {
            if (retryButton != null && !retryListenerBound)
            {
                retryButton.onClick.AddListener(RetryGame);
                retryListenerBound = true;
            }

            if (quitButton != null && !quitListenerBound)
            {
                quitButton.onClick.AddListener(QuitGame);
                quitListenerBound = true;
            }
        }

        private static void EnsureEventSystem()
        {
            GameObject eventSystemObject;
            if (EventSystem.current != null)
            {
                eventSystemObject = EventSystem.current.gameObject;
                eventSystemObject.SetActive(true);
            }
            else
            {
                eventSystemObject = new GameObject("RuntimeEventSystem", typeof(EventSystem));
            }

            BaseInputModule module = eventSystemObject.GetComponent<BaseInputModule>();
            if (module != null)
            {
                module.enabled = true;
                return;
            }

#if ENABLE_INPUT_SYSTEM
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
#else
            eventSystemObject.AddComponent<StandaloneInputModule>();
#endif
        }
    }
}
