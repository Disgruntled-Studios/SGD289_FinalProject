using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private Transform _dropPosition;
    [SerializeField] private LayerMask _groundLayer;
    
    private readonly List<InventoryItem> _items = new();

    public IReadOnlyList<InventoryItem> Items => _items;

    public event Action OnInventoryChanged;
    
    public void AddItem(InventoryItem item)
    {
        if (!item.isGun)
        {
            _items.Add(item);
        }
        
        if (UIManager.Instance)
        {
            UIManager.Instance.StartPopUpText($"You picked up: {item.itemName}. {item.additionalText}");
        }
        
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
}
