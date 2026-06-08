using UnityEngine;
using UnityEngine.UI;

namespace LightNShadowSurvivor
{
    [RequireComponent(typeof(Button))]
    public class ButtonSoundTrigger : MonoBehaviour
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            if (button == null) button = GetComponent<Button>();
            if (button != null) button.onClick.AddListener(PlayClick);
        }

        private void OnDisable()
        {
            if (button != null) button.onClick.RemoveListener(PlayClick);
        }

        private static void PlayClick()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClickSFX();
            }
        }
    }
}
