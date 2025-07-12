using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class GarageDoorController : MonoBehaviour
{
    [FormerlySerializedAs("OpenYPos")] [SerializeField] private float _openYPos = 5f;
    [FormerlySerializedAs("OpenYScale")] [SerializeField] private float _openYScale = 0.5803f;

    [SerializeField] private float _animationDuration = 2f;

    [SerializeField] private AnimationCurve _easing = AnimationCurve.Linear(0, 0, 1, 1);

    private bool _hasOpened;
    private Coroutine _animationRoutine;

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

        while (time < _animationDuration)
        {
            var t = time / _animationDuration;
            var easedT = _easing.Evaluate(t);

            

            transform.localPosition = Vector3.Lerp(startPos, openPosition, easedT);
            transform.localScale = Vector3.Lerp(startScale, openScale, easedT);

            time += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = openPosition;
        transform.localScale = openScale;
    }
}
