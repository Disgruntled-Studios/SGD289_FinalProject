using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string _firstLevelName = "L1PowerPlant";

    [SerializeField] private MenuButtonEffects _startButtonEffects;
    [SerializeField] private MenuButtonEffects _quitButtonEffects;

    [SerializeField] private Button _startButton;
    [SerializeField] private Button _quitButton;

    private float _lastHorizontal;
    
    private void Update()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");

        if (_lastHorizontal <= 0.5f && horizontal > 0.5f)
        {
            OnButtonActivated(_quitButtonEffects);
        }
        else if (_lastHorizontal >= -0.5f && horizontal < -0.5f)
        {
            OnButtonActivated(_startButtonEffects);
        }

        _lastHorizontal = horizontal;

        var submit = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) ||
                     Input.GetKeyDown(KeyCode.Joystick1Button1); // Need more robust solution for getting gamepad south button 

        if (submit)
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
