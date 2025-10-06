using UnityEngine;

public class Botao : MonoBehaviour
{
    public Transform player;
    public Rotacao bloco;
    private float maxDistancia = 3f;

    void OnMouseDown()
    {
        float distancia = Vector3.Distance(player.position, transform.position);

        if (distancia <= maxDistancia)
        {
            bloco.Girar();
        }
        else
        {
            Debug.Log("");
        }
    }
}


