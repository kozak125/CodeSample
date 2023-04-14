using System.Collections;
using UnityEngine;

namespace VRShooter.Enemy
{
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
        [SerializeField]
        private int attackDamage = 10;
        [SerializeField]
        private int pointsForKilling = 10;
        [SerializeField]
        private int damageFromGun = 10;
        private Transform playerTransform;
        private EnemyLogic logic;

        private void Start()
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            logic = new EnemyLogic(enemyMovements, noMovementStrategy, transform, playerTransform.position, health);
            logic.OnAttacking += Attack;
            logic.OnDie += Die;
            EventBroker.OnGameOver += GameOver;
        }

        private void Attack(IDamagable objectToDamage) => StartCoroutine(AttackInIntervals(objectToDamage));

        private IEnumerator AttackInIntervals(IDamagable objectToDamage)
        {
            var attacksInterval = new WaitForSeconds(attackSpeed);
            while (true)
            {
                objectToDamage.OnDamageReceived(attackDamage);
                yield return attacksInterval;
            }
        }

        private void Die()
        {
            gameObject.SetActive(false);
            EventBroker.CallOnEnemyDestroyed(pointsForKilling);
        }

        private void GameOver()
        {
            gameObject.SetActive(false);
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

        private void OnEnable()
        {
            if (logic != null)
            {
                logic.OnEnabled();
            }
        }

        private void OnDestroy()
        {
            EventBroker.OnGameOver -= GameOver;
        }
    }
}
