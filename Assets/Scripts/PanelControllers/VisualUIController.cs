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
    private List<Selectable> _selectables;
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
    }

    public void OnPanelDeactivated()
    {
        return;
    }

    public void HandleNavigation(Vector2 input)
    {
        if (_selectables == null || _selectables.Count == 0) return;

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
        else
        {
            return;
        }

        UIManager.Instance.SetEventSystemObject(_selectables[_currentIndex].gameObject);
    }

    public void HandleSubmit()
    {
        var current = UIManager.Instance.GetCurrentSelectedObject();
        if (!current) return;

        var toggle = current.GetComponent<Toggle>();
        if (toggle)
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
}
