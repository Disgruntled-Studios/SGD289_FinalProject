using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _inventorySlotPrefab;
    [SerializeField] private Transform _slotParent;

    private readonly List<GameObject> _slotObjects = new();
    private int _selectedIndex;
    
    public void RefreshUI(IReadOnlyList<InventoryItem> items)
    {
        // Destroy previous inventory for proper arrangement
        foreach (var obj in _slotObjects)
        {
            Destroy(obj);
        }
        
        _slotObjects.Clear();

        foreach (var item in items)
        {
            var obj = Instantiate(_inventorySlotPrefab, _slotParent);
            var controller = obj.GetComponent<InventorySlotController>();

            if (controller)
            {
                controller.SetSlot(item);
            }

            _slotObjects.Add(obj);
        }

        if (_slotObjects.Count > 0)
        {
            _selectedIndex = 0;
            HighlightSlot(_selectedIndex);
        }
    }

    public void Navigate(int direction)
    {
        if (_slotObjects.Count == 0) return;

        _selectedIndex = (_selectedIndex + direction + _slotObjects.Count) % _slotObjects.Count;
        HighlightSlot(_selectedIndex);
    }

    public InventoryItem GetSelectedItem(IReadOnlyList<InventoryItem> items)
    {
        return items.Count == 0 ? null : items[_selectedIndex];
    }

    private void HighlightSlot(int index)
    {
        for (var i = 0; i < _slotObjects.Count; i++)
        {
            var controller = _slotObjects[i].GetComponent<InventorySlotController>();
            if (controller)
            {
                controller.SetHighlighted(i == index);
            }
        }
    }
}
