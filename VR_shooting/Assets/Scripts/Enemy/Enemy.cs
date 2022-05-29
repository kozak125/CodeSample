using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private EnemyMovementPatterns enemyMovements;
    [SerializeField]
    private EnemyNullMovementStrategy noMovementStrategy;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private int health;
    private int damageFromGun = 10;
    private EnemyLogic logic;

    private void Start()
    {
        logic = new EnemyLogic(enemyMovements, noMovementStrategy, transform, playerTransform.position, health);
        logic.OnDie += OnDie;
    }

    private void Update()
    {
        logic.UpdateLogic();
    }

    private void OnTriggerEnter(Collider other)
    {
        logic.OnTriggerEnter(other);
    }

    public void OnPointerClick()
    {
        RecieveDamage();
    }

    private void RecieveDamage()
    {
        logic.RecieveDamage(damageFromGun);
    }

    private void OnDie()
    {
        Destroy(gameObject);
    }
}
