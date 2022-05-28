using UnityEngine;

public class EnemyLogic
{
    private EnemyMovementForLogic enemyMovements;
    private EnemyNullMovementStrategy noMovementStrategy;
    private Transform enemyTransform;
    private Vector3 playerPosition;
    private EnemyMovementForLogic.MovementStrategy moveEnemy;

    public EnemyLogic(EnemyMovementForLogic _enemyMovements, EnemyNullMovementStrategy _noMovement, Transform _enemyTransform, Vector3 _playerPosition)
    {
        enemyMovements = _enemyMovements;
        noMovementStrategy = _noMovement;
        enemyTransform = _enemyTransform;
        playerPosition = _playerPosition;

        moveEnemy = enemyMovements.NormalMovementStrategy;
    }

    public void UpdateLogic()
    {
        moveEnemy(enemyTransform, playerPosition);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Attack();
        }
    }

    private void Attack()
    {
        moveEnemy = noMovementStrategy.Move;
    }
}
