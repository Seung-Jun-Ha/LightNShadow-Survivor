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
                if (player.TryGetComponent(out FlashLightAttack fla)) fla.enabled = false;
            }

            // 2. Stop Spawning
            var spawner = FindFirstObjectByType<MonsterSpawner>();
            if (spawner != null) spawner.StopSpawning();

            // 3. Destroy all monsters
            var monsters = GameObject.FindObjectsByType<MonsterBase>(FindObjectsSortMode.None);
            foreach (var m in monsters)
            {
                // In a real game, trigger dissolve here
                Destroy(m.gameObject);
            }

            // 4. Increase Light
            if (directionalLight != null)
            {
                float startIntensity = directionalLight.intensity;
                float targetIntensity = 2.0f;
                float elapsed = 0f;

                while (elapsed < sunriseDuration)
                {
                    elapsed += Time.deltaTime;
                    directionalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, elapsed / sunriseDuration);
                    RenderSettings.ambientIntensity = Mathf.Lerp(0.5f, 1.5f, elapsed / sunriseDuration);
                    yield return null;
                }
            }

            // 5. Show Result
            if (resultUI != null) resultUI.SetActive(true);
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
