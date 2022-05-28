using UnityEngine;

public abstract class EnemyBaseMovementStrategy : MonoBehaviour
{
    [SerializeField]
    protected float speed = 0;
    public abstract void Move(Transform enemyTranform, Vector3 playerPosition);
}
