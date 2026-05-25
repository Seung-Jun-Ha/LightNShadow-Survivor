using UnityEngine;
using UnityEngine.InputSystem;

namespace LightNShadowSurvivor
{
    public class PlayerAim : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform rightHandBone;
        [SerializeField] private Transform rightForearmBone;
        [SerializeField] private Transform flashlightPivot;
        
        [Header("Settings")]
        [SerializeField] private float rotationSmoothSpeed = 15f;
        // Adjusted offsets: 0,0,0 means the bone's forward (Z) points at the target.
        // Since the flashlight is aligned with the bone's Z, we keep these minimal.
        [SerializeField] private Vector3 handRotationOffset = Vector3.zero;
        [SerializeField] private Vector3 forearmRotationOffset = Vector3.zero;
        
        [Header("Aiming Config")]
        [SerializeField] private float aimDistance = 20f;
        [SerializeField] private float verticalLimit = 2.0f;

        private Vector3 currentTargetPoint;
        private Quaternion currentHandRot;
        private Quaternion currentForearmRot;

        private void Start()
        {
            var animator = GetComponentInChildren<Animator>();
            if (animator != null && animator.isHuman)
            {
                if (rightHandBone == null) rightHandBone = animator.GetBoneTransform(HumanBodyBones.RightHand);
                if (rightForearmBone == null) rightForearmBone = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            }
            
            currentTargetPoint = transform.position + transform.forward * aimDistance;
            if (rightHandBone != null) currentHandRot = rightHandBone.rotation;
            if (rightForearmBone != null) currentForearmRot = rightForearmBone.rotation;
        }

        private void LateUpdate()
        {
            UpdateTargetPoint();

            // Aim the arm bones
            if (rightForearmBone != null)
            {
                currentForearmRot = AimBoneSmoothly(rightForearmBone, currentForearmRot, forearmRotationOffset);
                rightForearmBone.rotation = currentForearmRot;
            }

            if (rightHandBone != null)
            {
                currentHandRot = AimBoneSmoothly(rightHandBone, currentHandRot, handRotationOffset);
                rightHandBone.rotation = currentHandRot;
            }

            if (flashlightPivot != null)
            {
                flashlightPivot.localRotation = Quaternion.identity;
            }
        }

        private void UpdateTargetPoint()
        {
            if (Mouse.current == null || Camera.main == null) return;

            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            
            // To ensure it points "forward" and not just at the ground:
            // We use the mouse ray to find a point at a fixed distance from the camera,
            // but we clamp its Y to be roughly at the player's height or higher.
            Vector3 pointAtDistance = ray.GetPoint(aimDistance);
            
            // Limit how low the target can be to prevent pointing at feet
            float minHeight = transform.position.y + 0.5f;
            pointAtDistance.y = Mathf.Max(pointAtDistance.y, minHeight);
            
            currentTargetPoint = pointAtDistance;
        }

        private Quaternion AimBoneSmoothly(Transform bone, Quaternion currentRot, Vector3 offset)
        {
            Vector3 direction = (currentTargetPoint - bone.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction) * Quaternion.Euler(offset);
                return Quaternion.Slerp(currentRot, targetRot, rotationSmoothSpeed * Time.deltaTime);
            }
            return currentRot;
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(currentTargetPoint, 0.5f);
                if (rightHandBone != null) Gizmos.DrawLine(rightHandBone.position, currentTargetPoint);
            }
        }
    }
}
