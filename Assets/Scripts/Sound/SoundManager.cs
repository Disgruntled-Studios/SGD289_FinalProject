using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;
    public static SoundManager Instance { get; private set; }

    public float MasterVolume { get; private set; } = 1f;
    public float MusicVolume { get; private set; } = 1f;
    public float SfxVolume { get; private set; } = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.outputAudioMixerGroup = s.mixerOutput;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.loop = s.isLooping;
            s.source.spatialBlend = s.spatialBlend;
        }
    }

    public void PlaySfx(string sfxName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == sfxName);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + sfxName + " not found!");
            return;
        }
        s.source.Play();
    }

    public void SetMasterVolume(float value)
    {
        MasterVolume = value;
        // TODO: Apply to audio mixer
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = value;
        // TODO: Apply to audio mixer
    }

    public void SetSfxVolume(float value)
    {
        SfxVolume = value;
        // TODO: Apply to audio mixer
    }
}
