using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput _inputActions;
    public static event Action<InputActionMap> actionMapChange;

    void Awake()
    {
        //_inputActions = gameObject.GetComponent<PlayerInput>();
        //ToggleActionMap(_inputActions.Player);
    }

    public static void ToggleActionMap(InputActionMap actionMap)
    {
        if (actionMap.enabled) return;

        _inputActions.Disable();
        actionMapChange?.Invoke(actionMap);
        actionMap.Enable();
    }
}
