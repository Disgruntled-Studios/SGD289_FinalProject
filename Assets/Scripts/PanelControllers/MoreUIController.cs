using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MoreUIController : MonoBehaviour, IUIPanelController
{
    [Header("Buttons")] 
    [SerializeField] private Button _quitToMenuButton;
    [SerializeField] private Button _quitToDesktopButton;

    [SerializeField] private Toggle _rumbleToggle;

    private readonly List<Selectable> _selectables = new();
    private int _currentIndex;

    private void Awake()
    {
        _selectables.Add(_rumbleToggle);
        _selectables.Add(_quitToMenuButton);
        _selectables.Add(_quitToDesktopButton);
    }
    
    public void OnPanelActivated()
    {
        _currentIndex = 0;

        foreach (var s in _selectables)
        {
            HighlightSelectable(s, false);
        }

        HighlightSelectable(_selectables[_currentIndex], true);
        UIManager.Instance.SetEventSystemObject(_selectables[_currentIndex].gameObject);
    }

    public void OnPanelDeactivated()
    {
        foreach (var s in _selectables)
        {
            HighlightSelectable(s, false);
        }
    }

    public void HandleNavigation(Vector2 input)
    {
        if (_selectables.Count == 0) return;

        if (input.y > 0.5f)
        {
            _currentIndex = (_currentIndex - 1 + _selectables.Count) % _selectables.Count;
        }
        else if (input.y < -0.5f)
        {
            _currentIndex = (_currentIndex + 1) & _selectables.Count;
        }

        foreach (var s in _selectables)
        {
            HighlightSelectable(s, false);
        }

        var selected = _selectables[_currentIndex];
        HighlightSelectable(selected, true);
        UIManager.Instance.SetEventSystemObject(selected.gameObject);

        if (selected is Slider slider)
        {
            var step = (slider.maxValue - slider.minValue) * 0.1f;
            switch (input.x)
            {
                case < -0.5f:
                    slider.value -= step;
                    break;
                case > 0.5f:
                    slider.value += step;
                    break;
            }
            UIManager.Instance.UIAudioController.PlaySound(UISound.SliderAdjust);
        }
    }

    public void HandleSubmit()
    {
        var selected = _selectables[_currentIndex];

        if (selected is Button button)
        {
            button.onClick.Invoke();
            UIManager.Instance.UIAudioController.PlaySound(UISound.Button);
        }
        else if (selected is Toggle toggle)
        {
            toggle.isOn = !toggle.isOn;
            UIManager.Instance.UIAudioController.PlaySound(UISound.Toggle);
        }
    }

    public void HandleCancel()
    {
        UIManager.Instance.ClosePauseMenu();
    }

    public GameObject GetDefaultSelectable()
    {
        return _selectables.Count > 0 ? _selectables[0].gameObject : null;
    }

    private void HighlightSelectable(Selectable selectable, bool highlighted)
    {
        switch (selectable)
        {
            case Button button:
                var buttonImage = button.targetGraphic as Image;
                if (buttonImage) buttonImage.color = highlighted ? Color.yellow : Color.white;
                break;
            case Toggle toggle:
                var toggleImage = toggle.targetGraphic as Image;
                if (toggleImage) toggleImage.color = highlighted ? Color.yellow : Color.white;
                break;
        }
    }
}
