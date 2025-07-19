using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundSequence : MonoBehaviour
{
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _gameMusic;
    [SerializeField] private AudioMixerGroup _musicGroup;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _audioSource.Play();
    }

    private IEnumerator PlaySequence()
    {
        _audioSource.Play();

        yield return new WaitWhile(() => _audioSource.isPlaying);

        _audioSource.clip = _gameMusic;
        _audioSource.outputAudioMixerGroup = _musicGroup;
        _audioSource.Play();
    }
}
