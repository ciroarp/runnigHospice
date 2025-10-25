using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class ShowName : MonoBehaviour
{
    [Tooltip("Nome que aparecerá quando mirar neste objeto.")]
    public string objectName = "Objeto sem nome";

}
