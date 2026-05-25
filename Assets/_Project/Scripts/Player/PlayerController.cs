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

        public static PlayerController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
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
            // A/D keys for turning (Left/Right rotation)
            if (moveInput.x != 0)
            {
                float rotation = moveInput.x * turnSpeed * Time.fixedDeltaTime;
                rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotation, 0));
            }
        }

        private void Move()
        {
            // W/S keys for Forward/Backward movement relative to character's facing direction
            Vector3 moveDirection = transform.forward * moveInput.y;
            Vector3 targetVelocity = moveDirection * moveSpeed;
            
            // Maintain vertical velocity (gravity/jumping)
            targetVelocity.y = rb.linearVelocity.y;
            rb.linearVelocity = targetVelocity;
        }

        private void UpdateAnimations()
        {
            if (animator != null)
            {
                // Animation based on movement magnitude
                float speed = new Vector2(moveInput.x, moveInput.y).magnitude;
                animator.SetFloat("MoveSpeed", speed);
            }
        }
    }
}

