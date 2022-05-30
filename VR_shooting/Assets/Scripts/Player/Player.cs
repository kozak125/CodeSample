using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable
{
    [SerializeField]
    private HealthBar healthBar;
    [SerializeField]
    private int health = 100;

    private Action<int> onDamageTaken;


    public Action OnAttacked;

    private void Start()
    {
        healthBar.Setup(health, ref onDamageTaken);
    }

    public void GetDamaged(int damageAmout)
    {
        health -= damageAmout;
        onDamageTaken?.Invoke(damageAmout);

        CheckForDeath();
    }

    private void CheckForDeath()
    {
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        EventBroker.callOnGameOver();
    }
}
