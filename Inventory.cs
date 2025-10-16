using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<ItemData> items = new List<ItemData>();

    public void AddItem(ItemData item)
    {
        if (item == null)
        {
            Debug.LogWarning("Tentativa de adicionar item nulo ao inventário.");
            return;
        }
        items.Add(item);
        Debug.Log(item.itemName + " adicionado ao inventário!");
    }

    public void RemoveItem(ItemData item)
    {
        if (item == null) return;

        // Remove() retorna true se o item foi encontrado e removido
        if (items.Remove(item))
        {
            Debug.Log(item.itemName + " removido do inventário!");
        }
    }
}