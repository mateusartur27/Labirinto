using UnityEngine;

namespace Labirinto.Mechanics
{
    // Area 5 (Configurable Joint). O ConfigurableJoint nao tem como travar
    // um eixo ate o outro atingir um limite - cada Motion (Linear/Angular)
    // e independente. Esse script e o que impõe a ordem "desliza primeiro,
    // gira depois": angularYMotion comeca Locked, e so vira Limited quando
    // a porta ja deslizou (quase) ate o fim do curso definido pelo Linear
    // Limit.
    //
    // O pivo do giro e sempre o ponto onde a ancora fixa (connectedBody)
    // esta - se ela ficar la atras, de onde a porta comecou a deslizar, o
    // giro pivotaria em torno desse ponto antigo, nao da borda atual da
    // porta. Por isso, no momento de liberar o giro, o script tambem move
    // a ancora fixa pra posicao atual da porta e trava o eixo linear ali -
    // a partir dai o mecanismo vira, na pratica, um hinge de verdade.
    [RequireComponent(typeof(ConfigurableJoint))]
    public class LiberaGiroAoDeslizar : MonoBehaviour
    {
        [SerializeField] private float folga = 0.2f;
        [SerializeField] private Transform ancoraFixa;

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
                if (ancoraFixa != null)
                    ancoraFixa.position = transform.TransformPoint(joint.anchor);

                joint.zMotion = ConfigurableJointMotion.Locked;
                joint.angularYMotion = ConfigurableJointMotion.Limited;
                liberado = true;
            }
        }
    }
}
