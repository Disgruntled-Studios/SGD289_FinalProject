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

    public void DropItem(InventoryItem item)
    {
        if (item == null) return;

        var dropPos = _dropPosition.position;

        if (Physics.Raycast(dropPos, Vector3.down, out var hit, 10f, _groundLayer))
        {
            dropPos = hit.point;

            if (item.prefab)
            {
                var prefabRenderer = item.prefab.GetComponentInChildren<Renderer>();
                if (prefabRenderer)
                {
                    var halfHeight = prefabRenderer.bounds.size.y / 2f;
                    dropPos += Vector3.up * halfHeight;
                }
            }
        }
        
        if (item.prefab != null)
        {
            Instantiate(item.prefab, dropPos, Quaternion.identity);
            Debug.Log($"Dropped item: {item.itemName}");
        }

        RemoveItem(item);
    }
}
