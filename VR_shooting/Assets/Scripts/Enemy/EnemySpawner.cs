using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> enemiesToSpawn;

    private Transform playerTransform;
    private float timer = 0f;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 3)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        var newEnemy = Instantiate(enemiesToSpawn[0], SetEnemyPosition(), Quaternion.identity);
        SetEnemyRotation(newEnemy.transform);
    }

    private Vector3 SetEnemyPosition()
    {
        // edge case of Vector2(0, 0) position spawn
        var enemyPosition = Random.insideUnitCircle.normalized * 40f;
        return new Vector3(enemyPosition.x, 0, enemyPosition.y);
    }

    private void SetEnemyRotation(Transform enemyTransform)
    {
        enemyTransform.LookAt(playerTransform);
    }
}
