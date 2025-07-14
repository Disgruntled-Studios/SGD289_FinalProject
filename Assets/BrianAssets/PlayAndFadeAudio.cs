using System.Collections;
using UnityEngine;

public class PlayAndFadeAudio : MonoBehaviour
{
    [SerializeField] private AudioSource _source;
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private float _totalDuration = 3f;

    public void PlayWithFade()
    {
        if (!_source || !_source.clip) return;

        _source.volume = 1f;
        _source.Play();
        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        yield return new WaitForSeconds(_totalDuration - _fadeDuration);

        var t = 0f;
        var startVolume = _source.volume;

        while (t < _fadeDuration)
        {
            t += Time.deltaTime;
            _source.volume = Mathf.Lerp(startVolume, 0f, t / _fadeDuration);
            yield return null;
        }

        _source.volume = 0f;
        _source.Stop();
    }
}
