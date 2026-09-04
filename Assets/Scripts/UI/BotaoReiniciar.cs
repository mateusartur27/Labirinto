using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Labirinto.UI
{
    [RequireComponent(typeof(Button))]
    public class BotaoReiniciar : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(Reiniciar);
        }

        private void Reiniciar()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
