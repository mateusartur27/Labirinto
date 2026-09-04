using UnityEngine;
using UnityEngine.InputSystem;

namespace Labirinto.Player
{
    // Orbits and positions the camera's own transform around the player.
    // Never touches the player's Rigidbody/Transform - purely a spectator rig.
    public class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private Transform target;

        [SerializeField] private float distance = 6f;
        [SerializeField] private float height = 1.6f;
        [SerializeField] private float sensitivity = 0.15f;
        [SerializeField] private float minPitch = -30f;
        [SerializeField] private float maxPitch = 60f;
        [SerializeField] private float followSmoothing = 12f;
        [SerializeField] private float collisionBuffer = 0.3f;
        [SerializeField] private LayerMask obstructionMask = ~0;

        private InputAction lookAction;
        private float yaw;
        private float pitch = 15f;

        private void Awake()
        {
            var map = inputActions.FindActionMap("Player");
            lookAction = map.FindAction("Look");
        }

        private void OnEnable()
        {
            lookAction.Enable();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnDisable()
        {
            lookAction.Disable();
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector2 look = lookAction.ReadValue<Vector2>();
            yaw += look.x * sensitivity;
            pitch -= look.y * sensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 pivot = target.position + Vector3.up * height;
            Vector3 desiredPosition = pivot - rotation * Vector3.forward * distance;

            float clampedDistance = distance;
            if (Physics.Linecast(pivot, desiredPosition, out RaycastHit hit, obstructionMask, QueryTriggerInteraction.Ignore))
            {
                clampedDistance = Mathf.Max(hit.distance - collisionBuffer, 0.2f);
            }
            desiredPosition = pivot - rotation * Vector3.forward * clampedDistance;

            transform.position = Vector3.Lerp(transform.position, desiredPosition, 1f - Mathf.Exp(-followSmoothing * Time.deltaTime));
            transform.rotation = rotation;
        }
    }
}
