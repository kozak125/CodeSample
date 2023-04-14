using System;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable
{
    private event Action<int> onDamageTaken;

    [SerializeField]
    private HealthBar healthBar;
    [SerializeField]
    private int health = 100;

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
        EventBroker.CallOnGameOver();
    }
}
