using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class MenuButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Controller Reference")]
    [SerializeField] private MonoBehaviour _menuController;
    private IMenuController _controller;
    
    [Header("Button Text")]
    [SerializeField] private TMP_Text _buttonText;

    [FormerlySerializedAs("HighlightScale")]
    [Header("Visual Settings")]
    [SerializeField] private float _highlightScale = 1.2f;
    [FormerlySerializedAs("DefaultScale")] [SerializeField] private float _defaultScale = 1.0f;

    [SerializeField] private Color _highlightColor = Color.white;
    [SerializeField] private Color _defaultColor = Color.black;
    
    public bool IsActivated { get; set; }

    private void Awake()
    {
        _controller = _menuController as IMenuController;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Activate();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Deactivate();
    }

    public void Deactivate()
    {
        IsActivated = false;
        ApplyVisual();
    }

    public void Activate()
    {
        _controller?.OnButtonActivated(this);
    }

    public void ApplyVisual()
    {
        _buttonText.color = IsActivated ? _highlightColor : _defaultColor;
        _buttonText.transform.localScale = Vector3.one * (IsActivated ? _highlightScale : _defaultScale);
    }
}
