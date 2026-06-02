using UnityEngine;
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

            // 1. Disable Player Movement/Aiming
            var player = GameObject.Find("Player_Main");
            if (player != null)
            {
                if (player.TryGetComponent(out PlayerController pc)) pc.enabled = false;
                if (player.TryGetComponent(out PlayerAim pa)) pa.enabled = false;
                var fla = player.GetComponentInChildren<FlashLightAttack>();
                if (fla != null) fla.enabled = false;
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

            // 5. Show Result
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
    }
}



