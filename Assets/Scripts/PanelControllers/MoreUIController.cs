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
        UIManager.Instance.SetEventSystemObject(_buttons[_currentIndex].gameObject);
    }

    public void OnPanelDeactivated()
    {
        return;
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
        else
        {
            return;
        }

        UIManager.Instance.SetEventSystemObject(_buttons[_currentIndex].gameObject);
    }

    public void HandleSubmit()
    {
        var current = UIManager.Instance.GetCurrentSelectedObject();
        if (!current) return;

        var button = current.GetComponent<Button>();
        if (button)
        {
            button.onClick.Invoke();
        }
    }

    public void HandleCancel()
    {
        UIManager.Instance.ClosePauseMenu();
    }

    public GameObject GetDefaultSelectable()
    {
        return _buttons.Count > 0 ? _buttons[0].gameObject : null;
    }
}
