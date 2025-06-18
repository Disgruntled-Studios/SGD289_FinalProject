using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private List<InventoryItem> _items = new();

    public IReadOnlyList<InventoryItem> Items => _items;

    public void AddItem(InventoryItem item)
    {
        _items.Add(item);
    }

    public void RemoveItem(InventoryItem item)
    {
        if (_items.Contains(item))
        {
            _items.Remove(item);
        }
    }

    public void UseItem(InventoryItem item)
    {
        Debug.Log($"Using {item.itemName}");
    }
}
