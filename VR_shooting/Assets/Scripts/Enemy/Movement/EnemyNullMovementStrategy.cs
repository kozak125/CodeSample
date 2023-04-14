using UnityEngine;

namespace VRShooter.Enemies
{
    public sealed class EnemyNullMovementStrategy : EnemyBaseMovementStrategy
    {
        public override void Move(Transform enemyTranform, Vector3 playerPosition)
        {
            // don't move
        }
    }
}
