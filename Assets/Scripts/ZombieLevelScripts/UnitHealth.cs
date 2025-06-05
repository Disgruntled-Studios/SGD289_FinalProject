using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class UnitHealth
{
    private float _maxHealth, _currentHealth;
    private bool isInvincible;

    public UnityEvent onDamageEvent;

    public float MaxHealth
    {
        get => _maxHealth;

        set
        {
            _maxHealth = value switch
            {
                >= 1000 => 1000,
                <= 0 => 1,
                _ => value
            };
        }
    }

    public float CurrentHealth
    {
        get => _currentHealth;

        set
        {
            if (value >= _maxHealth)
            {
                _currentHealth = _maxHealth;
            }
            else if (value <= 0)
            {
                _currentHealth = 0;
            }
            else
            {
                _currentHealth = value;
            }
        }
    }

    public bool IsDead => _currentHealth <= 0;

    /// <summary>
    /// Holds information/Functions on this unit's health system.
    /// </summary>
    /// <param name="maxHealth">How much health can possibly have and will the unit start with once this instance is created.</param>
    public UnitHealth(float maxHealth)
    {
        _maxHealth = maxHealth;
        _currentHealth = _maxHealth;
    }
    
    /// <summary>
    /// Holds information/Functions on this unit's health system.
    /// </summary>
    /// <param name="maxHealth">How much health can possibly have and will the unit start with once this instance is created.</param>
    public UnitHealth(float maxHealth, UnityEvent _onDamage)
    {
        _maxHealth = maxHealth;
        _currentHealth = _maxHealth;
        onDamageEvent = _onDamage;
    }

    /// <summary>
    /// Holds information/Functions on this unit's health system.
    /// </summary>
    /// <param name="maxHealth">How much health can possibly have and will the unit start with once unit is instantiated.</param>
    /// <param name="currentHealth">Directly set the current health when the unit is instantiated.</param>
    public UnitHealth(float maxHealth, float currentHealth, UnityEvent _onDamage)
    {
        _maxHealth = maxHealth;
        _currentHealth = currentHealth;
        onDamageEvent = _onDamage;
    }

    /// <summary>
    /// This function will damage the unit's health.
    /// </summary>
    /// <param name="damageAmount">How much will the function damage the unit.</param>
    public void Damage(float damageAmount)
    {
        if (!isInvincible)
        {
            _currentHealth -= damageAmount;
            if (onDamageEvent != null)
            {
                Debug.Log("OnDamageEventInvoking");
                onDamageEvent.Invoke();
            }
        }
    }

    /// <summary>
    /// This function will heal the units health.
    /// </summary>
    /// <param name="healAmount">How much will this function heal the unit.</param>
    public void Heal(float healAmount)
    {
        _currentHealth += healAmount;
    }

    /// <summary>
    /// Resets the current health to the maximum amount set
    /// </summary>
    public void ResetUnitsHealth()
    {
        _currentHealth = _maxHealth;
    }

    /// <summary>
    /// Starts the units invincibility. Make sure to use EndInvincibility to end it.
    /// </summary>
    public void StartInvincibility()
    {
        isInvincible = true;
    }

    /// <summary>
    /// End the units invincibility. Make sure to use StartInvincibility if you want to start it.
    /// </summary>
    public void EndInvincibility()
    {
        isInvincible = false;
    }

}
