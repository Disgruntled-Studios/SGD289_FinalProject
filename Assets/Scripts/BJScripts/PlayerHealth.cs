using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
 
    public UnitHealth Health { get; private set; }

    public float CurrentHealth => Health.CurrentHealth;
    
    private void Awake()
    {
        Health = new UnitHealth(_maxHealth);
    }

    public void TakeDamage(float amount)
    {
        Health.Damage(amount);
    }

    public void Heal(float amount)
    {
        Health.Heal(amount);
    }
}
