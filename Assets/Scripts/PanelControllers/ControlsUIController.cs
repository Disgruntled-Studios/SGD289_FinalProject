using UnityEngine;
using UnityEngine.UI;

public class ControlsUIController : MonoBehaviour, IUIPanelController
{
    [SerializeField] private Selectable _dummyButton;
    
    public void OnPanelActivated()
    {
        if (_dummyButton)
        {
            UIManager.Instance.SetEventSystemObject(_dummyButton.gameObject);
        }
    }

    public void OnPanelDeactivated()
    {
        return;
    }

    public void HandleNavigation(Vector2 input)
    {
        return;
    }

    public void HandleSubmit()
    {
        return;
    }

    public void HandleCancel()
    {
        UIManager.Instance.ClosePauseMenu();
    }

    public GameObject GetDefaultSelectable()
    {
        return _dummyButton ? _dummyButton.gameObject : null;
    }
}
