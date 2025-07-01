using UnityEngine;

public interface IUIPanelController
{
    void OnPanelActivated();
    void OnPanelDeactivated();
    void HandleNavigation(Vector2 input);
    void HandleSubmit();
    void HandleCancel();
    GameObject GetDefaultSelectable();
}
