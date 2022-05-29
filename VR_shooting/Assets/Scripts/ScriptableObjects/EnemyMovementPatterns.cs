using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Enemy Movement Patterns")]
public class EnemyMovementPatterns : ScriptableObject
{
    [SerializeField]
    private EnemyBaseMovementStrategy normalMovementStrategy;
    [SerializeField]
    private EnemyBaseMovementStrategy shotMovementStrategy;
    [SerializeField]
    private EnemyBaseMovementStrategy erraticMovementStrategy;

    public delegate void MovementStrategy(Transform enemyTransform, Vector3 playerPosition);
    public MovementStrategy NormalMovementStrategy => normalMovementStrategy.Move;
    public MovementStrategy ShotMovementStrategy => shotMovementStrategy.Move;
    public MovementStrategy ErraticMovementStrategy => erraticMovementStrategy.Move;
}
