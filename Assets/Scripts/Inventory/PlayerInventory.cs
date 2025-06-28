using System;
using System.Collections.Generic;
using TMPro.EditorUtilities;
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

    public void DropItem(InventoryItem item)
    {
        if (item is not { isDroppable: true }) return;

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
        
        if (item.prefab)
        {
            Instantiate(item.prefab, dropPos, Quaternion.identity);
        }
        
        if (UIManager.Instance)
        {
            UIManager.Instance.StartPopUpText($"You dropped: {item.itemName}.");
        }

        RemoveItem(item);
    }

    public bool TryReadItem(InventoryItem item)
    {
        if (item is not { isReadable: true }) return false;

        if (!item.isReadable && !string.IsNullOrWhiteSpace(item.noteContents))
        {
            if (!UIManager.Instance.IsOnInventoryPanel) return false;
            
            UIManager.Instance.ToggleNoteContents(true, item.noteContents);
            return true;
        }

        return false;
    }
}
