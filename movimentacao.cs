using UnityEngine;

public class movimentacao : MonoBehaviour
{
    // --- Configurações de Velocidade e Pulo ---
    [Header("Velocidade e Pulo")]
    public float velocidadeCorrida = 5f;       // Velocidade base (usada ao correr, no Shift)
    public float velocidadeCaminhada = 3.75f;  // Velocidade normal
    public float multiplicadorAgachado = 0.5f; // Multiplicador de velocidade ao agachar
    public float alturaPulo = 1.5f;
    public float gravidade = -30f;
        
    // --- Configurações de Agachamento ---
    [Header("Agachamento")]
    public float alturaNormal = 2f;
    public float alturaAgachado = 1f;
    public float velocidadeTransicaoAgachamento = 8f;
    public float alturaVerificacaoLevantar = 1.9f; // Altura que o Raycast usará para verificar se há teto

    // --- Configurações de Fluidez (Smoothing) ---
    [Header("Fluidez (Smoothing)")]
    public float tempoAceleracao = 0.1f; // Tempo para atingir velocidade máxima
    public float tempoFrenagem = 0.2f;   // Tempo para parar (usado ao soltar ou trocar de direção)

    public CharacterController controlador;
    private Vector3 velocidade;
    private bool estaAgachado;

    // Variáveis privadas para controle de fluidez
    private float velocidadeHorizontalAlvo;
    private float velocidadeHorizontalAtual;
    private float velocidadeVerticalAlvo;
    private float velocidadeVerticalAtual;

    void Start()
    {
        controlador = GetComponent<CharacterController>();
        controlador.height = alturaNormal;
    }

    void Update()
    {
        // 1. Lógica de Gravidade e 'Chão'
        bool estaNoChao = controlador.isGrounded;

        if (estaNoChao && velocidade.y < 0)
        {
            // Garante que o personagem 'grude' no chão sem acumular velocidade negativa
            velocidade.y = -1f;
        }

        // 2. Movimentação Horizontal Suavizada (FLUIDEZ)

        // Usamos GetAxisRaw para obter a intenção instantânea (1, -1 ou 0)
        float moverXRaw = Input.GetAxisRaw("Horizontal");
        float moverZRaw = Input.GetAxisRaw("Vertical");

        // Define a velocidade alvo
        velocidadeHorizontalAlvo = moverXRaw;
        velocidadeVerticalAlvo = moverZRaw;

        // Determina se estamos acelerando (movendo) ou freando (parando ou trocando direção)
        float controleSuavidade = (moverXRaw != 0 || moverZRaw != 0) ? tempoAceleracao : tempoFrenagem;

        // Suaviza a velocidade atual em direção à velocidade alvo
        float taxaSuavizacao = Time.deltaTime / controleSuavidade;

        velocidadeHorizontalAtual = Mathf.Lerp(
            velocidadeHorizontalAtual,
            velocidadeHorizontalAlvo,
            taxaSuavizacao
        );

        velocidadeVerticalAtual = Mathf.Lerp(
            velocidadeVerticalAtual,
            velocidadeVerticalAlvo,
            taxaSuavizacao
        );

        // O vetor de direção agora usa os valores suavizados para o CharacterController
        Vector3 direcaoMovimento =
            transform.right * velocidadeHorizontalAtual +
            transform.forward * velocidadeVerticalAtual;

        // Calcula a velocidade baseada no estado (Agachado, Correndo ou Caminhando)
        float velocidadeAtual;

        if (estaAgachado)
        {
            velocidadeAtual = velocidadeCorrida * multiplicadorAgachado;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            velocidadeAtual = velocidadeCorrida;
        }
        else
        {
            velocidadeAtual = velocidadeCaminhada;
        }

        // Aplica o movimento horizontal
        controlador.Move(direcaoMovimento * velocidadeAtual * Time.deltaTime);

        // 3. Pulo
        if (Input.GetKeyDown(KeyCode.Space) && estaNoChao && !estaAgachado)
        {
            // Cálculo do pulo 
            velocidade.y = Mathf.Sqrt(alturaPulo * -2f * gravidade);
        }

        // Aplica a gravidade continuamente
        velocidade.y += gravidade * Time.deltaTime;
        controlador.Move(velocidade * Time.deltaTime);

        // 4. Lógica de Agachamento
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (estaAgachado)
            {
                TentarLevantar(); // Chamada da função com Raycast
            }
            else
            {
                estaAgachado = true;
            }
        }

        // Transição suave de altura (LERP)
        float alturaAlvo = estaAgachado ? alturaAgachado : alturaNormal;
        controlador.height = Mathf.Lerp(
            controlador.height,
            alturaAlvo,
            Time.deltaTime * velocidadeTransicaoAgachamento
        );
    }

    /// <summary>
    /// Verifica se há espaço para levantar e, se houver, define 'estaAgachado' como false.
    /// </summary>
    private void TentarLevantar()
    {
        // Ponto de origem do Raycast (na altura do CharacterController.center)
        Vector3 origemRaycast = transform.position + Vector3.up * controlador.center.y;

        // Se o Raycast NÃO atingir nada até a altura de verificação...
        if (!Physics.Raycast(origemRaycast, Vector3.up, alturaVerificacaoLevantar, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            // ... pode se levantar.
            estaAgachado = false;
        }
    }
}





