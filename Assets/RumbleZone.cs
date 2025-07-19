using System;
using System.Collections;
using UnityEngine;

public class RumbleZone : MonoBehaviour
{
    [SerializeField] private float _maxIntensity = 0.6f;
    [SerializeField] private float _fadeDuration = 0.75f;

    private Coroutine _fadeCoroutine;
    private bool _isPlayerInside;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _isPlayerInside = true;

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        _fadeCoroutine = StartCoroutine(FadeInRumble());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _isPlayerInside = false;

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        _fadeCoroutine = StartCoroutine(FadeOutRumble());
    }
    
    private IEnumerator FadeInRumble()
    {
        var time = 0f;

        while (time < _fadeDuration)
        {
            if (!_isPlayerInside) yield break;

            var factor = time / _fadeDuration;
            RumbleController.Instance.SetMotorSpeeds(_maxIntensity * factor, _maxIntensity * factor);

            time += Time.deltaTime;
            yield return null;
        }

        RumbleController.Instance.SetMotorSpeeds(_maxIntensity, _maxIntensity);
    }

    private IEnumerator FadeOutRumble()
    {
        var currentIntensity = _maxIntensity;
        var time = 0f;

        while (time < _fadeDuration)
        {
            var factor = 1f - (time / _fadeDuration);
            RumbleController.Instance.SetMotorSpeeds(currentIntensity * factor, currentIntensity * factor);

            time += Time.deltaTime;
            yield return null;
        }

        RumbleController.Instance.SetMotorSpeeds(0f, 0f);
    }
}
