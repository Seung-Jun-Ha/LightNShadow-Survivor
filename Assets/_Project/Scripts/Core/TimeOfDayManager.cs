using UnityEngine;
using UnityEngine.Rendering;
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

        if (nightVolume) nightVolume.weight = 0.55f;
        if (morningVolume) morningVolume.weight = 0f;

        if (directionalLight)
        {
            directionalLight.color = nightColor;
            directionalLight.intensity = 0.39f;
        }

        RenderSettings.ambientIntensity = 0.27f;
        RenderSettings.fog = true;
        RenderSettings.fogDensity = 0.021f;
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
