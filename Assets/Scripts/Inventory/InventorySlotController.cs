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
        var burstDuration = .85f;
        var restDuration = 1.5f;

        var shakeAngle = 15f;
        var shakeOffset = 3f;
        var shakeFrequency = 0.03f;

        var timer = 0f;

        while (true)
        {
            // === Burst Phase ===
            timer = 0f;
            while (timer < burstDuration)
            {
                timer += Time.deltaTime;

                var angle = Random.Range(-shakeAngle, shakeAngle);
                var offsetX = Random.Range(-shakeOffset, shakeOffset);

                _itemIconImage.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
                _itemIconImage.transform.localPosition = _originalPosition + new Vector3(offsetX, 0f, 0f);

                yield return new WaitForSeconds(shakeFrequency);
            }

            // === Rest Phase ===
            _itemIconImage.transform.localRotation = _originalRotation;
            _itemIconImage.transform.localPosition = _originalPosition;
            
            yield return new WaitForSeconds(restDuration);
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
