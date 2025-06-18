using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInput _playerInput;
    public PlayerInput PlayerInput => _playerInput;
    public event Action<InputActionMap> OnActionMapChange;

    private string _lastUsedDevice;
    private const string Keyboard = "Keyboard";
    private const string Controller = "Controller";
    
    public bool IsUsingKeyboard => _lastUsedDevice == Keyboard;
    public bool IsUsingController => _lastUsedDevice == Controller;

    public bool IsInPuzzle => _playerInput.PuzzleMap.enabled;
    public bool IsInUI => _playerInput.UI.enabled;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _playerInput = new PlayerInput();
        SwitchToDefaultInput();
    }

    private void Update()
    {
        var keyboardTime = InputSystem.GetDevice<Keyboard>()?.lastUpdateTime ?? 0;
        var controllerTime = InputSystem.GetDevice<Gamepad>()?.lastUpdateTime ?? 0;

        _lastUsedDevice = keyboardTime > controllerTime ? Keyboard : Controller;

        Debug.Log($"Is In Puzzle = {IsInPuzzle}");
        Debug.Log($"Is In UI = {IsInUI}");
        Debug.Log($"Default = {!IsInUI && !IsInPuzzle}");
    }

    public void SwitchToPuzzleInput()
    {
        _playerInput.Player.Disable();
        _playerInput.UI.Disable();
        _playerInput.PuzzleMap.Enable();
    }

    public void SwitchToDefaultInput()
    {
        _playerInput.Player.Enable();
        _playerInput.UI.Disable();
        _playerInput.PuzzleMap.Disable();
    }

    public void SwitchToUIInput()
    {
        _playerInput.UI.Enable();
        _playerInput.PuzzleMap.Disable();
        _playerInput.Player.Disable();
    }
}
