using UnityEngine;
using UnityEngine.UI;

namespace Labirinto.UI
{
    public class GameUIManager : MonoBehaviour
    {
        public static GameUIManager Instance { get; private set; }

        [SerializeField] private GameObject painelMensagem;
        [SerializeField] private Text textoMensagem;

        private void Awake()
        {
            Instance = this;
            if (painelMensagem != null) painelMensagem.SetActive(false);
        }

        public void MostrarMensagem(string mensagem)
        {
            if (painelMensagem != null) painelMensagem.SetActive(true);
            if (textoMensagem != null) textoMensagem.text = mensagem;
        }
    }
}
