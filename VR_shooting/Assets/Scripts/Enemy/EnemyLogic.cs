using UnityEngine;
using System;

public class EnemyLogic
{
    private EnemyMovementPatterns enemyMovements;
    private EnemyMovementPatterns.MovementStrategy noMovementStrategy;
    private Transform enemyTransform;
    private Vector3 playerPosition;
    private int health;
    private EnemyMovementPatterns.MovementStrategy moveEnemy;
    private bool isAttacking = false;

    public Action OnDie;
    public Action<IDamagable> OnAttacking;

    public EnemyLogic(EnemyMovementPatterns _enemyMovements, EnemyNullMovementStrategy _noMovement, Transform _enemyTransform, Vector3 _playerPosition, int _health)
    {
        enemyMovements = _enemyMovements;
        noMovementStrategy = _noMovement.Move;
        enemyTransform = _enemyTransform;
        playerPosition = _playerPosition;
        health = _health;

        ChangeMovementStrategy(enemyMovements.NormalMovementStrategy);
    }


    private void ChangeMovementStrategy(EnemyMovementPatterns.MovementStrategy movementStrategy)
    {
        if (!isAttacking)
        {
            moveEnemy = movementStrategy;
        }
    }

    public void RecieveDamage(int damageAmount)
    {
        health -= damageAmount;
        CheckForDeath();
    }

    private void CheckForDeath()
    {
        if (health <= 0)
        {
            Die();
            return;
        }

        ChangeMovementStrategy(enemyMovements.ShotMovementStrategy);
    }

    private void Die()
    {
        ChangeMovementStrategy(noMovementStrategy);
        OnDie.Invoke();
    }

    public void UpdateLogic()
    {
        moveEnemy(enemyTransform, playerPosition);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IDamagable player = other.GetComponent<Player>();
            Attack(player);
        }
    }

    private void Attack(IDamagable objectToAttack)
    {
        moveEnemy = noMovementStrategy;
        isAttacking = true;
        OnAttacking.Invoke(objectToAttack);
    }
}
