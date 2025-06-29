using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeycodeUIController
{
    private readonly GameObject _keycodePanel;
    private readonly TMP_Text _promptText;
    private readonly List<TMP_Text> _digitDisplays;
    private readonly int[] _currentDigits;
    private int _activeDigitIndex;
    private KeycodeReceiver _activeReceiver;

    public bool IsOpen => _keycodePanel.activeSelf;

    public KeycodeUIController(GameObject keycodePanel, TMP_Text promptText, List<TMP_Text> digitDisplays)
    {
        _keycodePanel = keycodePanel;
        _promptText = promptText;
        _digitDisplays = digitDisplays;
        _currentDigits = new int[digitDisplays.Count];
    }

    public void Open(KeycodeReceiver receiver, string prompt = "Enter Keycode:")
    {
        _activeReceiver = receiver;
        _promptText.text = prompt;

        _keycodePanel.SetActive(true);
        _activeDigitIndex = 0;

        for (var i = 0; i < _currentDigits.Length; i++)
        {
            _currentDigits[i] = 0;
            _digitDisplays[i].text = "0";
        }
        
        HighlightActiveDigit();
        InputManager.Instance.SwitchToKeycodeInput();
    }

    public void Close()
    {
        if (!IsOpen) return;

        _keycodePanel.SetActive(false);
        _activeReceiver = null;
        InputManager.Instance.SwitchToDefaultInput();
    }

    public void Navigate(Vector2 input)
    {
        if (!IsOpen) return;

        if (input.x > 0.1f)
        {
            _activeDigitIndex = (_activeDigitIndex + 1) % _digitDisplays.Count;
            HighlightActiveDigit();
        }
        else if (input.x < -0.1f)
        {
            _activeDigitIndex = (_activeDigitIndex - 1 + _digitDisplays.Count) % _digitDisplays.Count;
            HighlightActiveDigit();
        }
        else if (input.y > 0.1f)
        {
            _currentDigits[_activeDigitIndex] = (_currentDigits[_activeDigitIndex] + 1) % 10;
            UpdateDigitDisplay();
        }
        else if (input.y < -0.1f)
        {
            _currentDigits[_activeDigitIndex] = (_currentDigits[_activeDigitIndex] - 1 + 10) % 10;
            UpdateDigitDisplay();
        }
    }

    public void Submit()
    {
        if (!_activeReceiver) return;

        var code = string.Join("", _currentDigits);
        _activeReceiver.SubmitCode(code);
    }

    public void ShowInvalidFeedback()
    {
        Debug.Log("Invalid keycode");
    }

    private void HighlightActiveDigit()
    {
        for (var i = 0; i < _digitDisplays.Count; i++)
        {
            _digitDisplays[i].color = (i == _activeDigitIndex) ? Color.yellow : Color.white;
        }
    }

    private void UpdateDigitDisplay()
    {
        _digitDisplays[_activeDigitIndex].text = _currentDigits[_activeDigitIndex].ToString();
    }
}
