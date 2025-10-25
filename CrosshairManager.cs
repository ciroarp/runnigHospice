using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrosshairManager : MonoBehaviour
{
    [Header("Referências")]
    public Camera playerCamera;
    public Image crosshair;
    public TextMeshProUGUI objectNameText;

    [Header("Configurações")]
    public float maxDistance = 100f;
    public LayerMask raycastLayers = ~0;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        objectNameText.text = "";
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, raycastLayers))
        {
            ShowName showName = hit.collider.GetComponent<ShowName>();

            if (showName != null)
            {
                objectNameText.text = showName.objectName;
            }
            else
            {
                objectNameText.text = "";
            }
        }
        else
        {
            objectNameText.text = "";
        }
    }
}


