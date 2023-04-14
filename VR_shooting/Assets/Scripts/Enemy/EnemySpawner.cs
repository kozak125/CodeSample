using System.Collections.Generic;
using UnityEngine;
using VRShooter.Enemies;

namespace VRShooter
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> enemiesToSpawn;
		[SerializeField]
		private Transform enemiesParent;
        [SerializeField]
		private Transform playerTransform;

		private List<GameObject> pooledEnemies = new List<GameObject>();

        private const float TIME_BETWEEN_SPAWN = 3f;

        private void Start()
        {
            EventBroker.OnGameOver += GameOver;

            InvokeRepeating(nameof(SpawnEnemy), 0f, TIME_BETWEEN_SPAWN);
        }

        private void OnDestroy()
        {
            EventBroker.OnGameOver -= GameOver;
        }

        private void GameOver()
        {
            enabled = false;
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
            newEnemy.GetComponent<Enemy>().Setup(playerTransform);
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
    }
}
