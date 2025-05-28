using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    [Tooltip("The name of this Sound element.")]
    public string name;

    [Tooltip("The audio clip that this sound will play.")]
    public AudioClip clip;

    [Tooltip("The audio clip that this sound will play.")]
    public AudioMixerGroup mixerOutput;

    [Tooltip("The audio clip that this sound will play.")]
    public bool isLooping;

    [Tooltip("Sets the overall volume of the sound.")]
    [Range(0f, 1f)]
    public float volume;

    [Tooltip("Sets the frequency of the sound. Use this to speed up or slowdown the sound.")]
    [Range(.1f, 3f)]
    public float pitch;

    [HideInInspector]
    public AudioSource source;
}
