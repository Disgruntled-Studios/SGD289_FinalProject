using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsUIController
{
    private readonly EventSystem _eventSystem;
    private readonly List<Button> _subButtons;
    private readonly Button _helpButton;
    private readonly Button _graphicsButton;
    private readonly Button _soundButton;
    private readonly GameObject _helpPanel;
    private readonly GameObject _graphicsPanel;
    private readonly GameObject _soundPanel;
    private readonly GameObject _settingsButton;

    private readonly List<Selectable> _graphicsElements;
    private readonly List<Selectable> _soundElements;

    private int _selectedButtonIndex;
    private int _graphicsElementIndex;
    private int _soundElementIndex;
    
    public SettingsFocusState FocusState { get; private set; }
    public SettingsPanelType PanelType { get; private set; }

    private Selectable _currentSelected;

    public SettingsUIController(EventSystem eventSystem, List<Button> subButtons, Button helpButton, 
        Button graphicsButton, Button soundButton, GameObject helpPanel, GameObject graphicsPanel, GameObject soundPanel, 
        GameObject settingsButton, List<Selectable> graphicsElements, List<Selectable> soundElements)
    {
        _eventSystem = eventSystem;
        _subButtons = subButtons;
        _helpButton = helpButton;
        _graphicsButton = graphicsButton;
        _soundButton = soundButton;
        _helpPanel = helpPanel;
        _graphicsPanel = graphicsPanel;
        _soundPanel = soundPanel;
        _settingsButton = settingsButton;
        _graphicsElements = graphicsElements;
        _soundElements = soundElements;
    }

    public void Reset()
    {
        FocusState = SettingsFocusState.MainButtons;
        PanelType = SettingsPanelType.Help;
        _selectedButtonIndex = 0;
        _graphicsElementIndex = 0;
        _soundElementIndex = 0;

        _currentSelected = null;

        ShowSubPanel(PanelType);
        SelectButton();
    }

    public void NavigateSidebar(int direction)
    {
        _selectedButtonIndex = (_selectedButtonIndex + direction + _subButtons.Count) % _subButtons.Count;

        var selected = _subButtons[_selectedButtonIndex];

        if (selected == null) return;

        if (selected == _helpButton)
        {
            PanelType = SettingsPanelType.Help;
        }
        else if (selected == _graphicsButton)
        {
            PanelType = SettingsPanelType.Graphics;
        }
        else if (selected == _soundButton)
        {
            PanelType = SettingsPanelType.Sound;
        }

        ShowSubPanel(PanelType);
        SelectButton();
    }

    public void EnterSubPanel()
    {
        FocusState = SettingsFocusState.SubPanel;

        if (PanelType == SettingsPanelType.Graphics && _graphicsElements.Count > 0)
        {
            Select(_graphicsElements[_graphicsElementIndex]);
        }
        else if (PanelType == SettingsPanelType.Sound && _soundElements.Count > 0)
        {
            Select(_soundElements[_soundElementIndex]);
        }
    }

    public void ExitSubPanel()
    {
        if (_currentSelected)
        {
            UpdateKnobHighlight(_currentSelected, false);
            _currentSelected = null;
        }
        
        FocusState = SettingsFocusState.MainButtons;
        SelectButton();
    }

    public void NavigateSubPanel(int direction)
    {
        if (PanelType == SettingsPanelType.Graphics && _graphicsElements.Count > 0)
        {
            _graphicsElementIndex =
                (_graphicsElementIndex + direction + _graphicsElements.Count) % _graphicsElements.Count;
            Select(_graphicsElements[_graphicsElementIndex]);
        }
        else if (PanelType == SettingsPanelType.Sound && _soundElements.Count > 0)
        {
            _soundElementIndex = (_soundElementIndex + direction + _soundElements.Count) % _soundElements.Count;
            Select(_soundElements[_soundElementIndex]);
        }
    }

    public void AdjustCurrentElement(int direction)
    {
        Selectable current = null;

        if (PanelType == SettingsPanelType.Graphics && _graphicsElements.Count > 0)
        {
            current = _graphicsElements[_graphicsElementIndex];
        }
        else if (PanelType == SettingsPanelType.Sound && _soundElements.Count > 0)
        {
            current = _soundElements[_soundElementIndex];
        }

        if (!current) return;

        if (current.TryGetComponent<Slider>(out var slider))
        {
            var step = slider.wholeNumbers ? 1f : (slider.maxValue - slider.minValue) / 20f;
            slider.value = Mathf.Clamp(slider.value + (step * direction), slider.minValue, slider.maxValue);
        }
        else if (current.TryGetComponent<Toggle>(out var toggle))
        {
            toggle.isOn = !toggle.isOn;
        }
        else if (current.TryGetComponent<Button>(out var button))
        {
            button.onClick.Invoke();
        }
    }

    private void ShowSubPanel(SettingsPanelType panel)
    {
        _helpPanel.SetActive(panel == SettingsPanelType.Help);
        _graphicsPanel.SetActive(panel == SettingsPanelType.Graphics);
        _soundPanel.SetActive(panel == SettingsPanelType.Sound);
    }

    private void SelectButton()
    {
        if (_currentSelected)
        {
            UpdateKnobHighlight(_currentSelected, false);
            _currentSelected = null;
        }
        
        var button = (_selectedButtonIndex >= 0 && _selectedButtonIndex < _subButtons.Count)
            ? _subButtons[_selectedButtonIndex]
            : null;

        _eventSystem.SetSelectedGameObject(null);
        _eventSystem.SetSelectedGameObject(button?.gameObject);
    }

    private void Select(Selectable selectable)
    {
        if (_currentSelected)
        {
            UpdateKnobHighlight(_currentSelected, false);
        }

        UpdateKnobHighlight(selectable, true);

        _eventSystem.SetSelectedGameObject(null);
        _eventSystem.SetSelectedGameObject(selectable.gameObject);

        _currentSelected = selectable;
    }

    private void UpdateKnobHighlight(Selectable selectable, bool isSelected)
    {
        if (!selectable) return;

        if (selectable.TryGetComponent<Slider>(out var slider))
        {
            if (!slider.handleRect) return;

            var knobImage = slider.handleRect.GetComponent<Image>();
            if (!knobImage) return;

            knobImage.color = isSelected ? Color.yellow : Color.white;
        }
    }
}
