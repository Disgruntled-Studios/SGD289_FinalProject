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
    GunRecoil,
    UIConfirm,
    DamageImpact,
    KeycodeError,
    DeathShock,
    ItemPickup,
    KeycodeDigitSuccess,
    KeycodeInvalid,
    KeycodeSequenceComplete,
    ConnectedFeedback,
    DisconnectedFeedback,
    EnemyGrowl,
    KeycodeInteract
}

public class RumbleController : MonoBehaviour
{
    public static RumbleController Instance { get; private set; }
    
    private Coroutine _activeRumble;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

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
            case RumblePreset.GunRecoil:
                TriggerPatternedRumble(0.6f, 0.35f, RumblePattern.Pulse);
                break;
            case RumblePreset.UIConfirm:
                TriggerPatternedRumble(0.3f, 0.15f, RumblePattern.Constant);
                break;
            case RumblePreset.DamageImpact:
                TriggerPatternedRumble(0.8f, 0.4f, RumblePattern.Pulse);
                break;
            case RumblePreset.KeycodeError:
                TriggerPatternedRumble(0.6f, 0.3f, RumblePattern.Heartbeat);
                break;
            case RumblePreset.DeathShock:
                TriggerPatternedRumble(1f, 1.2f, RumblePattern.RampDown);
                break;
            case RumblePreset.ItemPickup:
                TriggerPatternedRumble(0.4f, 0.2f, RumblePattern.Constant);
                break;
            case RumblePreset.KeycodeDigitSuccess:
                TriggerPatternedRumble(0.25f, 0.1f, RumblePattern.Constant);
                break;
            case RumblePreset.KeycodeSequenceComplete:
                TriggerPatternedRumble(0.5f, 0.35f, RumblePattern.RampUp);
                break;
            case RumblePreset.ConnectedFeedback:
                TriggerPatternedRumble(0.4f, 0.1f, RumblePattern.RampUp);
                break;
            case RumblePreset.DisconnectedFeedback:
                TriggerPatternedRumble(0.4f, 0.1f, RumblePattern.RampDown);
                break;
            case RumblePreset.KeycodeInvalid:
                TriggerPatternedRumble(0.8f, 0.2f, RumblePattern.RampDown);
                break;
            case RumblePreset.EnemyGrowl:
                TriggerPatternedRumble(0.8f, 1.5f, RumblePattern.RampUp);
                break;
            case RumblePreset.KeycodeInteract:
                TriggerPatternedRumble(0.4f, 0.2f, RumblePattern.RampUp);
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

    public void StopAllRumbles()
    {
        var gamepad = Gamepad.current;

        gamepad?.SetMotorSpeeds(0f, 0f);
    }

    public void SetMotorSpeeds(float lowFreq, float highFreq)
    {
        Gamepad.current?.SetMotorSpeeds(lowFreq, highFreq);
    }
}
