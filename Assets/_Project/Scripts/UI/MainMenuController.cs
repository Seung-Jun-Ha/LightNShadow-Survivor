using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LightNShadowSurvivor
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private Button startButton;
        [SerializeField] private Button quitButton;

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += HandleStateChanged;
                HandleStateChanged(GameManager.Instance.CurrentState);
            }

            if (startButton != null)
                startButton.onClick.AddListener(OnStartClicked);

            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitClicked);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state)
        {
            Debug.Log($"[MainMenuController] State changed to: {state}");
            
            if (mainMenuPanel != null)
            {
                bool isMenu = (state == GameState.MainMenu);
                mainMenuPanel.SetActive(isMenu);
                Debug.Log($"[MainMenuController] MainMenuPanel visibility set to: {isMenu}");
            }
            
            if (state == GameState.MainMenu)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0f; // Ensure paused in menu
            }
        }

        private void OnStartClicked()
        {
            Debug.Log("[MainMenuController] Start Button Clicked!");
            Time.timeScale = 1f;
            var gm = GameManager.Instance;
            if (gm == null) gm = Object.FindAnyObjectByType<GameManager>();

            if (gm != null)
            {
                gm.StartRound();
            }
            else
            {
                Debug.LogError("[MainMenuController] GameManager not found in scene!");
            }
        }

        private void OnQuitClicked()
        {
            Debug.Log("[MainMenu] Quit Game clicked.");
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
