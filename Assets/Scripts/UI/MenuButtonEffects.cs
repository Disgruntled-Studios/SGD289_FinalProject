using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class MenuButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private MonoBehaviour _menuController;
    private IMenuController _controller;
    
    [SerializeField] private TMP_Text _buttonText;

    private const float HighlightScale = 1.2f;
    private const float DefaultScale = 1.0f;

    private readonly Color _highlightColor = Color.white;
    private readonly Color _defaultColor = Color.black;
    
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
        _buttonText.transform.localScale = Vector3.one * (IsActivated ? HighlightScale : DefaultScale);
    }
}
