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
        [SerializeField] private Light aimLight;
        
        [Header("Settings")]
        [SerializeField] private float rotationSmoothSpeed = 15f;
        // Adjusted offsets: 0,0,0 means the bone's forward (Z) points at the target.
        // Since the flashlight is aligned with the bone's Z, we keep these minimal.
        [SerializeField] private Vector3 handRotationOffset = Vector3.zero;
        [SerializeField] private Vector3 forearmRotationOffset = Vector3.zero;
        
        [Header("Aiming Config")]
        [SerializeField] private float aimDistance = 20f;
        [SerializeField] private float verticalLimit = 2.0f;
        [SerializeField] private LayerMask aimSurfaceMask = 0;

        private Vector3 currentTargetPoint;
        private Quaternion currentHandRot;
        private Quaternion currentForearmRot;

        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
            ResolveFlashlightPivot();

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

        private void ResolveFlashlightPivot()
        {
            if (aimLight == null) aimLight = GetComponentInChildren<Light>(true);
            if (flashlightPivot == null && aimLight != null) flashlightPivot = aimLight.transform;
            if (flashlightPivot == null)
            {
                FlashLightAttack attack = GetComponentInChildren<FlashLightAttack>(true);
                if (attack != null) flashlightPivot = attack.transform;
            }
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

            AimFlashlight();
        }

        private void UpdateTargetPoint()
        {
            if (Mouse.current == null) return;
            if (mainCamera == null) mainCamera = Camera.main;
            if (mainCamera == null) return;

            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mousePos);

            if (aimSurfaceMask.value != 0 && Physics.Raycast(ray, out RaycastHit hit, aimDistance * 2f, aimSurfaceMask, QueryTriggerInteraction.Ignore))
            {
                currentTargetPoint = ClampVerticalTarget(hit.point);
                return;
            }

            Plane aimPlane = new Plane(Vector3.up, transform.position + Vector3.up * 0.8f);
            if (aimPlane.Raycast(ray, out float enter))
            {
                currentTargetPoint = ClampVerticalTarget(ray.GetPoint(enter));
            }
            else
            {
                currentTargetPoint = ClampVerticalTarget(ray.GetPoint(aimDistance));
            }
        }

        private Vector3 ClampVerticalTarget(Vector3 targetPoint)
        {
            float limit = Mathf.Max(0f, verticalLimit);
            targetPoint.y = Mathf.Clamp(targetPoint.y, transform.position.y - limit, transform.position.y + limit);
            return targetPoint;
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

        private void AimFlashlight()
        {
            if (flashlightPivot == null) ResolveFlashlightPivot();
            if (flashlightPivot == null) return;

            AimTransformSmoothly(flashlightPivot);
        }

        private void AimTransformSmoothly(Transform target)
        {
            Vector3 direction = currentTargetPoint - target.position;
            if (direction.sqrMagnitude <= 0.0001f) return;

            Quaternion targetRot = Quaternion.LookRotation(direction.normalized, Vector3.up);
            target.rotation = Quaternion.Slerp(target.rotation, targetRot, rotationSmoothSpeed * Time.deltaTime);
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
