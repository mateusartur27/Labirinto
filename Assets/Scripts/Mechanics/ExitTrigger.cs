using UnityEngine;
using Labirinto.UI;

namespace Labirinto.Mechanics
{
    [RequireComponent(typeof(Collider))]
    public class ExitTrigger : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private string message = "Labirinto concluido!";

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag) && GameUIManager.Instance != null)
                GameUIManager.Instance.ShowMessage(message);
        }
    }
}
