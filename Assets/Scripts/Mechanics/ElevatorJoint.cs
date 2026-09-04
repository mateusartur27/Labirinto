using UnityEngine;

namespace Labirinto.Mechanics
{
    // Area 5 (Configurable Joint). The portcullis' ConfigurableJoint has
    // yMotion = Limited with a Linear Limit capping how far it can rise, and a
    // yDrive (Linear Drive: positionSpring/positionDamper) that smoothly pulls
    // it toward targetPosition. This script only ever changes targetPosition -
    // the drive/spring does the actual moving, never a Transform write.
    [RequireComponent(typeof(Collider))]
    public class ElevatorJoint : MonoBehaviour
    {
        [SerializeField] private ConfigurableJoint gate;
        [SerializeField] private float raisedHeight = 2.2f;
        [SerializeField] private string playerTag = "Player";

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
                gate.targetPosition = new Vector3(0f, raisedHeight, 0f);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(playerTag))
                gate.targetPosition = Vector3.zero;
        }
    }
}
