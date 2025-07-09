using UnityEngine;

public class LightBlink : MonoBehaviour
{
    [SerializeField] private float _blinkInterval = 0.5f;

    private Light _light;
    private float _timer;

    private void Awake()
    {
        _light = GetComponent<Light>();
        _timer = _blinkInterval;
    }

    private void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            _light.enabled = !_light.enabled;
            _timer = _blinkInterval;
        }
    }
}
