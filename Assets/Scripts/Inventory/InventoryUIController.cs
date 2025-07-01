using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUIController : MonoBehaviour, IUIPanelController
{
    private readonly EventSystem _eventSystem;
    private readonly List<InventorySlotController> _slots;
    private readonly TMP_Text _descriptionText;
    private readonly TMP_Text _promptInstructionsText;
    
    private int _selectedIndex;

    private const int GridColumns = 4;

    public InventoryUIController(EventSystem eventSystem, List<InventorySlotController> slots,
        TMP_Text descriptionText, TMP_Text promptInstructionsText)
    {
        _eventSystem = eventSystem;
        _slots = slots;
        _descriptionText = descriptionText;
        _promptInstructionsText = promptInstructionsText;
    }

    public void Refresh(IReadOnlyList<InventoryItem> items)
    {
        for (var i = 0; i < _slots.Count; i++)
        {
            if (i < items.Count && items[i] != null)
            {
                _slots[i].SetSlot(items[i]);
            }
            else
            {
                _slots[i].ClearSlot();
            }
        }

        if (items.Count > 0)
        {
            _promptInstructionsText.gameObject.SetActive(true);

            _selectedIndex = 0;
            HighlightSlot(_selectedIndex);

            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(_slots[0].gameObject);
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
        const int totalRows = 5;
        var row = _selectedIndex / GridColumns;
        var col = _selectedIndex % GridColumns;

        if (input.x > 0.5f)
        {
            col++;
            if (col >= GridColumns)
            {
                col = 0;
                row = (row + 1) % totalRows;
            }
        }
        else if (input.x < -0.5f)
        {
            col--;
            if (col < 0)
            {
                col = GridColumns - 1;
                row = (row - 1 + totalRows) % totalRows;
            }
        }

        if (input.y > 0.5f)
        {
            row--;
            if (row < 0)
            {
                row = totalRows - 1;
            }
        }
        else if (input.y < -0.5f)
        {
            row = (row + 1) % totalRows;
        }

        var newIndex = row * GridColumns + col;

        var attempts = 0;
        while (attempts < total)
        {
            if (newIndex >= total)
            {
                newIndex = 0;
            }

            if (_slots[newIndex].ItemInSlot != null)
            {
                break;
            }

            newIndex++;
            attempts++;
        }

        if (attempts >= total)
        {
            return; // everything is empty
        }

        _selectedIndex = newIndex;
        HighlightSlot(_selectedIndex);

        _eventSystem.SetSelectedGameObject(null);
        _eventSystem.SetSelectedGameObject(_slots[_selectedIndex].gameObject);
    }

    private void HighlightSlot(int index)
    {
        for (var i = 0; i < _slots.Count; i++)
        {
            _slots[i].SetHighlighted(i == index);
        }

        var selected = _slots[index];
        var itemInSlot = selected.ItemInSlot;

        if (itemInSlot != null)
        {
            _descriptionText.text = itemInSlot.inventoryItemDescription;
        }
    }

    public InventoryItem GetSelectedItem()
    {
        if (_slots.Count == 0 || _selectedIndex >= _slots.Count) return null;

        return _slots[_selectedIndex].ItemInSlot;
    }

    public void OnPanelActivated()
    {
        throw new System.NotImplementedException();
    }

    public void OnPanelDeactivated()
    {
        throw new System.NotImplementedException();
    }

    public void HandleNavigation(Vector2 input)
    {
        throw new System.NotImplementedException();
    }

    public void HandleSubmit()
    {
        throw new System.NotImplementedException();
    }

    public void HandleCancel()
    {
        throw new System.NotImplementedException();
    }
}
