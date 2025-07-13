using UnityEngine;
using UnityEngine.UI;

public class DeathMenuController : MonoBehaviour, IMenuController, IUIPanelController
{
    [SerializeField] private MenuButtonEffects _retryButtonEffects;
    [SerializeField] private MenuButtonEffects _mainMenuButtonEffects;

    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _mainMenuButton;

    private MenuButtonEffects _currentSelection;
    
    public void OnButtonActivated(MenuButtonEffects activatedButton)
    {
        _retryButtonEffects.Deactivate();
        _mainMenuButtonEffects.Deactivate();

        activatedButton.IsActivated = true;
        activatedButton.ApplyVisual();
        _currentSelection = activatedButton;
    }

    public void OnPanelActivated()
    {
        OnButtonActivated(_retryButtonEffects);
    }

    public void OnPanelDeactivated()
    {
        _retryButtonEffects.Deactivate();
        _mainMenuButtonEffects.Deactivate();
    }

    public void HandleNavigation(Vector2 input)
    {
        if (Mathf.Abs(input.x) < 0.5f) return;

        OnButtonActivated(input.x < 0f ? _retryButtonEffects : _mainMenuButtonEffects);
    }

    public void HandleSubmit()
    {
        if (_currentSelection == _retryButtonEffects)
        {
            _retryButton.onClick?.Invoke();
        }
        else if (_currentSelection == _mainMenuButtonEffects)
        {
            _mainMenuButton.onClick?.Invoke();
        }
    }

    public void HandleCancel()
    {
        GameManager.Instance.ReturnToMainMenu();
    }

    public GameObject GetDefaultSelectable()
    {
        return _retryButton.gameObject;
    }
}
