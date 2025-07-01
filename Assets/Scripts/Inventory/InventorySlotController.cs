using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InventorySlotController : MonoBehaviour
{
    [SerializeField] private GameObject _iconObject;
    [SerializeField] private Image _itemIconImage;
    [SerializeField] private Image _backgroundImage;

    private InventoryItem _itemInSlot;
    public InventoryItem ItemInSlot => _itemInSlot;
    
    public void SetSlot(InventoryItem item)
    {
        _itemInSlot = item;

        if (_iconObject)
        {
            _iconObject.SetActive(true);
        }

        if (_itemIconImage)
        {
            _itemIconImage.sprite = item.icon;
        }
        
        _itemIconImage.transform.localScale = Vector3.one;
    }

    public void SetHighlighted(bool highlighted)
    {
        _backgroundImage.color = highlighted ? Color.red : Color.white;
    }

    public void ClearSlot()
    {
        _iconObject.SetActive(false);
        _itemIconImage.sprite = null;
        
        _itemIconImage.transform.localScale = Vector3.one;
    }
}
