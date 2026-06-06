using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class TimeOfDayManager : MonoBehaviour
{
    [Header("Volumes")]
    public Volume nightVolume;
    public Volume morningVolume;

    [Header("Lighting")]
    public Light directionalLight;
    public Color nightColor = new Color(0.25f, 0.45f, 0.75f);
    public Color morningColor = new Color(1.0f, 0.95f, 0.8f);
    public float nightIntensity = 0.3f;
    public float morningIntensity = 1.2f;

    [Header("Transition")]
    public float transitionDuration = 5.0f;
    [SerializeField, Range(0f, 1f)] private float playableNightVolumeWeight = 0.15f;
    [SerializeField] private float playableNightIntensity = 0.65f;

    private bool isMorning = false;

    private void Awake()
    {
        // Initial state
        if (nightVolume) nightVolume.weight = 1f;
        if (morningVolume) morningVolume.weight = 0f;
        if (directionalLight)
        {
            directionalLight.color = nightColor;
            directionalLight.intensity = nightIntensity;
        }
    }

    [ContextMenu("Transition to Morning")]
    public void ClearAllStages()
    {
        if (isMorning) return;
        StartCoroutine(TransitionRoutine());
    }

    public void RestorePlayableLighting()
    {
        StopAllCoroutines();
        isMorning = false;

        if (nightVolume) nightVolume.weight = playableNightVolumeWeight;
        if (morningVolume) morningVolume.weight = 0f;

        if (directionalLight)
        {
            directionalLight.color = nightColor;
            directionalLight.intensity = playableNightIntensity;
        }

        RenderSettings.skybox = null;
        RenderSettings.ambientMode = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.025f, 0.035f, 0.08f, 1f);
        RenderSettings.ambientEquatorColor = new Color(0.018f, 0.022f, 0.045f, 1f);
        RenderSettings.ambientGroundColor = new Color(0.01f, 0.01f, 0.018f, 1f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.015f, 0.018f, 0.035f, 1f);
        RenderSettings.fogDensity = 0.012f;

        foreach (Camera camera in FindObjectsByType<Camera>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.012f, 0.014f, 0.03f, 1f);
            UniversalAdditionalCameraData cameraData = camera.GetUniversalAdditionalCameraData();
            if (cameraData != null) cameraData.renderPostProcessing = false;
        }
    }

    private IEnumerator TransitionRoutine()
    {
        isMorning = true;
        float elapsed = 0;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;

            // Blend Volumes
            if (morningVolume) morningVolume.weight = t;

            // Blend Lighting
            if (directionalLight)
            {
                directionalLight.color = Color.Lerp(nightColor, morningColor, t);
                directionalLight.intensity = Mathf.Lerp(nightIntensity, morningIntensity, t);
            }

            yield return null;
        }

        if (morningVolume) morningVolume.weight = 1f;
        if (directionalLight)
        {
            directionalLight.color = morningColor;
            directionalLight.intensity = morningIntensity;
        }
    }
}
