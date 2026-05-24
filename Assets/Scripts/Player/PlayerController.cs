using UnityEngine;
using UnityEngine.InputSystem;

namespace LightNShadowSurvivor
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float turnSpeed = 100f; // Adjusted for degrees per second

        public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

        private Rigidbody rb;
        private Animator animator;
        private InputAction moveAction;
        private Vector2 moveInput;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            animator = GetComponentInChildren<Animator>();
            if (animator != null)
            {
                animator.applyRootMotion = false;
            }

            // Lock rotation to prevent physics spinning, but we will control Y rotation manually
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
            }
        }

        private void OnEnable()
        {
            if (InputSystem.actions != null)
            {
                InputSystem.actions.Enable();
                moveAction = InputSystem.actions.FindAction("Move");
            }
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void OnDisable()
        {
            if (moveAction != null) moveAction.Disable();
        }

        private void Update()
        {
            if (moveAction != null) moveInput = moveAction.ReadValue<Vector2>();
            
            UpdateAnimations();
        }

        private void FixedUpdate()
        {
            HandleRotation();
            Move();
            
            if (rb != null)
            {
                rb.angularVelocity = Vector3.zero;
            }
        }

        private void HandleRotation()
        {
            // A/D (moveInput.x) controls rotation
            if (Mathf.Abs(moveInput.x) > 0.01f)
            {
                float rotation = moveInput.x * turnSpeed * Time.fixedDeltaTime;
                rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotation, 0));
            }
        }

        private void Move()
        {
            // W/S (moveInput.y) controls forward/backward movement
            Vector3 move = transform.forward * moveInput.y;
            Vector3 targetVelocity = move * moveSpeed;
            targetVelocity.y = rb.linearVelocity.y;
            rb.linearVelocity = targetVelocity;
        }

        private void UpdateAnimations()
        {
            if (animator != null)
            {
                // Animation based on forward movement
                animator.SetFloat("MoveSpeed", Mathf.Abs(moveInput.y));
            }
        }
    }
}

