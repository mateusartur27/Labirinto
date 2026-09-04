using UnityEngine;
using Labirinto.UI;

namespace Labirinto.Mechanics
{
    // Area Final. Condicao de vitoria por colisao/trigger, como o
    // enunciado pede - o jogador atravessa a area marcada como SAIDA e
    // uma mensagem de conclusao aparece.
    [RequireComponent(typeof(Collider))]
    public class SaidaTrigger : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private string mensagem = "Labirinto concluido!";
        private bool concluido;

        private void OnTriggerEnter(Collider other)
        {
            if (concluido) return;
            if (!other.CompareTag(playerTag)) return;
            if (GameUIManager.Instance == null) return;

            concluido = true;
            GameUIManager.Instance.MostrarMensagem(mensagem);
        }
    }
}
