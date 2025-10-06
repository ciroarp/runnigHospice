using UnityEngine;

public class Rotacao : MonoBehaviour
{
    public float velocidade = 100f;
    private Quaternion alvo;
    private bool girando = false;

    void Start()
    {
        alvo = transform.rotation;
    }

    void Update()
    {
        if (girando)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, alvo, velocidade * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, alvo) < 0.01f)
            {
                transform.rotation = alvo;
                girando = false;
            }
        }
    }

    public void Girar()
    {
        if (!girando)
        {
            alvo = transform.rotation * Quaternion.Euler(0, 0, 90);
            girando = true;
        }
    }
}
