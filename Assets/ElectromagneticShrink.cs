using UnityEngine;

public class ElectromagneticShrink : MonoBehaviour
{
    [SerializeField] private float _shrinkDuration = 3f;

    private Vector3 _initialScale;
    private float _shrinkTimer;
    private bool _isShrinking;

    private void Start()
    {
        _initialScale = transform.localScale;
    }
    
    private void Update()
    {
        if (!_isShrinking) return;

        _shrinkTimer += Time.deltaTime;
        var t = _shrinkTimer / _shrinkDuration;

        transform.localScale = Vector3.Lerp(_initialScale, Vector3.zero, t);

        if (t >= 1f)
        {
            gameObject.SetActive(false);
            _isShrinking = false;
        }
    }

    public void TriggerShrink()
    {
        _isShrinking = true;
        _shrinkTimer = 0f;

        if (RumbleController.Instance)
        {
            RumbleController.Instance.TriggerPatternedRumble(0.6f, _shrinkDuration, RumblePattern.RampDown);
        }
    }
}
