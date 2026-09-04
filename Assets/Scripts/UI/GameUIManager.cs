using UnityEngine;
using UnityEngine.UI;

namespace Labirinto.UI
{
    public class GameUIManager : MonoBehaviour
    {
        public static GameUIManager Instance { get; private set; }

        [SerializeField] private GameObject messagePanel;
        [SerializeField] private Text messageText;

        private void Awake()
        {
            Instance = this;
            if (messagePanel != null) messagePanel.SetActive(false);
        }

        public void ShowMessage(string message)
        {
            if (messagePanel != null) messagePanel.SetActive(true);
            if (messageText != null) messageText.text = message;
        }
    }
}
