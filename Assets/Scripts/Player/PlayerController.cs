using UnityEngine;
using UnityEngine.InputSystem;

namespace LightNShadowSurvivor
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float mouseSensitivity = 0.15f;
        
        private Rigidbody rb;
        private Animator animator;
        private InputAction moveAction;
        private InputAction lookAction;
        private Vector2 moveInput;
        private Vector2 lookInput;
        private float rotationY;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            animator = GetComponentInChildren<Animator>();
        }

        private void OnEnable()
        {
            if (InputSystem.actions != null)
            {
                InputSystem.actions.Enable();
                moveAction = InputSystem.actions.FindAction("Move");
                lookAction = InputSystem.actions.FindAction("Look");
            }
            
            // Lock cursor for better 3rd person control
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnDisable()
        {
            if (moveAction != null) moveAction.Disable();
            if (lookAction != null) lookAction.Disable();
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Start()
        {
            if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            
            rotationY = transform.eulerAngles.y;
        }

        private void Update()
        {
            if (moveAction != null) moveInput = moveAction.ReadValue<Vector2>();
            if (lookAction != null) lookInput = lookAction.ReadValue<Vector2>();
            
            // Handle Rotation
            rotationY += lookInput.x * mouseSensitivity;
            transform.rotation = Quaternion.Euler(0, rotationY, 0);

            UpdateAnimations();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            // Strafe movement: relative to player's current facing
            Vector3 move = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;
            Vector3 targetVelocity = move * moveSpeed;
            targetVelocity.y = rb.linearVelocity.y;
            rb.linearVelocity = targetVelocity;
        }

        private void UpdateAnimations()
        {
            if (animator != null)
            {
                // For animation, we use the magnitude of movement
                animator.SetFloat("MoveSpeed", moveInput.magnitude);
            }
        }
    }
}

