using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;
    public static SoundManager Instance { get; set; }

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
            s.source.spatialBlend = 1;
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }
}
