using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InventorySlotController : MonoBehaviour
{
    [SerializeField] private GameObject _iconObject;
    [SerializeField] private Image _itemIconImage;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private GameObject _indicatorImage;

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
        
        _indicatorImage.SetActive(!_itemInSlot.hasBeenRead);
    }

    public void SetHighlighted(bool highlighted)
    {
        _backgroundImage.color = highlighted ? Color.red : Color.white;
        
        if (highlighted && _itemInSlot != null && !_itemInSlot.hasBeenRead)
        {
            _itemInSlot.hasBeenRead = true;
            _indicatorImage.SetActive(false);
        }
    }

    public void ClearSlot()
    {
        _itemInSlot = null;

        if (_iconObject)
        {
            _iconObject.SetActive(false);
        }

        if (_itemIconImage)
        {
            _itemIconImage.sprite = null;
            _itemIconImage.transform.localScale = Vector3.one;
        }
    }
}
