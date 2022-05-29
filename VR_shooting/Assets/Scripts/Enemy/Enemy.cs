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
    private int health;
    [SerializeField]
    private float attackSpeed;
    private int damageFromGun = 10;
    private Transform playerTransform;
    private EnemyLogic logic;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        logic = new EnemyLogic(enemyMovements, noMovementStrategy, transform, playerTransform.position, health);
        logic.OnAttacking += Attack;
        logic.OnDie += OnDie;
    }

    private void Attack(IDamagable objectToDamage) => StartCoroutine(AttackInIntervals(objectToDamage));

    private IEnumerator AttackInIntervals(IDamagable objectToDamage)
    {
        var attacksInterval = new WaitForSeconds(attackSpeed);
        while (true)
        {
            objectToDamage.GetDamaged();
            yield return attacksInterval;
        }
    }

    private void OnDie()
    {
        Destroy(gameObject);
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
}
