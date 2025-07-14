using System.Collections;
using UnityEngine;

public class EvacuationWarningLooper : MonoBehaviour
{
    [SerializeField] private AudioSource _source;
    [SerializeField] private float _delay = 4f;
    [SerializeField] private float _fadeDuration = 0.5f;

    private Coroutine _loopRoutine;
    private Coroutine _fadeRoutine;
    private bool _isPlaying;

    private float _startingVolume;

    [SerializeField] private bool _shouldPlayOnStart;
    
    private void Start()
    {
        if (!_source || !_source.clip) return;

        _source.playOnAwake = false;
        _source.loop = false;
        _startingVolume = _source.volume;

        if (_shouldPlayOnStart)
        {
            _source.Play();
        }

        StartCoroutine(BeginLoopAfterInitial());
    }

    private IEnumerator BeginLoopAfterInitial()
    {
        yield return new WaitForSeconds(_source.clip.length + _delay);
        StartLoop();
    }

    public void StartLoop()
    {
        if (_loopRoutine != null || _isPlaying) return;
        _isPlaying = true;
        _loopRoutine = StartCoroutine(Loop());
    }

    public void StopLoop()
    {
        if (_loopRoutine == null || !_isPlaying) return;
        StopCoroutine(_loopRoutine);
        _loopRoutine = null;
        _isPlaying = false;

        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
        }

        _fadeRoutine = StartCoroutine(FadeOut());
    }

    private IEnumerator Loop()
    {
        while (true)
        {
            _source.volume = 0f;
            _source.Play();

            if (_fadeRoutine != null)
            {
                StopCoroutine(_fadeRoutine);
            }

            _fadeRoutine = StartCoroutine(FadeIn());

            yield return new WaitForSeconds(_source.clip.length + _delay);
        }
    }

    private IEnumerator FadeIn()
    {
        var t = 0f;
        var initialVolume = _source.volume;
        
        while (t < _fadeDuration)
        {
            t += Time.deltaTime;
            _source.volume = Mathf.Lerp(initialVolume, _startingVolume, t / _fadeDuration);
            yield return null;
        }

        _source.volume = _startingVolume;
    }

    private IEnumerator FadeOut()
    {
        var t = 0f;
        var initialVolume = _source.volume;

        while (t < _fadeDuration)
        {
            t += Time.deltaTime;
            _source.volume = Mathf.Lerp(initialVolume, 0f, t / _fadeDuration);
            yield return null;
        }

        _source.volume = 0f;

        if (_source.isPlaying)
        {
            _source.Stop();
        }
    }

    public void ResetLoop()
    {
        StopLoop();
        StartLoop();
    }
}
