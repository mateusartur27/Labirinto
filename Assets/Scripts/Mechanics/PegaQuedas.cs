using UnityEngine;

namespace Labirinto.Mechanics
{
    // Safety net far below the level. Not part of the movement mechanic -
    // this only fires when the player has already fallen out of play (e.g. a
    // bridge plank broke under them) and just resets them to the last
    // Checkpoint, zeroing velocity so they don't keep falling.
    [RequireComponent(typeof(Collider))]
    public class PegaQuedas : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTag)) return;

            var rb = other.attachedRigidbody;
            if (rb == null) return;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = Checkpoint.UltimaPosicaoSegura;
        }
    }
}
