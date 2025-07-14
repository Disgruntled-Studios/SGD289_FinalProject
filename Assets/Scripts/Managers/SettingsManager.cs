using UnityEngine;
using UnityEngine.Rendering;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [Header("Audio Settings")] 
    public float MasterVolume { get; private set; } = 1f;
    public float MusicVolume { get; private set; } = 1f;
    public float SFXVolume { get; private set; } = 1f;
    public float AmbienceVolume { get; private set; } = 1f;

    [Header("Visual Settings")] 
    public float Brightness { get; private set; } = 1f;
    public bool IsFullScreen { get; private set; } = true;
    public bool HealthVignetteEnabled { get; private set; } = true;
    public bool PopupTypingEffectEnabled { get; private set; } = true;

    [Header("Gameplay Settings")] 
    public bool RumbleEnabled { get; private set; } = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetMasterVolume(float value) => MasterVolume = value;
    public void SetMusicVolume(float value) => MusicVolume = value;
    public void SetSFXVolume(float value) => SFXVolume = value;
    public void SetAmbienceVolume(float value) => AmbienceVolume = value;

    public void SetBrightness(float value) => Brightness = value;
    public void SetFullscreen(bool value) => IsFullScreen = value;
    public void SetHealthVignette(bool value) => HealthVignetteEnabled = value;
    public void SetTypingEffect(bool value) => PopupTypingEffectEnabled = value;

    public void SetRumbleEnabled(bool value) => RumbleEnabled = value;
}
