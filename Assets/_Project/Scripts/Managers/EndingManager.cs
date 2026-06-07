using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace LightNShadowSurvivor
{
    public class EndingManager : MonoBehaviour
    {
        [SerializeField] private Light directionalLight;
        [SerializeField] private float sunriseDuration = 10f;
        private GameObject resultUI;

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += HandleStateChanged;
            }
            
            // Try to find Result_UI if not assigned (it's in UIScene now)
            StartCoroutine(FindResultUIRoutine());
        }

        private IEnumerator FindResultUIRoutine()
        {
            // Wait a bit for UIScene to load
            yield return new WaitForSeconds(0.5f);
            resultUI = GameObject.Find("Result_UI");
            if (resultUI != null) resultUI.SetActive(false);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.Ending)
            {
                StartCoroutine(SunriseRoutine());
            }
        }

        private IEnumerator SunriseRoutine()
        {
            Debug.Log("Starting Sunrise Ending...");

            // 1. Freeze the player completely (movement, aiming, attacking, physics, animation).
            var player = GameObject.Find("Player_Main");
            if (player != null)
            {
                if (player.TryGetComponent(out PlayerController pc)) pc.enabled = false;
                if (player.TryGetComponent(out PlayerAim pa)) pa.enabled = false;
                var fla = player.GetComponentInChildren<FlashLightAttack>();
                if (fla != null) fla.enabled = false;

                if (player.TryGetComponent(out Rigidbody rb))
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                var playerAnimator = player.GetComponentInChildren<Animator>();
                if (playerAnimator != null)
                {
                    foreach (var parameter in playerAnimator.parameters)
                    {
                        if (parameter.type == AnimatorControllerParameterType.Float && parameter.name == "MoveSpeed")
                        {
                            playerAnimator.SetFloat("MoveSpeed", 0f);
                        }
                    }
                }
            }

            // 1b. Cinematic camera pull-back to reveal the whole map during the sunrise.
            var cameraFollow = FindAnyObjectByType<CameraFollow>();
            if (cameraFollow != null)
            {
                cameraFollow.ZoomOutCinematic(new Vector3(0f, 28f, -18f), 60f, sunriseDuration);
            }

            // 2. Stop Spawning
            var spawner = FindAnyObjectByType<MonsterSpawner>();
            if (spawner != null) spawner.StopSpawning();

            // 3. Cleanup all monsters with a delay or dissolve if possible
            var monsters = GameObject.FindObjectsByType<MonsterBase>(FindObjectsInactive.Exclude);
            foreach (var m in monsters)
            {
                // We can't easily trigger dissolve without a specific method, 
                // but we can at least stop their AI
                m.StopBehavior();
                Destroy(m.gameObject, 2f); // Fade out then destroy
            }

            // 4. Sunrise Visuals
            float elapsed = 0f;
            float startIntensity = directionalLight != null ? directionalLight.intensity : 0.1f;
            float targetIntensity = 1.5f;
            
            Color nightFogColor = RenderSettings.fogColor;
            Color dayFogColor = new Color(0.8f, 0.9f, 1.0f); // Bright blue-ish

            while (elapsed < sunriseDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / sunriseDuration;

                if (directionalLight != null)
                {
                    directionalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);
                    directionalLight.color = Color.Lerp(directionalLight.color, Color.white, t);
                }

                RenderSettings.ambientIntensity = Mathf.Lerp(0.5f, 1.2f, t);
                RenderSettings.fogColor = Color.Lerp(nightFogColor, dayFogColor, t);
                RenderSettings.fogDensity = Mathf.Lerp(0.05f, 0.01f, t);

                yield return null;
            }

            // 5. Show the "GAME CLEAR" title, then the results panel.
            yield return StartCoroutine(ShowGameClearText());

            if (resultUI != null)
            {
                resultUI.SetActive(true);
            }
            else
            {
                Debug.LogWarning("[EndingManager] resultUI not found. Searching again...");
                resultUI = GameObject.Find("Result_UI");
                if (resultUI != null) resultUI.SetActive(true);
            }
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private IEnumerator ShowGameClearText()
        {
            GameObject canvasObject = new GameObject("RuntimeGameClearCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 200;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            GameObject textObject = new GameObject("GameClearText", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(canvasObject.transform, false);
            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.5f, 0.5f);
            textRect.anchorMax = new Vector2(0.5f, 0.5f);
            textRect.anchoredPosition = new Vector2(0f, 120f);
            textRect.sizeDelta = new Vector2(1400f, 300f);

            TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
            text.text = "GAME CLEAR";
            text.fontSize = 140f;
            text.fontStyle = FontStyles.Bold;
            text.alignment = TextAlignmentOptions.Center;
            text.color = new Color(1f, 0.95f, 0.7f, 0f);

            // Fade the title in.
            float elapsed = 0f;
            const float fadeDuration = 1.5f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                Color color = text.color;
                color.a = Mathf.Clamp01(elapsed / fadeDuration);
                text.color = color;
                yield return null;
            }

            yield return new WaitForSeconds(1.5f);
        }
    }
}



