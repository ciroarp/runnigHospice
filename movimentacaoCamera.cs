using UnityEngine;

public class movimentacaoCamera : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    [Header("Balanço da Câmera (Head Bob)")]
    // Referência ao script de movimentação para saber se está andando/correndo
    public MovimentaçãoRevisada scriptMovimentacao;

    public float amplitudeBob = 0.01f;     // Intensidade do balanço (altura)
    public float frequenciaBob = 10f;      // Velocidade do balanço (passos por segundo)
    public float amortecimentoBob = 0.9f;  // Para suavizar a parada

    private float tempoBob;                 // Contador para a função Seno
    private Vector3 posicaoCameraInicial;   // Posição original da câmera

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        // Armazena a posição local inicial (o centro da cabeça do player)
        posicaoCameraInicial = transform.localPosition;
    }

    void Update()
    {
        // ... (Leitura do mouse e Rotação Vertical/Horizontal permanecem as mesmas)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

        // --- INÍCIO: Lógica do Balanço da Câmera ---

        // 1. Verifica se o personagem está se movendo horizontalmente
        bool estaMovendo = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;

        // 2. Se estiver se movendo e no chão, balança a câmera
        if (estaMovendo && scriptMovimentacao.controlador.isGrounded)
        {
            // Aumenta o contador no tempo (faz a onda do Seno avançar)
            tempoBob += Time.deltaTime * frequenciaBob;

            // Calcula o deslocamento vertical (seno) e horizontal (cosseno)
            float deslocamentoVertical = Mathf.Sin(tempoBob) * amplitudeBob;

            // Aplica o deslocamento à posição local da câmera
            transform.localPosition = posicaoCameraInicial + Vector3.up * deslocamentoVertical;
        }
        else
        {
            // 3. Se estiver parado ou no ar, retorna suavemente à posição inicial
            if (transform.localPosition != posicaoCameraInicial)
            {
                // Interpolação suave de volta à posição inicial
                transform.localPosition = Vector3.Lerp(
                    transform.localPosition,
                    posicaoCameraInicial,
                    Time.deltaTime * frequenciaBob * amortecimentoBob
                );
            }
            // Zera o contador para que, ao recomeçar a andar, o balanço comece suave
            tempoBob = 0f;
        }

        // --- FIM: Lógica do Balanço da Câmera ---
    }
}
