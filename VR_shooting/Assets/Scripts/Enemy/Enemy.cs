using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private EnemyMovementForLogic enemyMovements;
    [SerializeField]
    private EnemyNullMovementStrategy noMovementStrategy;
    [SerializeField]
    private Transform playerTransform;
    private EnemyLogic logic;

    private void Start()
    {
        logic = new EnemyLogic(enemyMovements, noMovementStrategy, transform, playerTransform.position);
    }

    private void Update()
    {
        logic.UpdateLogic();
    }

    private void OnTriggerEnter(Collider other)
    {
        logic.OnTriggerEnter(other);
    }
}
