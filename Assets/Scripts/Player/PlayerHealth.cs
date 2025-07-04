using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.LightTransport.PostProcessing;
using UnityEngine.Rendering.Universal;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private PlayerAnimationController _animController;
    
    private const float MaxHealth = 3.0f;

    [SerializeField] private Volume _volume;
    private Vignette _vignette;
    public Vignette Vignette => _vignette;

    [SerializeField] private GameObject _vignetteObject;
    public GameObject VignetteObject => _vignetteObject;

    private const float FirstHitIntensity = 0.4f;
    private const float SecondHitIntensity = 0.55f;
    private const float ThirdHitIntensity = 1.0f;
    
    public UnitHealth Health { get; private set; }

    public float CurrentHealth => Health.CurrentHealth;

    public UnityEvent onDeath;
    
    public bool IsInjured { get; private set; }

    public int HitsRemaining { get; private set; } = 3;

    private void Awake()
    {
        Health = new UnitHealth(MaxHealth);
        _volume.profile.TryGet(out _vignette);
    }

    void Start()
    {
        _vignette.intensity.value = 0f;
    }

    [ContextMenu("Take Damage")]
    public void TakeDamage()
    {
        const float amount = 1.0f;
        Health.Damage(amount);

        if (Mathf.Approximately(Health.CurrentHealth, 2.0f))
        {
            _vignette.intensity.value = FirstHitIntensity;
            HitsRemaining--;
        }
        else if (Mathf.Approximately(Health.CurrentHealth, 1.0f))
        {
            _vignette.intensity.value = SecondHitIntensity;
            IsInjured = true;
            _animController.SetInjured(IsInjured);
            HitsRemaining--;

        }
        else if (Mathf.Approximately(Health.CurrentHealth, 0.0f))
        {
            _vignette.intensity.value = ThirdHitIntensity;
            HitsRemaining--;
        }
    }

    public void Heal(float amount)
    {
        Health.Heal(amount);
    }

    void FixedUpdate()
    {
        if (Health.IsDead)
        {
            onDeath.Invoke();
        }
    }
}
