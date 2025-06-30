using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUIController
{
    private readonly EventSystem _eventSystem;
    private readonly GameObject _slotPrefab;
    private readonly Transform _slotParent;
    private readonly TMP_Text _descriptionText;
    private readonly TMP_Text _promptInstructionsText;
    private readonly int _gridColumns;

    private readonly List<GameObject> _slots = new();
    private int _selectedIndex;

    public InventoryUIController(EventSystem eventSystem, GameObject slotPrefab, Transform slotParent,
        TMP_Text descriptionText, TMP_Text promptInstructionsText, int gridColumns = 3)
    {
        _eventSystem = eventSystem;
        _slotPrefab = slotPrefab;
        _slotParent = slotParent;
        _descriptionText = descriptionText;
        _promptInstructionsText = promptInstructionsText;
        _gridColumns = gridColumns;
    }

    public void Refresh(IReadOnlyList<InventoryItem> items)
    {
        foreach (var obj in _slots)
        {
            Object.Destroy(obj);
        }
        
        _slots.Clear();

        foreach (var item in items)
        {
            var obj = Object.Instantiate(_slotPrefab, _slotParent);
            var controller = obj.GetComponent<InventorySlotController>();
            if (controller)
            {
                controller.SetSlot(item);
            }

            _slots.Add(obj);
        }

        if (_slots.Count > 0)
        {
            _promptInstructionsText.gameObject.SetActive(true);
            
            _selectedIndex = 0;
            HighlightSlot(_selectedIndex);

            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(_slots[0]);
        }
        else
        {
            _eventSystem.SetSelectedGameObject(null);
            _descriptionText.gameObject.SetActive(false);
            _promptInstructionsText.gameObject.SetActive(false);
        }
    }

    public void Navigate(Vector2 input)
    {
        if (_slots.Count == 0) return;

        var total = _slots.Count;
        var rows = Mathf.CeilToInt((float)total / _gridColumns);
        var row = _selectedIndex / _gridColumns;
        var col = _selectedIndex % _gridColumns;

        if (input.x > 0.5f)
        {
            col++;
            if (col >= _gridColumns)
            {
                col = 0;
                row = (row + 1) % rows;
            }
        }
        else if (input.x < -0.5f)
        {
            col--;
            if (col < 0)
            {
                col = _gridColumns - 1;
                row = (row - 1 + rows) % rows;
            }
        }

        if (input.y > 0.5f)
        {
            row--;
            if (row < 0)
            {
                row = rows - 1;
            }
        }
        else if (input.y < -0.5f)
        {
            row = (row + 1) % rows;
        }

        var newIndex = row * _gridColumns + col;
        if (newIndex >= total)
        {
            while (col > 0)
            {
                col--;
                newIndex = row * _gridColumns + col;
                if (newIndex < total) break;
            }

            if (newIndex >= total) return;
        }

        _selectedIndex = newIndex;
        HighlightSlot(_selectedIndex);

        _eventSystem.SetSelectedGameObject(null);
        _eventSystem.SetSelectedGameObject(_slots[_selectedIndex]);
    }

    private void HighlightSlot(int index)
    {
        for (var i = 0; i < _slots.Count; i++)
        {
            var controller = _slots[i].GetComponent<InventorySlotController>();
            if (controller)
            {
                controller.SetHighlighted(i == index);
            }
        }

        var selected = _slots[index].GetComponent<InventorySlotController>();
        if (selected)
        {
            _descriptionText.text = selected.ItemInSlot.itemName;
            _descriptionText.gameObject.SetActive(true);
        }

        var itemInSlot = selected.ItemInSlot;

        if (GameManager.Instance.PlayerController.CurrentItemReceiver != null && !itemInSlot.isDroppable &&
            !itemInSlot.isReadable && !itemInSlot.isGun)
        {
            _promptInstructionsText.gameObject.SetActive(true);
            _promptInstructionsText.text = "Press X to Use";
        }
        else if (itemInSlot.isDroppable)
        {
            _promptInstructionsText.gameObject.SetActive(true);
            _promptInstructionsText.text = "Press X to Drop";
        }
        else if (itemInSlot.isReadable)
        {
            _promptInstructionsText.gameObject.SetActive(true);
            _promptInstructionsText.text = "Press X to Read";
        }
        else if (!itemInSlot.isDroppable && !itemInSlot.isReadable)
        {
            // Gun behavior
            _promptInstructionsText.gameObject.SetActive(false);
        }
        else
        {
            _promptInstructionsText.gameObject.SetActive(false);
        }
    }

    public InventoryItem GetSelectedItem(IReadOnlyList<InventoryItem> items)
    {
        if (_slots.Count == 0 || _selectedIndex >= items.Count) return null;

        return items[_selectedIndex];
    }
}
