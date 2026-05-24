using UnityEngine;

namespace LightNShadowSurvivor
{
    public class PlayerAim : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform flashlightPivot;

        private void Update()
        {
            // Flashlight now strictly follows the character's facing (the pivot is attached to the hand)
            // We can optionally keep the pivot rotation at identity relative to the bone
            if (flashlightPivot != null)
            {
                flashlightPivot.localRotation = Quaternion.identity;
            }
        }
    }
}
