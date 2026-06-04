using UnityEngine;
using UnityEngine.UI;

namespace LightNShadowSurvivor
{
    [RequireComponent(typeof(Button))]
    public class ButtonSoundTrigger : MonoBehaviour
    {
        private void Start()
        {
            Button btn = GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(PlaySound);
            }
        }

        private void PlaySound()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClickSFX();
            }
        }
    }
}