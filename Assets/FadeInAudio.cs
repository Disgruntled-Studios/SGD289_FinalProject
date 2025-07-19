using System.Collections;
using UnityEngine;

public class FadeInAudio : MonoBehaviour
{
    [SerializeField] private float _fadeDuration = 5f;
    [SerializeField] private AudioSource _audioSource;

    private void Start()
    {
        if (!_audioSource) return;
        if (!_audioSource?.clip) return;

        _audioSource.volume = 0f;
        _audioSource.Play();

        StartCoroutine(FadeIn());
    }
    
    private IEnumerator FadeIn()
    {
        var targetVolume = 0.5f;
        var currentTime = 0f;

        while (currentTime < _fadeDuration)
        {
            currentTime += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(0f, targetVolume, currentTime / _fadeDuration);
            yield return null;
        }

        _audioSource.volume = targetVolume;
    }
}
