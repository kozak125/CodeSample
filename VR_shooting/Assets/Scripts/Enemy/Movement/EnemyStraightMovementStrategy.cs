using UnityEngine;

namespace VRShooter.Enemy
{
    public class EnemyStraightMovementStrategy : EnemyBaseMovementStrategy
    {
        public override void Move(Transform enemyTransform, Vector3 playerPosition)
        {
            var step = speed * Time.deltaTime;
            var newPosition = Vector3.MoveTowards(enemyTransform.position, playerPosition, step);
            enemyTransform.position = new Vector3(newPosition.x, 0.5f, newPosition.z);
        }
    }
}
