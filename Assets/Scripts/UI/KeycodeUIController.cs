using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeycodeUIController
{
    private readonly GameObject _keycodePanel;
    private readonly List<TMP_Text> _digitDisplays;
    private readonly int[] _currentDigits;
    private int _activeDigitIndex;
    private KeycodeReceiver _activeReceiver;

    private readonly RectTransform _panelTransform;

    public bool IsOpen => _keycodePanel.activeSelf;

    private bool _isAnimating;

    public KeycodeUIController(GameObject keycodePanel, List<TMP_Text> digitDisplays)
    {
        _keycodePanel = keycodePanel;
        _panelTransform = keycodePanel.GetComponent<RectTransform>();
        _digitDisplays = digitDisplays;
        _currentDigits = new int[digitDisplays.Count];
    }

    public void Open(KeycodeReceiver receiver)
    {
        _activeReceiver = receiver;

        _keycodePanel.SetActive(true);
        _activeDigitIndex = 0;

        for (var i = 0; i < _currentDigits.Length; i++)
        {
            _digitDisplays[i].text = _currentDigits[i].ToString();
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
        ResetDigits();
        UIManager.Instance.ShakeKeycodePanel();
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

    public void ResetDigits()
    {
        for (var i = 0; i < _currentDigits.Length; i++)
        {
            _currentDigits[i] = 0;
            _digitDisplays[i].text = "0";
        }

        _activeDigitIndex = 0;
        HighlightActiveDigit();
    }

    public void ResetDigitsAndClose()
    {
        ResetDigits();
        Close();
    }

    public void AnimateCorrectCodeSequence(Action onComplete = null)
    {
        if (_isAnimating || !IsOpen) return;
        UIManager.Instance.StartCoroutine(CorrectCodeSequenceRoutine(onComplete));
    }

    private IEnumerator CorrectCodeSequenceRoutine(Action onComplete)
    {
        _isAnimating = true;

        const float animDuration = 0.2f;
        const float startSize = 150f;
        const float endSize = 200f;
        const float stepDelay = 0.075f;
        const float basePitch = 1f;
        const float pitchStep = 0.07f;

        for (var i = 0; i < _digitDisplays.Count; i++)
        {
            var text = _digitDisplays[i];
            text.color = Color.green;
            
            UIManager.Instance.UIAudioController.PlaySoundWithPitch(UISound.DigitSuccess, basePitch + i * pitchStep);

            RumbleController.Instance.TriggerPresetRumble(RumblePreset.KeycodeDigitSuccess);

            var t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / animDuration;
                text.fontSize = Mathf.Lerp(startSize, endSize, t);
                yield return null;
            }

            text.fontSize = endSize;

            yield return new WaitForSeconds(stepDelay);
        }

        _isAnimating = false;
        onComplete?.Invoke();
    }
}
