﻿using UnityEngine;
using System;

namespace VRShooter.Enemies
{
    public class EnemyLogic
    {
        public event Action OnDie;
        public event Action<IDamagable> OnAttacking;

        private Vector3 playerPosition;
        private int currentHealth;
        private EnemyMovementPatterns.MovementStrategy moveEnemy;
        private bool isAttacking = false;

        private readonly EnemyMovementPatterns enemyMovements;
        private readonly EnemyMovementPatterns.MovementStrategy noMovementStrategy;
        private readonly Transform enemyTransform;
        private readonly int maxHealth;

        public EnemyLogic(EnemyMovementPatterns _enemyMovements, EnemyNullMovementStrategy _noMovement, Transform _enemyTransform, Vector3 _playerPosition, int _health)
        {
            enemyMovements = _enemyMovements;
            noMovementStrategy = _noMovement.Move;
            enemyTransform = _enemyTransform;
            playerPosition = _playerPosition;
            currentHealth = maxHealth = _health;

            ChangeMovementStrategy(enemyMovements.NormalMovementStrategy);
        }

        public void RecieveDamage(int damageAmount)
        {
            currentHealth -= damageAmount;
            CheckForDeath();
        }

        public void UpdateLogic()
        {
            moveEnemy(enemyTransform, playerPosition);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IDamagable damagable))
            {
                Attack(damagable);
            }
        }

        public void OnEnabled()
        {
            moveEnemy = enemyMovements.NormalMovementStrategy;
            currentHealth = maxHealth;
        }

        private void ChangeMovementStrategy(EnemyMovementPatterns.MovementStrategy movementStrategy)
        {
            if (!isAttacking)
            {
                moveEnemy = movementStrategy;
            }
        }

        private void CheckForDeath()
        {
            if (currentHealth <= 0)
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

        private void Attack(IDamagable objectToAttack)
        {
            moveEnemy = noMovementStrategy;
            isAttacking = true;
            OnAttacking.Invoke(objectToAttack);
        }
    }
}
