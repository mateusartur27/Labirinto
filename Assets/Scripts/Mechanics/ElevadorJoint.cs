using UnityEngine;

namespace Labirinto.Mechanics
{
    // Area 5 (Configurable Joint). O elevador tem xMotion/zMotion=Locked e
    // yMotion=Limited (so anda no eixo vertical, nunca escorrega de lado -
    // um HingeJoint so gira, um FixedJoint nao move nada, nenhum dos dois
    // reproduz "translada so num eixo"). O yDrive (Linear Drive:
    // positionSpring/positionDamper) e quem realmente movimenta a
    // plataforma - este script so muda o alvo (TargetPosition), nunca
    // escreve na posicao/Transform diretamente.
    [RequireComponent(typeof(Collider))]
    public class ElevadorJoint : MonoBehaviour
    {
        [SerializeField] private ConfigurableJoint elevador;
        [SerializeField] private float alturaSubida = 5f;
        [SerializeField] private string playerTag = "Player";

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
                elevador.targetPosition = new Vector3(0f, -alturaSubida, 0f);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(playerTag))
                elevador.targetPosition = Vector3.zero;
        }
    }
}
