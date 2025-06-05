using UnityEngine;

public class L3EnemyController : MonoBehaviour
{
    private UnitHealth _health;
    
    private void Start()
    {
        _health = new UnitHealth(10);
    }

    public void TakeDamage(float amount)
    {
        _health.Damage(amount);
    }
}
