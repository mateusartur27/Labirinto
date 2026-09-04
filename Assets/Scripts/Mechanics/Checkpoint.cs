using UnityEngine;

namespace Labirinto.Mechanics
{
    // Marks the last safe spot the player passed through. Read by PegaQuedas
    // to know where to send the player back after a fall - this is respawn/
    // recovery logic, not the player's movement mechanic.
    [RequireComponent(typeof(Collider))]
    public class Checkpoint : MonoBehaviour
    {
        public static Vector3 UltimaPosicaoSegura { get; private set; }

        [SerializeField] private string playerTag = "Player";

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
                UltimaPosicaoSegura = transform.position;
        }
    }
}
