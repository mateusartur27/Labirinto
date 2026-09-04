using UnityEngine;

namespace Labirinto.Mechanics
{
    // Area 5 (Configurable Joint). O ConfigurableJoint nao tem como travar
    // um eixo ate o outro atingir um limite - cada Motion (Linear/Angular)
    // e independente. Esse script e o que impõe a ordem "desliza primeiro,
    // gira depois": angularYMotion comeca Locked, e so vira Limited quando
    // a porta ja deslizou (quase) ate o fim do curso definido pelo Linear
    // Limit. Nunca escreve na posicao/rotacao - so troca a configuracao do
    // proprio joint, quem move a porta continua sendo o jogador empurrando.
    [RequireComponent(typeof(ConfigurableJoint))]
    public class LiberaGiroAoDeslizar : MonoBehaviour
    {
        [SerializeField] private float folga = 0.2f;

        private ConfigurableJoint joint;
        private Vector3 posicaoInicial;
        private bool liberado;

        private void Awake()
        {
            joint = GetComponent<ConfigurableJoint>();
            posicaoInicial = transform.position;
        }

        private void FixedUpdate()
        {
            if (liberado) return;

            float distanciaPercorrida = Vector3.Distance(transform.position, posicaoInicial);
            if (distanciaPercorrida >= joint.linearLimit.limit - folga)
            {
                joint.angularYMotion = ConfigurableJointMotion.Limited;
                liberado = true;
            }
        }
    }
}
