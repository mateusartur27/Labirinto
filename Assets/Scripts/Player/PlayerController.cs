using UnityEngine;
using UnityEngine.InputSystem;

namespace Labirinto.Player
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActions;

        [SerializeField] private float moveForce = 40f;
        [SerializeField] private float maxSpeed = 6f;
        [SerializeField] private float jumpImpulse = 6f;
        [SerializeField] private float groundCheckRadius = 0.3f;
        [SerializeField] private LayerMask groundMask = ~0;

        private Rigidbody rb;
        private CapsuleCollider capsule;
        private InputAction moveAction;
        private InputAction jumpAction;
        private bool jumpQueued;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            capsule = GetComponent<CapsuleCollider>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;

            var map = inputActions.FindActionMap("Player");
            moveAction = map.FindAction("Move");
            jumpAction = map.FindAction("Jump");
        }

        private void OnEnable()
        {
            moveAction.Enable();
            jumpAction.Enable();
            jumpAction.performed += OnJumpPerformed;
        }

        private void OnDisable()
        {
            jumpAction.performed -= OnJumpPerformed;
            moveAction.Disable();
            jumpAction.Disable();
        }

        private void OnJumpPerformed(InputAction.CallbackContext ctx) => jumpQueued = true;

        private void FixedUpdate()
        {
            Vector2 input = moveAction.ReadValue<Vector2>();

            // Camera is fixed (no mouse orbit), so WASD maps straight onto world axes:
            // W/S = world Z (the corridor direction), A/D = world X.
            Vector3 direction = new Vector3(input.x, 0f, input.y);
            if (direction.sqrMagnitude > 1f) direction.Normalize();

            rb.AddForce(direction * moveForce, ForceMode.Force);

            Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (flatVelocity.magnitude > maxSpeed)
            {
                Vector3 clamped = flatVelocity.normalized * maxSpeed;
                rb.linearVelocity = new Vector3(clamped.x, rb.linearVelocity.y, clamped.z);
            }

            if (jumpQueued)
            {
                jumpQueued = false;
                if (IsGrounded())
                {
                    rb.AddForce(Vector3.up * jumpImpulse, ForceMode.Impulse);
                }
            }
        }

        private bool IsGrounded()
        {
            Vector3 feet = transform.position + Vector3.down * (capsule.height * 0.5f - capsule.radius * 0.5f);
            return Physics.CheckSphere(feet, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);
        }
    }
}
