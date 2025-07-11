using System.Collections;
using UnityEngine;

public class GarageDoorController : MonoBehaviour
{
    private const float OpenYPos = 5f;
    private const float OpenYScale = 0.5803f;

    private const float AnimationDuration = 2f;

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
        
        var openPosition = new Vector3(startPos.x, OpenYPos, startPos.z);
        var openScale = new Vector3(startScale.x, OpenYScale, startScale.z);
        
        var time = 0f;

        while (time < AnimationDuration)
        {
            var t = time / AnimationDuration;
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
