using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InventorySlotController : MonoBehaviour
{
    [SerializeField] private Image _itemIconImage;
    [SerializeField] private Outline _slotOutline;

    private InventoryItem _itemInSlot;
    public InventoryItem ItemInSlot => _itemInSlot;
    
    public void SetSlot(InventoryItem item)
    {
        _itemInSlot = item;

        if (_itemIconImage)
        {
            _itemIconImage.sprite = item.icon;
            _itemIconImage.enabled = item.icon != null;
        }
    }

    public void SetHighlighted(bool highlighted)
    {
        var bgImage = GetComponent<Image>();
        if (bgImage)
        {
            bgImage.color = highlighted ? Color.yellow : Color.white;
        }
    }
}
