using System;
using System.Collections;
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
    }

    private void Update()
    {
        var keyboardTime = InputSystem.GetDevice<Keyboard>()?.lastUpdateTime ?? 0;
        var controllerTime = InputSystem.GetDevice<Gamepad>()?.lastUpdateTime ?? 0;

        _lastUsedDevice = keyboardTime > controllerTime ? Keyboard : Controller;

        if (_lockoutFramesRemaining > 0)
        {
            _lockoutFramesRemaining--;
        }
        
        //  Debug.Log($"Is In Puzzle = {IsInPuzzle}");
        //  Debug.Log($"Is In UI = {IsInUI}");
        //  Debug.Log($"Is In KeyCode = {IsInKeycode}");
        //  Debug.Log($"Default = {!IsInUI && !IsInPuzzle && !IsInKeycode}");
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
