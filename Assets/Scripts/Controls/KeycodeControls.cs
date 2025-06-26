using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeycodeControls : MonoBehaviour
{
    private PlayerInput Input => InputManager.Instance.PlayerInput;

    private void OnEnable()
    {
        var keycodeMap = Input.KeycodeMap;
        
        keycodeMap.Navigate.performed += OnNavigate;
        keycodeMap.Submit.performed += OnSubmit;
        keycodeMap.Cancel.performed += OnCancel;
    }

    private void OnDisable()
    {
        var keycodeMap = Input.KeycodeMap;
        
        keycodeMap.Navigate.performed -= OnNavigate;
        keycodeMap.Submit.performed -= OnSubmit;
        keycodeMap.Cancel.performed -= OnCancel;
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.ShouldBlockInput(context)) return;

        var input = context.ReadValue<Vector2>();
        UIManager.Instance.NavigateKeycodeDigits(input);
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.ShouldBlockInput(context)) return;
        
        UIManager.Instance.SubmitKeycode();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.ShouldBlockInput(context)) return;
        
        UIManager.Instance.CloseKeycodePanel();
    }
}
