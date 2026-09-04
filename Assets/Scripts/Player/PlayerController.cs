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

        private Rigidbody rb;
        private InputAction moveAction;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;

            if (inputActions == null)
            {
                Debug.LogError("PlayerController: o campo 'Input Actions' esta vazio no Inspector. " +
                    "Arraste o asset Assets/InputSystem_Actions.inputactions para esse campo.", this);
                enabled = false;
                return;
            }

            var map = inputActions.FindActionMap("Player");
            moveAction = map.FindAction("Move");
        }

        private void OnEnable()
        {
            if (moveAction == null) return;
            moveAction.Enable();
        }

        private void OnDisable()
        {
            if (moveAction == null) return;
            moveAction.Disable();
        }

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
        }
    }
}
