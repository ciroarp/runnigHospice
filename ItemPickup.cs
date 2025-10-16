using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public EquipItem equipItem;

    void Update()
    {
        // Pega item (E)
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Melhor Raycast para FPS: Do centro da Câmera, na direção que ela está olhando
            if (Camera.main != null)
            {
                Transform camTransform = Camera.main.transform;
                // Raycast da Câmera principal
                if (Physics.Raycast(camTransform.position, camTransform.forward, out RaycastHit hit, 3f))
                {
                    ItemObject itemObj = hit.collider.GetComponent<ItemObject>();

                    if (itemObj != null)
                    {
                        // Usa as novas propriedades
                        if (!equipItem.IsRightHandOccupied)
                            equipItem.Equip(itemObj, true);
                        else if (!equipItem.IsLeftHandOccupied)
                            equipItem.Equip(itemObj, false);
                    }
                }
            }
        }

        // Dropa item (G)
        if (Input.GetKeyDown(KeyCode.G))
        {
            // Usa as novas propriedades para checar a mão
            if (equipItem.IsRightHandOccupied)
                equipItem.DropItem(true);
            else if (equipItem.IsLeftHandOccupied)
                equipItem.DropItem(false);
        }

        // Troca item de mão (T)
        if (Input.GetKeyDown(KeyCode.T))
        {
            equipItem.SwitchItem();
        }
    }
}


































