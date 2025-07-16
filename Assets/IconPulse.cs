using UnityEngine;
using UnityEngine.UI;

public class IconPulse : MonoBehaviour
{
    [SerializeField] private float _pulseSpeed = 2f;
    [SerializeField] private float _minAlpha = 0.4f;
    [SerializeField] private float _maxAlpha = 1f;

    private Image _image;
    private Color _originalColor;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _originalColor = _image.color;
    }

    private void Start()
    {
        var color = _image.color;
        color.a = _maxAlpha;
        _image.color = color;
    }

    private void Update()
    {
        var time = Time.time * _pulseSpeed;
        var t = (Mathf.Sin(time + Mathf.PI / 2f) + 1f) / 2f;
        var alpha = Mathf.Lerp(_minAlpha, _maxAlpha, t);

        var color = _originalColor;
        color.a = alpha;
        _image.color = color;
    }

}
