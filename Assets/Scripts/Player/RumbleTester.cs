using UnityEngine;

public class RumbleTester : MonoBehaviour
{
    [SerializeField] private RumbleController _rumble;

    private void Update()
    {
        if (!_rumble) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _rumble.TriggerConstantRumble(0.5f, 0.5f, 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _rumble.TriggerPatternedRumble(1f, 1f, RumblePattern.Pulse);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _rumble.TriggerPatternedRumble(1f, 1f, RumblePattern.RampUp);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _rumble.TriggerPatternedRumble(1f, 1f, RumblePattern.RampDown);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _rumble.TriggerPatternedRumble(1f, 1f, RumblePattern.Heartbeat);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            _rumble.TriggerPresetRumble(RumblePreset.LightTap);
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            _rumble.TriggerPresetRumble(RumblePreset.StrongPulse);
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            _rumble.TriggerPresetRumble(RumblePreset.LongBuzz);
        }
    }
}
