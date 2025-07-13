using UnityEngine;
using UnityEngine.Audio;

public static class SoundUtility
{
    public static void PlayClipAtPoint(
        AudioClip clip,
        Vector3 position,
        float volume = 1f,
        float pitch = 1f,
        AudioMixerGroup mixerGroup = null,
        float spatialBlend = 0f,
        float minDistance = 1f,
        float maxDistance = 15f
    )
    {
        if (clip == null) return;

        var tempGo = new GameObject("OneShotAudio")
        {
            transform =
            {
                position = position
            }
        };

        var source = tempGo.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.outputAudioMixerGroup = mixerGroup;
        source.spatialBlend = spatialBlend;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.Play();

        Object.Destroy(tempGo, clip.length / pitch);
    }
}
