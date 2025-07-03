using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUIController : MonoBehaviour, IUIPanelController
{
    [Header("Inventory UI Elements")] 
    private List<InventorySlotController> _slots;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _nameText;
    private PlayerInventory _inventory;

    private const int GridColumns = 4;

    private int _selectedIndex;

    private void Awake()
    {
        _slots = UIManager.Instance.InventorySlots;
        _inventory = GameManager.Instance.PlayerInventory;
    }
    
    public void OnPanelActivated()
    {
        _slots ??= UIManager.Instance.InventorySlots;

        if (!_inventory)
        {
            _inventory = GameManager.Instance.PlayerInventory;
        }
        
        Refresh(_inventory.Items);

        _inventory.OnInventoryChanged -= RefreshInventory;
        _inventory.OnInventoryChanged += RefreshInventory;
        
        _selectedIndex = 0;
        HighlightSlot(_selectedIndex);

        UIManager.Instance.SetEventSystemObject(_slots[_selectedIndex].gameObject);
    }

    public void OnPanelDeactivated()
    {
        foreach (var slot in _slots)
        {
            slot.SetHighlighted(false);
        }

        _descriptionText.gameObject.SetActive(false);

        if (_inventory)
        {
            _inventory.OnInventoryChanged -= RefreshInventory;
        }
    }

    public void HandleNavigation(Vector2 input)
    {
        if (_slots.Count == 0) return;

        var total = _slots.Count;
        const int gridRows = 5;
        var row = _selectedIndex / GridColumns;
        var col = _selectedIndex % GridColumns;

        if (input.x > 0.5f)
        {
            col++;
            if (col >= GridColumns)
            {
                col = 0;
                row = (row + 1) % gridRows;
            }
        }
        else if (input.x < -0.5f)
        {
            col--;
            if (col < 0)
            {
                col = GridColumns - 1;
                row = (row - 1 + gridRows) % gridRows;
            }
        }

        if (input.y > 0.5f)
        {
            row--;
            if (row < 0)
            {
                row = gridRows - 1;
            }
        }
        else if (input.y < -0.5f)
        {
            row = (row + 1) % gridRows;
        }

        var newIndex = row * GridColumns + col;

        if (newIndex >= total)
        {
            while (col > 0)
            {
                col--;
                newIndex = row * GridColumns + col;
                if (newIndex < total) break;
            }

            if (newIndex >= total) return;
        }

        _selectedIndex = newIndex;
        HighlightSlot(_selectedIndex);

        UIManager.Instance.SetEventSystemObject(_slots[_selectedIndex].gameObject);
    }

    public void HandleSubmit()
    {
        var selectedItem = GetSelectedItem();
        if (selectedItem == null) return;

        var playerController = GameManager.Instance.PlayerController;
        var receiver = playerController.CurrentItemReceiver;

        if (receiver != null && !selectedItem.isGun)
        {
            if (receiver.TryReceiveItem(_inventory, selectedItem))
            {
                //UIManager.Instance.StartPopUpText($"{selectedItem.itemName} used on {receiver.Name}.");
            }
            else
            {
                // Handle incorrect item? 
            }

            Refresh(_inventory.Items);
            UIManager.Instance.ClosePauseMenu();
        }
    }

    public void HandleCancel()
    {
        UIManager.Instance.ClosePauseMenu();
    }

    public GameObject GetDefaultSelectable()
    {
        return _slots is { Count: > 0 } ? _slots[0].gameObject : null;
    }

    private void Refresh(IReadOnlyList<InventoryItem> items)
    {
        var itemCount = items.Count;
        
        for (var i = 0; i < _slots.Count; i++)
        {
            // Recent items go first in inventory
            var reversedIndex = itemCount - 1 - i;

            if (reversedIndex >= 0 && reversedIndex < itemCount && items[reversedIndex] != null)
            {
                _slots[i].SetSlot(items[reversedIndex]);
            }
            else
            {
                _slots[i].ClearSlot();
            }
        }
    }

    public void RefreshInventory()
    {
        if (!gameObject.activeInHierarchy) return;
        
        Refresh(_inventory.Items);
    }

    private void HighlightSlot(int index)
    {
        for (var i = 0; i < _slots.Count; i++)
        {
            _slots[i].SetHighlighted(i == index);
        }

        var slot = _slots[index];
        var item = slot.ItemInSlot;

        if (item != null)
        {
            _nameText.text = item.itemName;
            _nameText.transform.parent.gameObject.SetActive(true);
            
            if (!item.isNote && !item.isGun && GameManager.Instance.PlayerController.CurrentItemReceiver != null)
            {
                _descriptionText.text = $"Use {item.itemName}?";
                _descriptionText.gameObject.SetActive(true);
            }
            else
            {
                _descriptionText.text = string.IsNullOrEmpty(item.additionalText) ? "" : item.additionalText;
                _descriptionText.gameObject.SetActive(true);
            }
        }
        else
        {
            _nameText.transform.parent.gameObject.SetActive(false);
            _descriptionText.gameObject.SetActive(false);
        }
    }

    private InventoryItem GetSelectedItem()
    {
        if (_selectedIndex < 0 || _selectedIndex >= _slots.Count) return null;

        return _slots[_selectedIndex].ItemInSlot;
    }
}
