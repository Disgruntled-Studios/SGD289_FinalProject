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
    Error
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

        if (!clip) return;

        _audioSource?.PlayOneShot(clip);
    }
}
