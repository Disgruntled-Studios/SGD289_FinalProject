using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class GarageDoorController : MonoBehaviour
{
    [SerializeField] private float _openYPos = 5f;
    [SerializeField] private float _openYScale = 0.5803f;

    [SerializeField] private float _animationDuration = 2f;

    [SerializeField] private AnimationCurve _easing = AnimationCurve.Linear(0, 0, 1, 1);

    private bool _hasOpened;
    private Coroutine _animationRoutine;

    [SerializeField] private AudioSource _openSound;
    [SerializeField] private float _fadeDuration = 0.5f;

    public void OpenDoor()
    {
        if (_hasOpened) return;

        _animationRoutine = StartCoroutine(AnimateDoorOpen());
    }

    private IEnumerator AnimateDoorOpen()
    {
        _hasOpened = true;

        var startPos = transform.localPosition;
        var startScale = transform.localScale;
        
        var openPosition = new Vector3(startPos.x, _openYPos, startPos.z);
        var openScale = new Vector3(startScale.x, _openYScale, startScale.z);
        
        var time = 0f;

        if (_openSound)
        {
            _openSound.volume = 1f;
            _openSound.Play();
        }

        while (time < _animationDuration)
        {
            var t = time / _animationDuration;
            var easedT = _easing.Evaluate(t);

            transform.localPosition = Vector3.Lerp(startPos, openPosition, easedT);
            transform.localScale = Vector3.Lerp(startScale, openScale, easedT);

            if (_openSound && time > _animationDuration - _fadeDuration)
            {
                var fadeT = (_animationDuration - time) / _fadeDuration;
                _openSound.volume = Mathf.Clamp01(fadeT);
            }

            time += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = openPosition;
        transform.localScale = openScale;

        if (_openSound && _openSound.isPlaying)
        {
            _openSound.Stop();
        }
    }
}
