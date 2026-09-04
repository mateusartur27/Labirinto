using UnityEngine;

namespace Labirinto.Player
{
    // Fixed-angle chase camera: follows the player's position from a constant
    // offset/angle, never rotates from input. Only ever writes to its own
    // transform - the player keeps moving exclusively through Rigidbody forces.
    public class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;

        [SerializeField] private float distance = 6f;
        [SerializeField] private float height = 1.6f;
        [SerializeField] private float pitchAngle = 20f;
        [SerializeField] private float followSmoothing = 12f;
        [SerializeField] private float collisionBuffer = 0.3f;
        [SerializeField] private LayerMask obstructionMask = ~0;

        private Quaternion fixedRotation;

        private void Awake()
        {
            fixedRotation = Quaternion.Euler(pitchAngle, 0f, 0f);
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 pivot = target.position + Vector3.up * height;
            Vector3 desiredPosition = pivot - fixedRotation * Vector3.forward * distance;

            float clampedDistance = distance;
            if (Physics.Linecast(pivot, desiredPosition, out RaycastHit hit, obstructionMask, QueryTriggerInteraction.Ignore))
            {
                clampedDistance = Mathf.Max(hit.distance - collisionBuffer, 0.2f);
            }
            desiredPosition = pivot - fixedRotation * Vector3.forward * clampedDistance;

            transform.position = Vector3.Lerp(transform.position, desiredPosition, 1f - Mathf.Exp(-followSmoothing * Time.deltaTime));
            transform.rotation = fixedRotation;
        }
    }
}
