using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<ItemData> items = new List<ItemData>();

    public void AddItem(ItemData item)
    {
        if (item == null)
        {
            Debug.LogWarning("Tentativa de adicionar item nulo ao invent�rio.");
            return;
        }
        items.Add(item);
        Debug.Log(item.itemName + " adicionado ao invent�rio!");
    }

    public void RemoveItem(ItemData item)
    {
        if (item == null) return;

        // Remove() retorna true se o item foi encontrado e removido
        if (items.Remove(item))
        {
            Debug.Log(item.itemName + " removido do invent�rio!");
        }
    }
}