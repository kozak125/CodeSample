using UnityEngine;
using VRShooter.Player.UI;

namespace VRShooter.Player
{
    public class Player : MonoBehaviour, IDamagable
    {
        [SerializeField]
        private PlayerHealthBar healthBar;
        [SerializeField]
        private int health = 100;

        private void Start()
        {
            healthBar.Setup(health);
        }

        public void GetDamaged(int damageAmout)
        {
            health -= damageAmout;
            healthBar.SubstractHealth(damageAmout);

            CheckForDeath();
        }

        private void CheckForDeath()
        {
            if (health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            EventBroker.CallOnGameOver();
        }
    }
}
