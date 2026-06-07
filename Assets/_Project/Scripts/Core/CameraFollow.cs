using System.Collections;
using UnityEngine;

namespace LightNShadowSurvivor
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 localOffset = new Vector3(0, 1.8f, -4.5f);
        
        [Header("Smoothing")]
        [SerializeField] private float positionSmoothTime = 0.3f;
        [SerializeField] private float rotationSmoothTime = 0.25f;
        [SerializeField] private float pitchOffset = 10f;

        private Vector3 currentVelocity = Vector3.zero;
        private float currentRotationVelocity;

        private void LateUpdate()
        {
            if (target == null) return;

            // 1. Calculate desired position in world space
            Vector3 desiredPosition = target.TransformPoint(localOffset);
            
            // 2. Smoothly move camera using SmoothDamp (Better than Lerp for cameras)
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, positionSmoothTime);

            // 3. Smoothly rotate Y angle using SmoothDampAngle to avoid jitter
            float currentAngle = transform.eulerAngles.y;
            float targetAngle = target.eulerAngles.y;
            
            float smoothedAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref currentRotationVelocity, rotationSmoothTime);

            // 4. Apply rotation with fixed pitch and smoothed yaw
            transform.rotation = Quaternion.Euler(pitchOffset, smoothedAngle, 0);
        }
        
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public void SetCameraSettings(Vector3 newLocalOffset, float newPitch)
        {
            localOffset = newLocalOffset;
            pitchOffset = newPitch;
        }

        /// <summary>
        /// Smoothly animates the camera's local offset and pitch over time so it
        /// pulls back from the target to reveal the whole map (used in the ending).
        /// </summary>
        public void ZoomOutCinematic(Vector3 targetOffset, float targetPitch, float duration)
        {
            StopAllCoroutines();
            StartCoroutine(ZoomOutRoutine(targetOffset, targetPitch, Mathf.Max(0.01f, duration)));
        }

        private IEnumerator ZoomOutRoutine(Vector3 targetOffset, float targetPitch, float duration)
        {
            Vector3 startOffset = localOffset;
            float startPitch = pitchOffset;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));
                localOffset = Vector3.Lerp(startOffset, targetOffset, k);
                pitchOffset = Mathf.Lerp(startPitch, targetPitch, k);
                yield return null;
            }

            localOffset = targetOffset;
            pitchOffset = targetPitch;
        }
    }
}
