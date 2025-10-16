using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [Header("Configuração do Item")]
    public ItemData item;
    public bool duasMaos = false;
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;

    private HighlightItem highlight;

    private void Awake()
    {
        // Tenta obter o HighlightItem de forma segura
        TryGetComponent(out highlight);
    }

    /// <summary>
    /// Define o estado de realce do item.
    /// </summary>
    public void SetHighlight(bool enable)
    {
        if (highlight == null) return;

        if (enable)
            highlight.EnableHighlight();
        else
            highlight.DisableHighlight();
    }
}


