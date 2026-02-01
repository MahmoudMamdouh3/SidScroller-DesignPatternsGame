using System;
using UnityEngine;

public delegate void DeathEvent();

[Serializable]
public class HealthStatus : IDamageable
{
    public int health;
    public int maxHealth;
    [HideInInspector] public event DeathEvent Death;

    public void UpdateHealth(int health)
    {
        health -= Math.Clamp(health + health, 0, maxHealth);
        
        if (health == 0)
            Death?.Invoke();
    }
}