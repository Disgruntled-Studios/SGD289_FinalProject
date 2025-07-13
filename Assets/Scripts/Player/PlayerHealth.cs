//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
//using UnityEngine.LightTransport.PostProcessing;
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

    private float damagedTimer;

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

        // if (Mathf.Approximately(Health.CurrentHealth, 2.0f))
        // {
        //     _vignette.intensity.value = FirstHitIntensity;
        //     HitsRemaining--;
        //     damagedTimer = 8f;
        // }
        // else if (Mathf.Approximately(Health.CurrentHealth, 1.0f))
        // {
        //     _vignette.intensity.value = SecondHitIntensity;
        //     IsInjured = true;
        //     _animController.SetInjured(IsInjured);
        //     HitsRemaining--;
        //     damagedTimer = 8f;

        // }
        // else if (Mathf.Approximately(Health.CurrentHealth, 0.0f))
        // {
        //     _vignette.intensity.value = ThirdHitIntensity;
        //     HitsRemaining--;
        //     damagedTimer = -1f;
        // }
        
    }

    public void Heal(float amount)
    {
        Health.Heal(amount);
    }

    void Update()
    {
        if (damagedTimer > 0)
        {
            damagedTimer -= 1 * Time.deltaTime;
            //_vignette.intensity.value = Mathf.Lerp(0, 1, damagedTimer / 8);
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
                        _animController.SetInjured(false);
                    }
                    break;
            }
        }
    }

    void FixedUpdate()
    {
        if (HitsRemaining <= 0 && !GameManager.Instance.IsGameOver)
        {
            Debug.Log("Dead");
            onDeath.Invoke();
        }
    }



    public void ResetVignette()
    {
        _vignette.intensity.value = 0f;
        HitsRemaining = 3;
        Health.CurrentHealth = Health.MaxHealth;
    }
}
