using UnityEngine;
using UnityEngine.InputSystem;

public class KeycodeControls : MonoBehaviour
{
    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        var input = context.ReadValue<Vector2>();
        UIManager.Instance.NavigateKeycodeDigits(input);
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        UIManager.Instance.SubmitKeycode();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        UIManager.Instance.CloseKeycodePanel();
    }
}
