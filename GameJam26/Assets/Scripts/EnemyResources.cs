using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyResources : MonoBehaviour
{
    public int MaxHealth = 2;
    private int _health;
    public int Health
    {
        get { return _health; }
        set
        {
            _health = Mathf.Clamp(value, 0, MaxHealth);
            HealthChanged?.Invoke();
        }
    }

    public UnityAction HealthChanged;

    private void Awake()
    {
        Health = MaxHealth;
    }

    public void Damage(int amount = 1)
    {
        Health -= amount;
    }
}
