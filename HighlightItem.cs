using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class HighlightItem : MonoBehaviour
{
    private Renderer objRenderer;
    private Material originalMaterial;
    public Material highlightMaterial; // Material verde para highlight

    void Awake()
    {
        objRenderer = GetComponent<Renderer>();
        if (objRenderer != null)
            originalMaterial = objRenderer.material;
    }

    public void EnableHighlight()
    {
        if (objRenderer != null && highlightMaterial != null)
            objRenderer.material = highlightMaterial;
    }

    public void DisableHighlight()
    {
        if (objRenderer != null && originalMaterial != null)
            objRenderer.material = originalMaterial;
    }
}



