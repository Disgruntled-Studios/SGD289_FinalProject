using System.Collections.Generic;
using UnityEngine;

public enum UISound
{
    TabNav, // Glass 002
    InventoryNav, // Click 003
    SliderAdjust, // Glass 001
    Toggle, // Click 002
    Button, // Select 004
    Open, // Open 001
    Close, // Close 001
    KeycodeError, // Error 008
    DigitNav, // Glass 002
    TileRotate, // Drop 001
    TileConnected, // Maximize 005
    TileDisconnected, // Minimize 005
    CircuitComplete, // Confirmation 004
    TileNav, // Tick 001
    DigitSuccess // Toggle 002
}

[System.Serializable]
public struct UISoundClip
{
    public UISound type;
    public AudioClip clip;
}

public class UIAudioController : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private List<UISoundClip> _soundClips;

    private readonly Dictionary<UISound, AudioClip> _clipMap = new();

    [SerializeField] [Range(0f, 1f)] private float _uiVolume = 1f;

    private void Awake()
    {
        foreach (var soundClip in _soundClips)
        {
            if (!_clipMap.ContainsKey(soundClip.type))
            {
                _clipMap.Add(soundClip.type, soundClip.clip);
            }
        }
    }

    public void PlaySound(UISound sound)
    {
        if (!_clipMap.TryGetValue(sound, out var clip)) return;

        if (!clip || !_audioSource) return;

        _audioSource?.PlayOneShot(clip, _uiVolume);
    }

    public void SetVolume(float volume)
    {
        _uiVolume = Mathf.Clamp01(volume);
    }

    public void PlaySoundWithPitch(UISound sound, float pitch)
    {
        if (!_clipMap.TryGetValue(sound, out var clip)) return;
        if (!clip || !_audioSource) return;

        _audioSource.pitch = pitch;
        _audioSource.PlayOneShot(clip, _uiVolume);
        _audioSource.pitch = 1f; // Reset
    }
}
