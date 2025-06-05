using System;
using UnityEngine;
using UnityEngine.Events;

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

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("L3Bullet"))
        {
            TakeDamage(10);
            Debug.Log(_health.CurrentHealth);
            if (_health.IsDead)
            {
                Debug.Log("I'm dead");
            }
        }
    }
}
