using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private bool _isActive;

    [SerializeField] private GameObject _inventorySlotPrefab;
    [SerializeField] private Transform _slotParent;

    private readonly List<GameObject> _slotObjects = new();
    private int _selectedIndex;

    private readonly Color _normalColor = Color.white;
    private readonly Color _highlightedColor = Color.yellow;

    public void Update()
    {
        if (GameManager.Instance)
        {
            _isActive = GameManager.Instance.isGamePaused;
        }
    }
    
    public void RefreshUI(IReadOnlyList<InventoryItem> items)
    {
        foreach (var item in _slotObjects)
        {
            Destroy(item);
        }
        
        _slotObjects.Clear();

        foreach (var item in items)
        {
            var obj = Instantiate(_inventorySlotPrefab, _slotParent);
            var text = obj.transform.Find("ItemName").GetComponent<TMP_Text>();
            
            if (text)
            {
                text.text = item.itemName;
            }
            else
            {
                Debug.Log("No text");
            }

            var icon = obj.transform.Find("Icon").GetComponent<Image>();
            if (icon)
            {
                icon.sprite = item.icon;
                icon.enabled = item.icon != null;
            }
            else
            {
                Debug.Log("No icon");
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
            var img = _slotObjects[i].GetComponent<Image>();
            if (img)
            {
                img.color = (i == index) ? _highlightedColor : _normalColor;
            }
        }
    }
}
