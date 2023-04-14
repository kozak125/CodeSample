using System.Collections.Generic;
using UnityEngine;

namespace VRShooter
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> enemiesToSpawn;
        [SerializeField]
        private Transform enemiesParent;

        private Transform playerTransform;
        private float timer = 0f;
        private List<GameObject> pooledEnemies = new List<GameObject>();

        private const float TIME_BETWEEN_SPAWN = 3f;

        private void Start()
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            EventBroker.OnGameOver += GameOver;
        }

        private void GameOver()
        {
            enabled = false;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer > TIME_BETWEEN_SPAWN)
            {
                SpawnEnemy();
                timer = 0f;
            }
        }

        private void SpawnEnemy()
        {
            var enemy = GetEnemy();
            SetEnemyPosition(enemy.transform);
            SetEnemyRotation(enemy.transform);
        }

        private GameObject GetEnemy()
        {
            foreach (var enemy in pooledEnemies)
            {
                if (!enemy.activeSelf)
                {
                    enemy.SetActive(true);
                    return enemy;
                }
            }

            return CreateNewEnemy();
        }

        private GameObject CreateNewEnemy()
        {
            var newEnemy = Instantiate(enemiesToSpawn[0], enemiesParent);
            pooledEnemies.Add(newEnemy);
            return newEnemy;
        }

        private void SetEnemyPosition(Transform enemyTransform)
        {
            // edge case of Vector2(0, 0) position spawn
            var enemyPosition = Random.insideUnitCircle.normalized * 40f;
            enemyTransform.position = new Vector3(enemyPosition.x, 0, enemyPosition.y);
        }

        private void SetEnemyRotation(Transform enemyTransform)
        {
            enemyTransform.LookAt(playerTransform);
        }

        private void OnDestroy()
        {
            EventBroker.OnGameOver -= GameOver;
        }
    }
}
