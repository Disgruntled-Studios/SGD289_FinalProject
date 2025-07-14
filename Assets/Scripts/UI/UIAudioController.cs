using System.Collections.Generic;
using UnityEngine;

public enum UISound
{
    TabNav,
    InventoryNav,
    InventorySubmit,
    SliderAdjust,
    Toggle,
    Button,
    Open,
    Close,
    Error,
    DigitAdjust,
    TileRotate,
    TileConnected,
    TileDisconnected,
    CircuitComplete
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
}
