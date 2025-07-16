using UnityEngine;
using System;
using UnityEngine.Audio;
using System.Collections;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;
    public static SoundManager Instance { get; private set; }

    public AudioMixer mainMixer;

    // public float MasterVolume { get; private set; } = 1f;
    // public float MusicVolume { get; private set; } = 1f;
    // public float SfxVolume { get; private set; } = 1f;
    // public float AmbianceVolume { get; private set; } = 1f;

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
            return;
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

    void Start()
    {
        if (PlayerPrefs.HasKey("MasterVolume") || PlayerPrefs.HasKey("MusicVolume") || PlayerPrefs.HasKey("SFXVolume") || PlayerPrefs.HasKey("AmbianceVolume"))
        {
            mainMixer.SetFloat("Master", Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume")) * 20);
            mainMixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume")) * 20);
            mainMixer.SetFloat("SFX", Mathf.Log10(PlayerPrefs.GetFloat("SFXVolume")) * 20);
            mainMixer.SetFloat("Ambiance", Mathf.Log10(PlayerPrefs.GetFloat("AmbianceVolume")) * 20);
        }
        else
        {
            mainMixer.SetFloat("Master", Mathf.Log10(0.5f) * 20);
            mainMixer.SetFloat("Music", Mathf.Log10(0.5f) * 20);
            mainMixer.SetFloat("SFX", Mathf.Log10(0.5f) * 20);
            mainMixer.SetFloat("Ambiance", Mathf.Log10(0.5f) * 20);
            
            PlayerPrefs.SetFloat("MasterVolume", 0.5f);
            PlayerPrefs.SetFloat("MusicVolume", 0.5f);
            PlayerPrefs.SetFloat("SFXVolume", 0.5f);
            PlayerPrefs.SetFloat("AmbianceVolume", 0.5f);
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

    public void FadeInSFX(string sfxName, float fadeTime = 1f)
    {
        Sound s = Array.Find(sounds, sound => sound.name == sfxName);
        s.source.volume = 0;
        s.source.Play();
        if (s == null)
        {
            Debug.LogError("Sound: " + sfxName + " not found cannot play sfx!");
            return;
        }
        StartCoroutine(FadeSound(fadeTime, s, true));
    }

    public void FadeInSFX(string sfxName, float startTime, float fadeTime = 1f)
    {
        Sound s = Array.Find(sounds, sound => sound.name == sfxName);
        s.source.volume = 0;
        s.source.time = startTime;
        s.source.Play();
        if (s == null)
        {
            Debug.LogError("Sound: " + sfxName + " not found cannot play sfx!");
            return;
        }
        StartCoroutine(FadeSound(fadeTime, s, true));
    }

    public void FadeOutSFX(string sfxName, float fadeTime = 1f)
    {
        Sound s = Array.Find(sounds, sound => sound.name == sfxName);
        s.source.volume = s.volume;
        if (s == null)
        {
            Debug.LogError("Sound: " + sfxName + " not found cannot play sfx!");
            return;
        }
        StartCoroutine(FadeSound(fadeTime, s, false));
    }

    private IEnumerator FadeSound(float fadeTime, Sound s, bool isFadeIn)
    {
        float currentTime = fadeTime;

        if (isFadeIn)
        {
            Debug.Log("Starting FadeIn");
            while (s.source.volume < s.volume)
            {
                currentTime -= 1 * Time.deltaTime;
                float percentage = currentTime / fadeTime;

                s.source.volume = Mathf.Lerp(s.volume, 0, percentage);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            Debug.Log("Starting FadeOut");
            while (s.source.volume > 0)
            {
                currentTime -= 1 * Time.deltaTime;
                float percentage = currentTime / fadeTime;

                s.source.volume = Mathf.Lerp(0, s.volume, percentage);
                yield return new WaitForEndOfFrame();
            }
            s.source.Stop();
            s.source.time = 0;
        }

    }

    // public void SetMasterVolume(float value)
    // {
    //     MasterVolume = value;
    //     // TODO: Apply to audio mixer
    // }

    // public void SetMusicVolume(float value)
    // {
    //     MusicVolume = value;
    //     // TODO: Apply to audio mixer
    // }

    // public void SetSfxVolume(float value)
    // {
    //     SfxVolume = value;
    //     // TODO: Apply to audio mixer
    // }

    // public void SetAmbianceVolume(float value)
    // {
    //     AmbianceVolume = value;
    //     // TODO: Apply to audio mixer
    // }


}
