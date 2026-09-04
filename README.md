# Labirinto Físico — Joints e Mecânicas de Física

Jogo de exploração física em Unity 6 (URP): o jogador atravessa um labirinto composto por 5 desafios, cada um construído em torno de um tipo diferente de Joint do Unity. Toda a movimentação e todas as mecânicas do cenário usam exclusivamente Rigidbody, Colliders e Joints — nenhum objeto é movido diretamente pelo Transform.

## Como jogar

- **WASD** — move o jogador (força aplicada via `Rigidbody.AddForce`).
- Câmera fixa em terceira pessoa, sem controle por mouse.
- Objetivo: atravessar as 5 áreas até o portal de saída.

## Movimentação do jogador

`Assets/Scripts/Player/PlayerController.cs` move o jogador exclusivamente por física:

- `Rigidbody.AddForce` para andar (sem pulo — o jogador não sai do chão em nenhum momento, toda a travessia depende de empurrar/subir em estruturas).
- `Rigidbody.linearVelocity` só é lido/limitado (clamp de velocidade máxima), nunca a posição.
- Nenhum uso de `transform.position`, `transform.Translate`, `SetPositionAndRotation`, `CharacterController` ou `NavMesh` em nenhum lugar do projeto (a única escrita em `transform.position` de todo o código é da câmera, em `ThirdPersonCamera.cs`, que nunca move o jogador).

A cápsula do jogador tem `Rigidbody` (com `Interpolate` ligado, pra não tremer) e `CapsuleCollider`.

## As 5 áreas e seus Joints

Cada área usa um material de cor própria pra facilitar identificar visualmente qual Joint está em ação.

### Área 1 — Estrutura Quebrável (Fixed Joint)

Três pontes de 10 tábuas cada, atravessando um abismo. Cada tábua é conectada à vizinha por um `FixedJoint` (11 juntas por ponte, 33 no total — bem acima do mínimo de 3 exigido).

- Só uma das três pontes tem `breakForce = Infinity` em todas as juntas (a segura).
- As outras duas têm `breakForce` finito — quebram sob o peso/impacto do jogador.
- O jogador descobre qual ponte é segura testando fisicamente — não há indicação visual, as três são idênticas.
- Se uma ponte quebrar e o jogador cair, a rede de segurança (`PegaQuedas.cs`) devolve ele ao último checkpoint.

### Área 2 — Plataforma Suspensa (Spring Joint)

Uma plataforma metálica presa a uma âncora fixa acima por `SpringJoint`.

- `spring` e `damper` configurados para dar à plataforma um comportamento oscilatório real.
- Ela começa deslocada da posição de equilíbrio (abaixo dele), então já nasce balançando assim que a simulação começa — não depende só do jogador para gerar movimento.
- O jogador sobe nela e usa o balanço da própria estrutura (não um pulo) para alcançar uma região elevada, do jeito que o enunciado pede.

### Área 3 — Catraca (Hinge Joint)

Uma catraca de 3 braços (120° entre si) bloqueando um gargalo estreito no corredor, com `HingeJoint` no eixo vertical (`axis = (0, 1, 0)`).

- `useMotor = true`, com `targetVelocity = 0`: o motor não gira a catraca sozinho — ele funciona como resistência mecânica. O jogador precisa empurrar com força suficiente para vencer essa resistência, e ela não fica girando livre depois que ele solta.
- Braços e poste central são filhos sem `Rigidbody` próprio, formando um corpo rígido composto que gira em torno do pivô.

### Área 4 — Cortina de Corrente (Character Joint)

Cinco tiras verticais de 6 elos cada (30 segmentos articulados no total, muito acima do mínimo de 3), penduradas de âncoras fixas no teto, bloqueando a passagem.

- Cada elo é conectado ao de cima por `CharacterJoint`, com `swing1Limit`/`swing2Limit = 45°` e limites de twist (`-15°` a `15°`) — permitem rotação física real dentro de uma faixa plausível para uma corrente.
- O jogador precisa andar contra a cortina e empurrar para atravessar; os elos balançam e cedem fisicamente, e voltam a bloquear atrás dele.

### Área 5 — Porta Deslizante-Giratória (Configurable Joint)

A mecânica final: uma porta que primeiro desliza, e só depois consegue girar — algo que nem `HingeJoint` (só gira) nem `FixedJoint` (não move nada) conseguem reproduzir sozinhos.

Configurações do `ConfigurableJoint` usadas (acima do mínimo de 2 exigido) e o papel de cada uma:

| Configuração | Valor | Função |
|---|---|---|
| `xMotion` / `yMotion` | `Locked` | A porta nunca sai do trilho — só pode se mover no eixo Z. |
| `zMotion` (Motion + Linear Limit) | `Limited`, `linearLimit ≈ 6.7` | Define o eixo e a distância máxima que a porta desliza ao ser empurrada. |
| `angularYMotion` (Motion + Angular Limit) | Começa `Locked`, script libera para `Limited` (`limit = 90°`) | Define até quantos graus a porta pode girar — mas só depois de liberada. |

Como a ordem "desliza primeiro, gira depois" é garantida: o `ConfigurableJoint` não tem como travar um eixo até o outro atingir um limite — cada `Motion` é independente. Por isso existe `Assets/Scripts/Mechanics/LiberaGiroAoDeslizar.cs`: ele mede quanto a porta já deslizou a cada `FixedUpdate` e, só quando ela chega perto do fim do curso, muda `angularYMotion` de `Locked` para `Limited` (e reancora o pivô de rotação na posição atual da porta, para o giro acontecer em torno de onde ela está de verdade, não de onde começou). O script nunca escreve na posição ou rotação da porta diretamente — só reconfigura o próprio joint; quem efetivamente move a porta continua sendo o jogador empurrando.

## Área Final — Escape

`Portal_Saida` (pilares + arco + painel de energia) marca a saída. Ele mesmo carrega o `Collider` (trigger) e o script `SaidaTrigger.cs` — não é um gatilho invisível separado.

Ao entrar no portal, `GameUIManager.cs` mostra a tela de conclusão: cartão central, título, resumo dos 5 Joints usados e um botão **Jogar Novamente** (`BotaoReiniciar.cs`) que recarrega a cena.

## Sistema de checkpoint e resgate

`Checkpoint.cs` e `PegaQuedas.cs` formam um sistema de recuperação de erro — **não fazem parte da mecânica de movimentação do jogador**, que continua sendo só `AddForce`:

- Cada área tem um `Checkpoint` (trigger) que marca a última posição seguindo por onde o jogador passou.
- Uma única `PegaQuedas` (trigger grande, bem abaixo de todo o labirinto) captura quem cair de uma estrutura quebrada e devolve ao último checkpoint via `Rigidbody.position`, zerando a velocidade.
- Isso é reposicionamento de recuperação de falha, análogo a "cair fora do mapa e voltar pro início da fase" em qualquer jogo — não é como o jogador se movimenta durante o jogo normal.

## Estrutura do projeto

```
Assets/
  Scenes/Labirinto.unity        cena única do jogo
  Scripts/
    Player/                     movimentação e câmera
    Mechanics/                  checkpoints, resgate, gatilho de saída, controle do joint da Área 5
    UI/                         tela de vitória e botão de reiniciar
  Materials/                    um material de cor por mecanismo/área
```

## Tecnologia

- Unity 6000.3.3f1, Universal Render Pipeline (URP).
- New Input System (`InputSystem_Actions.inputactions`).
