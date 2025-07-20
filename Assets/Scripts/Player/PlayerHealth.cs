using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private PlayerAnimationController _animController;
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] private AudioSource _hitAudio;

    private const float MaxHealth = 3.0f;

    [SerializeField] private Volume _volume;
    private Vignette _vignette;
    public Vignette Vignette => _vignette;

    [SerializeField] private GameObject _vignetteObject;
    public GameObject VignetteObject => _vignetteObject;

    private const float FirstHitIntensity = 0.4f;
    private const float SecondHitIntensity = 0.55f;
    private const float ThirdHitIntensity = 1.0f;

    // public UnitHealth Health { get; private set; }

    public UnityEvent onDeath;

    public bool IsDead { get; private set; }
    public bool IsInjured { get; private set; }

    public int HitsRemaining { get; private set; } = 3;

    private float damagedTimer;

    private void Awake()
    {
        IsDead = false;
        _volume.profile.TryGet(out _vignette);
    }

    void Start()
    {
        _vignette.intensity.value = 0f;
    }

    // TESTING METHOD
    [ContextMenu("Kill Player")]
    public void KillPlayer()
    {
        for (var i = 0; i < 3; i++)
        {
            TakeDamage();
        }
    }

    [ContextMenu("Take Damage")]
    public void TakeDamage()
    {
        // const float amount = 1.0f;
        // Health.Damage(amount);
        
        GameManager.Instance.PlayerController.IsMovementLocked = true;
        _animController.TriggerHit();

        _impulseSource?.GenerateImpulse();

        _hitAudio.PlayOneShot(_hitAudio.clip);
        
        RumbleController.Instance?.TriggerPresetRumble(RumblePreset.DamageImpact);

        switch (HitsRemaining)
        {
            case 3:
                _vignette.intensity.value = FirstHitIntensity;
                HitsRemaining--;
                damagedTimer = 8f;
                break;
            case 2:
                _vignette.intensity.value = SecondHitIntensity;
                IsInjured = true;
                _animController.SetInjured(IsInjured);
                HitsRemaining--;
                damagedTimer = 8f;
                break;
            case 1:
                _vignette.intensity.value = ThirdHitIntensity;
                HitsRemaining--;
                damagedTimer = -1f;
                break;
        }
    }

    public void Heal(float amount)
    {
        // Health.Heal(amount);
    }

    void Update()
    {
        if (damagedTimer > 0)
        {
            damagedTimer -= 1 * Time.deltaTime;
            switch (damagedTimer)
            {
                case <= 0:
                    if (HitsRemaining < 3)
                    {
                        HitsRemaining = 3;
                        _vignette.intensity.value = 0f;
                    }
                    break;
                case <= 3.2f:
                    if (HitsRemaining < 2)
                    {
                        HitsRemaining = 2;
                        _vignette.intensity.value = FirstHitIntensity;
                        IsInjured = false;
                        _animController.SetInjured(IsInjured);
                    }
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (HitsRemaining <= 0 && !GameManager.Instance.IsGameOver && !IsDead)
        {
            _animController.SetDeathTrigger();
            IsDead = true;
        }
    }

    public void CallOnDeath()
    {
        onDeath?.Invoke();
    }
    
    public void ResetHealth()
    {
        _vignette.intensity.value = 0f;
        HitsRemaining = 3;
        IsDead = false;
        IsInjured = false;
        _animController.SetInjured(IsInjured);
    }
}
