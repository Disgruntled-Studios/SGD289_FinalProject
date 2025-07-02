using System;
using System.Collections;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInput _playerInput;
    public PlayerInput PlayerInput => _playerInput;
    public event Action<InputActionMap> OnActionMapChange;

    // May need to rethink these
    public bool IsInPuzzle => _playerInput.PuzzleMap.enabled;
    public bool IsInUI => _playerInput.UIMap.enabled;
    public bool IsInKeycode => _playerInput.KeycodeMap.enabled;

    [Header("Control Scripts")] 
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private UIControls _uiControls;
    [SerializeField] private PuzzleControls _puzzleControls;
    [SerializeField] private KeycodeControls _keycodeControls;

    private int _lockoutFramesRemaining;
    private bool IsInputLocked => _lockoutFramesRemaining > 0;

    private InputDeviceType _currentDevice = InputDeviceType.Unknown;
    public InputDeviceType CurrentDevice => _currentDevice;

    public bool IsUsingPC => _currentDevice == InputDeviceType.PC;
    public bool IsUsingGamepad => _currentDevice == InputDeviceType.Gamepad;

    public event Action<InputDeviceType> OnActiveDeviceChanged;
    
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
        _playerInput.Enable();
        SwitchToDefaultInput();

        // Reset just in case
        InputSystem.onEvent -= OnInputEvent;
        InputSystem.onEvent += OnInputEvent;
    }

    private void OnDestroy()
    {
        InputSystem.onEvent -= OnInputEvent;
    }

    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>()) return;

        var hasMeaningfulInput = false;
        foreach (var control in device.allControls)
        {
            var value = control.ReadValueAsObject();
            if (value is float f && Mathf.Abs(f) > 0.2f)
            {
                hasMeaningfulInput = true;
                break;
            }

            if (value is bool and true)
            {
                hasMeaningfulInput = true;
                break;
            }
        }

        if (!hasMeaningfulInput) return;

        var newDevice = GetDeviceType(device);

        if (newDevice == _currentDevice) return;

        _currentDevice = newDevice;
        OnActiveDeviceChanged?.Invoke(_currentDevice);
        
        #if UNITY_EDITOR
        Debug.Log($"Changed to {_currentDevice}");
        #endif
    }

    private InputDeviceType GetDeviceType(InputDevice device)
    {
        if (device is Gamepad)
        {
            return InputDeviceType.Gamepad;
        }

        if (device is Keyboard or Mouse)
        {
            return InputDeviceType.PC;
        }

        return InputDeviceType.Unknown;
    }

    private void Update()
    {
        if (_lockoutFramesRemaining > 0)
        {
            _lockoutFramesRemaining--;
        }
    }

    public void SwitchToPuzzleInput()
    {
        _playerInput.PlayerMap.Disable();
        _playerInput.UIMap.Disable();
        _playerInput.KeycodeMap.Disable();
        
        _playerInput.PuzzleMap.Enable();
        InputSystem.Update();
        LockInputGlobally();
        Debug.Log("Switching Interaction map to Puzzle");

        ActivateInputContext(_puzzleControls, _playerController, _uiControls, _keycodeControls);
    }

    public void SwitchToDefaultInput()
    {
        _playerInput.UIMap.Disable();
        _playerInput.PuzzleMap.Disable();
        _playerInput.KeycodeMap.Disable();
        
        _playerInput.PlayerMap.Enable();
        InputSystem.Update();
        LockInputGlobally();
        Debug.Log("Switching Interaction map to Default");

        ActivateInputContext(_playerController, _puzzleControls, _uiControls, _keycodeControls);
    }

    public void SwitchToUIInput()
    {
        _playerInput.PuzzleMap.Disable();
        _playerInput.PlayerMap.Disable();
        _playerInput.KeycodeMap.Disable();
        
        _playerInput.UIMap.Enable();
        InputSystem.Update();
        LockInputGlobally();
        Debug.Log("Switching Interaction map to UI");

        ActivateInputContext(_uiControls, _playerController, _puzzleControls, _keycodeControls);
    }

    public void SwitchToKeycodeInput()
    {
        _playerInput.PuzzleMap.Disable();
        _playerInput.PlayerMap.Disable();
        _playerInput.UIMap.Disable();
        
        _playerInput.KeycodeMap.Enable();
        InputSystem.Update();
        LockInputGlobally();
        Debug.Log("Switching Interaction map to KeyCode");

        ActivateInputContext(_keycodeControls, _playerController, _puzzleControls, _keycodeControls);
    }

    private void ActivateInputContext(MonoBehaviour toEnable, params MonoBehaviour[] toDisable)
    {
        foreach (var script in toDisable)
        {
            script.enabled = false;
        }

        StartCoroutine(EnableNextFrame(toEnable));
    }

    private IEnumerator EnableNextFrame(MonoBehaviour script)
    {
        yield return null;
        script.enabled = true;
    }

    private void LockInputGlobally(int frameCount = 2)
    {
        _lockoutFramesRemaining = Mathf.Max(_lockoutFramesRemaining, frameCount);
    }

    public bool ShouldBlockInput(InputAction.CallbackContext context)
    {
        return !context.performed || Instance.IsInputLocked;
    }
}
