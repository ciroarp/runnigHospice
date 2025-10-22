using UnityEngine;

public class MovimentacaoCamera : MonoBehaviour
{
    [Header("Sensibilidade do Mouse")]
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    [Header("Balanço da Câmera (Head Bob)")]
    public MovimentaçãoRevisada scriptMovimentacao;

    public float amplitudeBob = 0.01f;
    public float frequenciaBob = 10f;
    public float amortecimentoBob = 0.9f;

    private float tempoBob;
    private Vector3 posicaoCameraInicial;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        posicaoCameraInicial = transform.localPosition;
    }

    // --- MOVIMENTO DE MOUSE EM LateUpdate ---
    void LateUpdate()
    {
        // Lê o mouse *após* todo o movimento do playerBody estar aplicado (evita jitter)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotação vertical (câmera)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotação horizontal (corpo do jogador)
        playerBody.Rotate(Vector3.up * mouseX);

        AtualizarHeadBob();
    }

    // --- HEAD BOB SEPARADO ---
    void AtualizarHeadBob()
    {
        bool estaMovendo = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;

        if (estaMovendo && scriptMovimentacao.controlador.isGrounded)
        {
            tempoBob += Time.deltaTime * frequenciaBob;
            float deslocamentoVertical = Mathf.Sin(tempoBob) * amplitudeBob;

            transform.localPosition = posicaoCameraInicial + Vector3.up * deslocamentoVertical;
        }
        else
        {
            if (transform.localPosition != posicaoCameraInicial)
            {
                transform.localPosition = Vector3.Lerp(
                    transform.localPosition,
                    posicaoCameraInicial,
                    Time.deltaTime * frequenciaBob * amortecimentoBob
                );
            }
            tempoBob = 0f;
        }
    }
}
