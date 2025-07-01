using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class VisualUIController : MonoBehaviour, IUIPanelController
{
    [Header("Controls")] 
    [SerializeField] private Slider _brightnessSlider;
    [SerializeField] private Toggle _fullscreenToggle;
    [SerializeField] private Toggle _vignetteToggle;
    [SerializeField] private Toggle _typingToggle;

    [Header("Selectables")] 
    private List<Selectable> _selectables = new();
    private int _currentIndex;
    
    [Header("References")] 
    [SerializeField] private CanvasGroup _brightnessOverlay;
    private GameObject _vignetteObject;
    private Vignette _vignette;
    
    private void Awake()
    {
        _vignetteObject = GameManager.Instance.Player.GetComponent<PlayerHealth>().VignetteObject;
        _vignette = GameManager.Instance.Player.GetComponent<PlayerHealth>().Vignette;
        
        _brightnessSlider.value = 0.5f;
        _fullscreenToggle.isOn = Screen.fullScreen;
        _vignetteToggle.isOn = _vignette.active;
        _typingToggle.isOn = UIManager.Instance.PopUpTypingEnabled;

        _brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
        _fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        _vignetteToggle.onValueChanged.AddListener(OnVignetteChanged);
        _typingToggle.onValueChanged.AddListener(OnTypingChanged);
    }

    private void OnEnable()
    {
        _selectables.Add(_brightnessSlider);
        _selectables.Add(_fullscreenToggle);
        _selectables.Add(_vignetteToggle);
        _selectables.Add(_typingToggle);
    }

    private void OnBrightnessChanged(float value)
    {
        if (_brightnessOverlay)
        {
            _brightnessOverlay.alpha = value;
        }
    }

    private void OnFullscreenChanged(bool isOn)
    {
        Screen.fullScreen = isOn;
    }

    private void OnVignetteChanged(bool isOn)
    {
        if (_vignetteObject)
        {
            _vignetteObject.SetActive(isOn);
        }
    }

    private void OnTypingChanged(bool isOn)
    {
        UIManager.Instance.PopUpTypingEnabled = isOn;
    }
    
    public void OnPanelActivated()
    {
        _selectables = new List<Selectable>
        {
            _brightnessSlider,
            _fullscreenToggle,
            _vignetteToggle,
            _typingToggle
        };

        _currentIndex = 0;
        UIManager.Instance.SetEventSystemObject(_selectables[_currentIndex].gameObject);
        
        SetSliderHighlight(_brightnessSlider, false);
        SetToggleHighlight(_fullscreenToggle, false);
        SetToggleHighlight(_vignetteToggle, false);
        SetToggleHighlight(_typingToggle, false);
        
        var selected = _selectables[_currentIndex];
        if (selected is Slider slider)
        {
            SetSliderHighlight(slider, true);
        }
        else if (selected is Toggle toggle)
        {
            SetToggleHighlight(toggle, true);
        }
    }

    public void OnPanelDeactivated()
    {
        SetSliderHighlight(_brightnessSlider, false);
        SetToggleHighlight(_fullscreenToggle, false);
        SetToggleHighlight(_vignetteToggle, false);
        SetToggleHighlight(_typingToggle, false);
    }

    public void HandleNavigation(Vector2 input)
    {
        if (input.y > 0.5f)
        {
            _currentIndex--;
            if (_currentIndex < 0)
            {
                _currentIndex = _selectables.Count - 1;
            }
        }
        else if (input.y < -0.5f)
        {
            _currentIndex++;
            if (_currentIndex >= _selectables.Count)
            {
                _currentIndex = 0;
            }
        }

        SetSliderHighlight(_brightnessSlider, false);
        SetToggleHighlight(_fullscreenToggle, false);
        SetToggleHighlight(_vignetteToggle, false);
        SetToggleHighlight(_typingToggle, false);

        var selected = _selectables[_currentIndex];
        if (selected is Slider slider)
        {
            SetSliderHighlight(slider, true);
        }
        else if (selected is Toggle toggle)
        {
            SetToggleHighlight(toggle, true);
        }

        UIManager.Instance.SetEventSystemObject(selected.gameObject);

        if (selected is Slider s)
        {
            var step = (s.maxValue - s.minValue) * 0.1f;
            if (input.x < -0.5f)
            {
                s.value -= step;
            }
            else if (input.x > 0.5f)
            {
                s.value += step;
            }
        }
    }

    public void HandleSubmit()
    {
        var selected = _selectables[_currentIndex];
        if (selected is Toggle toggle)
        {
            toggle.isOn = !toggle.isOn;
        }
    }

    public void HandleCancel()
    {
        UIManager.Instance.ClosePauseMenu();
    }

    public GameObject GetDefaultSelectable()
    {
        return _selectables is { Count: > 0 } ? _selectables[0].gameObject : null;
    }

    private void SetSliderHighlight(Slider slider, bool highlighted)
    {
        if (!slider || !slider.handleRect) return;

        var handle = slider.handleRect.GetComponent<Image>();
        if (handle)
        {
            handle.color = highlighted ? Color.yellow : Color.white;
        }
    }

    private void SetToggleHighlight(Toggle toggle, bool highlighted)
    {
        var backgroundImage = toggle.targetGraphic as Image;
        if (backgroundImage)
        {
            backgroundImage.color = highlighted ? Color.yellow : Color.white;
        }
    }
}
