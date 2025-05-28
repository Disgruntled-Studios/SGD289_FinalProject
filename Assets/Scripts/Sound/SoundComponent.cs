using System;
using UnityEngine;

public class SoundComponent : MonoBehaviour
{
    public Sound[] sounds;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
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
        s.source.Play();
    }

}
