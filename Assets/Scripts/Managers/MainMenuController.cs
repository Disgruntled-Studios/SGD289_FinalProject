using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour, IMenuController
{
    [SerializeField] private string _firstLevelName = "PowerPlant";

    [SerializeField] private MenuButtonEffects _startButtonEffects;
    [SerializeField] private MenuButtonEffects _quitButtonEffects;

    [SerializeField] private Button _startButton;
    [SerializeField] private Button _quitButton;

    private float _lastHorizontal;
    private bool _prevDpadLeft;
    private bool _prevDpadRight;
    
    private void Update()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var dpadLeft = Gamepad.current?.dpad.left.isPressed == true;
        var dpadRight = Gamepad.current?.dpad.right.isPressed == true;

        // Left movement
        if ((_lastHorizontal >= -0.5f && horizontal < -0.5f) || (!_prevDpadLeft && dpadLeft))
        {
            OnButtonActivated(_startButtonEffects);
        }
        
        // RIGHT movement
        if ((_lastHorizontal <= 0.5f && horizontal > 0.5f) ||
            (!_prevDpadRight && dpadRight))
        {
            OnButtonActivated(_quitButtonEffects);
        }

        _lastHorizontal = horizontal;
        _prevDpadLeft = dpadLeft;
        _prevDpadRight = dpadRight;

        var submitKeyboard = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space);
        var submitGamepad = Gamepad.current?.buttonSouth.wasPressedThisFrame == true;

        if (submitKeyboard || submitGamepad)
        {
            if (_startButtonEffects.IsActivated)
            {
                _startButton.onClick?.Invoke();
            }
            else
            {
                _quitButton.onClick?.Invoke();
            }
        }
    }

    public void OnButtonActivated(MenuButtonEffects activatedButton)
    {
        _startButtonEffects.Deactivate();
        _quitButtonEffects.Deactivate();

        activatedButton.IsActivated = true;
        activatedButton.ApplyVisual();
    }
    
    public void StartGame()
    {
        SceneManager.LoadScene(_firstLevelName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
