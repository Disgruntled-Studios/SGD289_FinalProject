using UnityEngine;
using UnityEngine.UI;

public class HeartBeat : MonoBehaviour
{
    [SerializeField] private Image _heartImage;
    [SerializeField] private RectTransform _heartTransform;
    [SerializeField] private AnimationCurve _beatCurve;
    [SerializeField] private float[] _beatDurations = { 0.2f, 0.5f, 1.0f };

    private int HitsRemaining => GameManager.Instance.Player.GetComponent<PlayerHealth>().HitsRemaining;
    private float _timer;
    
    private void Update()
    {
        if (HitsRemaining <= 0)
        {
            _heartTransform.localScale = Vector3.one;
            _heartImage.color = Color.black;
            return;
        }

        var index = Mathf.Clamp(HitsRemaining - 1, 0, 2);
        var beatDuration = _beatDurations[index];

        _timer += Time.unscaledDeltaTime;

        var t = (_timer % beatDuration) / beatDuration;
        var scale = _beatCurve.Evaluate(t);

        _heartTransform.localScale = Vector3.one * scale;
    }
    
    
}
