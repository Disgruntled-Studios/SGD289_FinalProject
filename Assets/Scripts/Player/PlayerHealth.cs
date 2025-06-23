using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;

    public UnitHealth Health { get; private set; }

    public float CurrentHealth => Health.CurrentHealth;

    public UnityEvent onDeath;
    private Animator animator;

    private void Awake()
    {
        Health = new UnitHealth(_maxHealth);
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float amount)
    {
        Health.Damage(amount);
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
