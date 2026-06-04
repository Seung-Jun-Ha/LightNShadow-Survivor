using UnityEngine;
using UnityEngine.InputSystem;

namespace LightNShadowSurvivor
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float turnSpeed = 100f; // Adjusted for degrees per second

        [Header("Collision Settings")]
        [SerializeField] private float capsuleHeight = 1.8f;
        [SerializeField] private float capsuleRadius = 0.35f;

        public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

        public void IncreaseMoveSpeedPercent(float percent)
        {
            moveSpeed *= 1f + Mathf.Max(0f, percent);
        }

        private Rigidbody rb;
        private CapsuleCollider capsuleCollider;
        private Animator animator;
        private InputAction moveAction;
        private Vector2 moveInput;

        public static PlayerController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            rb = GetComponent<Rigidbody>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

            capsuleCollider = GetComponent<CapsuleCollider>();
            if (capsuleCollider == null) capsuleCollider = gameObject.AddComponent<CapsuleCollider>();

            animator = GetComponentInChildren<Animator>();
            if (animator != null)
            {
                animator.applyRootMotion = false;
            }

            ConfigureCollisionBody();

            // Lock rotation to prevent physics spinning, but we will control Y rotation manually
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
            }
        }

        private void ConfigureCollisionBody()
        {
            if (capsuleCollider == null) return;

            capsuleCollider.isTrigger = false;
            capsuleCollider.direction = 1;
            capsuleCollider.height = Mathf.Max(capsuleCollider.height, capsuleHeight);
            capsuleCollider.radius = Mathf.Max(capsuleCollider.radius, capsuleRadius);

            float expectedCenterY = capsuleCollider.height * 0.5f;
            if (capsuleCollider.center.y < expectedCenterY * 0.5f)
            {
                capsuleCollider.center = new Vector3(capsuleCollider.center.x, expectedCenterY, capsuleCollider.center.z);
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

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
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
