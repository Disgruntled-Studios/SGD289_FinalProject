using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private List<InventoryItem> _items = new();

    public IReadOnlyList<InventoryItem> Items => _items;

    public event Action OnInventoryChanged;

    public void AddItem(InventoryItem item)
    {
        _items.Add(item);
        OnInventoryChanged?.Invoke();
    }

    public void RemoveItem(InventoryItem item)
    {
        if (_items.Contains(item))
        {
            _items.Remove(item);
            OnInventoryChanged?.Invoke();
        }
    }

    public void UseItem(InventoryItem item)
    {
        Debug.Log($"Using {item.itemName}");
        OnInventoryChanged?.Invoke();
    }
}
