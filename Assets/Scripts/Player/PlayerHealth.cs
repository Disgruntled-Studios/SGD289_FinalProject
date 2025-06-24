using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;

    public UnitHealth Health { get; private set; }

    public float CurrentHealth => Health.CurrentHealth;

    public UnityEvent onDeath;

    private void Awake()
    {
        Health = new UnitHealth(_maxHealth);
    }

    void Start()
    {
        UIManager.Instance.UpdateHealthText(Health.CurrentHealth);
    }

    public void TakeDamage(float amount)
    {
        Health.Damage(amount);
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealthText(Health.CurrentHealth);
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
