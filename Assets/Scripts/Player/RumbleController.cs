using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum RumblePattern
{
    Constant,
    Pulse,
    RampUp,
    RampDown,
    Heartbeat
}

public enum RumblePreset
{
    LightTap,
    StrongPulse,
    LongBuzz
}

public class RumbleController : MonoBehaviour
{
    private Coroutine _activeRumble;

    public void TriggerConstantRumble(float low, float high, float duration)
    {
        if (Gamepad.current == null) return;

        if (_activeRumble != null)
        {
            StopCoroutine(_activeRumble);
            Gamepad.current.SetMotorSpeeds(0f, 0f);
        }

        _activeRumble = StartCoroutine(RumbleRoutine(low, high, duration));
    }

    public void TriggerPatternedRumble(float intensity, float duration, RumblePattern pattern)
    {
        if (Gamepad.current == null) return;

        if (_activeRumble != null)
        {
            StopCoroutine(_activeRumble);
            Gamepad.current.SetMotorSpeeds(0f, 0f);
        }

        _activeRumble = StartCoroutine(RumblePatternRoutine(intensity, duration, pattern));
    }

    public void TriggerPresetRumble(RumblePreset preset)
    {
        switch (preset)
        {
            case RumblePreset.LightTap:
                TriggerPatternedRumble(0.3f, 0.2f, RumblePattern.Constant);
                break;
            case RumblePreset.StrongPulse:
                TriggerPatternedRumble(1f, 0.5f, RumblePattern.Pulse);
                break;
            case RumblePreset.LongBuzz:
                TriggerPatternedRumble(0.7f, 1.5f, RumblePattern.Constant);
                break;
        }
    }

    private IEnumerator RumbleRoutine(float low, float high, float duration)
    {
        var gamepad = Gamepad.current;

        gamepad.SetMotorSpeeds(low, high);

        yield return new WaitForSeconds(duration);

        gamepad.SetMotorSpeeds(0f, 0f);

        _activeRumble = null;
    }

    private IEnumerator RumblePatternRoutine(float intensity, float duration, RumblePattern pattern)
    {
        var gamepad = Gamepad.current;

        switch (pattern)
        {
            case RumblePattern.Constant:
                gamepad.SetMotorSpeeds(intensity, intensity);
                yield return new WaitForSeconds(duration);
                break;
            case RumblePattern.Pulse:
                var pulseTime = 0.1f;
                var elapsedPulse = 0f;

                while (elapsedPulse < duration)
                {
                    gamepad.SetMotorSpeeds(intensity, intensity);
                    yield return new WaitForSeconds(pulseTime);
                    gamepad.SetMotorSpeeds(0f, 0f);
                    yield return new WaitForSeconds(pulseTime);
                    elapsedPulse += pulseTime * 2f;
                }

                break;
            
            case RumblePattern.RampUp:
                var rampUpTime = 0f;
                while (rampUpTime < duration)
                {
                    var factor = rampUpTime / duration;
                    gamepad.SetMotorSpeeds(intensity * factor, intensity * factor);
                    yield return null;
                    rampUpTime += Time.deltaTime;
                }

                break;
            case RumblePattern.RampDown:
                var rampDownTime = 0f;
                while (rampDownTime < duration)
                {
                    var factor = 1f - (rampDownTime / duration);
                    gamepad.SetMotorSpeeds(intensity * factor, intensity * factor);
                    yield return null;
                    rampDownTime += Time.deltaTime;
                }

                break;
            case RumblePattern.Heartbeat:
                for (var i = 0; i < 2; i++)
                {
                    gamepad.SetMotorSpeeds(intensity, intensity);
                    yield return new WaitForSeconds(0.1f);
                    gamepad.SetMotorSpeeds(0f, 0f);
                    yield return new WaitForSeconds(0.1f);
                }

                break;
        }

        gamepad.SetMotorSpeeds(0f, 0f);

        _activeRumble = null;
    }
}
