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
            if (s.isLooping)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.outputAudioMixerGroup = s.mixerOutput;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = true;
                s.source.spatialBlend = s.spatialBlend;
                s.source.minDistance = s.minDistance;
                s.source.maxDistance = s.maxDistance;
                s.source.rolloffMode = AudioRolloffMode.Logarithmic;
            }
        }
    }

    public void PlaySFX(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;

        var s = Array.Find(sounds, sound => sound.name == name);
        if (s == null || !s.clip) return;

        if (s.isLooping && s.source)
        {
            s.source.Play();
        }
        else
        {
            SoundUtility.PlayClipAtPoint(s.clip, transform.position, s.volume, s.pitch, s.mixerOutput, s.spatialBlend,
                s.minDistance, s.maxDistance);
        }
    }

    public void StopSFX(string name)
    {
        var s = Array.Find(sounds, sound => sound.name == name);
        if (s?.source?.isPlaying == true)
        {
            s.source.Stop();
        }
    }
}
