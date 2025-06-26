using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InventorySlotController : MonoBehaviour
{
    [SerializeField] private TMP_Text _itemNameText;
    [SerializeField] private Image _itemIconImage;
    [SerializeField] private Outline _slotOutline;

    private InventoryItem _item;
    public string ItemName => _item.itemName;
    
    public void SetSlot(InventoryItem item)
    {
        _item = item;
        
        if (_itemNameText)
        {
            _itemNameText.text = item.itemName;
        }

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
