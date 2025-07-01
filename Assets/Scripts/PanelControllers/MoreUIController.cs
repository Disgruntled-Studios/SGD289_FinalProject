using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MoreUIController : MonoBehaviour, IUIPanelController
{
    [Header("Buttons")] 
    [SerializeField] private Button _quitToMenuButton;
    [SerializeField] private Button _quitToDesktopButton;

    private readonly List<Button> _buttons = new();
    private int _currentIndex;

    private void Awake()
    {
        _buttons.Add(_quitToMenuButton);
        _buttons.Add(_quitToDesktopButton);
    }
    
    public void OnPanelActivated()
    {
        _currentIndex = 0;

        foreach (var button in _buttons)
        {
            SetButtonHighlight(button, false);
        }

        SetButtonHighlight(_buttons[_currentIndex], true);
        
        UIManager.Instance.SetEventSystemObject(_buttons[_currentIndex].gameObject);
    }

    public void OnPanelDeactivated()
    {
        foreach (var button in _buttons)
        {
            SetButtonHighlight(button, false);
        }
    }

    public void HandleNavigation(Vector2 input)
    {
        if (_buttons.Count == 0) return;

        if (input.y > 0.5f)
        {
            _currentIndex--;
            if (_currentIndex < 0)
            {
                _currentIndex = _buttons.Count - 1;
            }
        }
        else if (input.y < -0.5f)
        {
            _currentIndex++;
            if (_currentIndex >= _buttons.Count)
            {
                _currentIndex = 0;
            }
        }

        for (var i = 0; i < _buttons.Count; i++)
        {
            SetButtonHighlight(_buttons[i], i == _currentIndex);
        }

        UIManager.Instance.SetEventSystemObject(_buttons[_currentIndex].gameObject);
    }

    public void HandleSubmit()
    {
        var current = _buttons[_currentIndex];
        if (!current) return;
        
        current.onClick.Invoke();
    }

    public void HandleCancel()
    {
        UIManager.Instance.ClosePauseMenu();
    }

    public GameObject GetDefaultSelectable()
    {
        return _buttons.Count > 0 ? _buttons[0].gameObject : null;
    }

    private void SetButtonHighlight(Button button, bool highlighted)
    {
        var image = button.targetGraphic as Image;
        if (image)
        {
            image.color = highlighted ? Color.yellow : Color.white;
        }
    }
}
