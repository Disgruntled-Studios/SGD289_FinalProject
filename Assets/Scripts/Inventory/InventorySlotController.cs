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

    private InventoryItem _itemInSlot;
    public InventoryItem ItemInSlot => _itemInSlot;

    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    private Coroutine _wiggleCoroutine;

    private void Awake()
    {
        _originalPosition = _itemIconImage.transform.localPosition;
        _originalRotation = _itemIconImage.transform.localRotation;
    }
    
    public void SetSlot(InventoryItem item)
    {
        _itemInSlot = item;

        if (_iconObject)
        {
            _iconObject.SetActive(true);
        }

        var isUnread = !_itemInSlot.hasBeenRead;
        _itemIconImage.sprite = isUnread ? _itemInSlot.unreadIcon : _itemInSlot.readIcon;
        
        _itemIconImage.transform.localScale = Vector3.one;

        if (isUnread)
        {
            StartWiggle();
        }
        else
        {
            StopWiggle();
        }
    }

    public void SetHighlighted(bool highlighted)
    {
        _backgroundImage.color = highlighted ? Color.red : Color.white;
        
        if (!highlighted || _itemInSlot == null || _itemInSlot.hasBeenRead) return;

        _itemInSlot.hasBeenRead = true;
        _itemIconImage.sprite = _itemInSlot.readIcon;
        StopWiggle();
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

        StopWiggle();
    }

    private void StartWiggle()
    {
        if (_wiggleCoroutine != null) return;

        if (!UIManager.Instance.IsInventoryPanelActive()) return;

        _originalRotation = _itemIconImage.transform.localRotation;
        _originalPosition = _itemIconImage.transform.localPosition;
        
        _wiggleCoroutine = StartCoroutine(WiggleIcon());
    }

    private void StopWiggle()
    {
        if (_wiggleCoroutine == null) return;
        StopCoroutine(_wiggleCoroutine);
        _wiggleCoroutine = null;
        
        if (_itemIconImage?.transform != null)
        {
            _itemIconImage.transform.localRotation = _originalRotation;
            _itemIconImage.transform.localPosition = _originalPosition;
        }
    }

    private IEnumerator WiggleIcon()
    {
        var pulseAmplitude = 0.02f;
        var pulseFrequency = 0.5f;

        var time = 0f;

        while (true)
        {
            time += Time.deltaTime * pulseFrequency;

            var scale = 1f + Mathf.Sin(time * Mathf.PI * 2f) * pulseAmplitude;
            _itemIconImage.transform.localScale = Vector3.one * scale;

            yield return null;
        }
    }



    public void RefreshWiggle()
    {
        StopWiggle();
        StartWiggle();
    }

    public void ForceStopWiggle()
    {
        StopWiggle();
    }
}
