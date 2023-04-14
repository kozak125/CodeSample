using UnityEngine;

namespace VRShooter.Player
{
    public class Player : MonoBehaviour, IDamagable
    {
        [SerializeField]
        private FloatValue maxHealth;

        private float currentHealth;

		public void Start()
		{
            currentHealth = maxHealth.Value;
		}

		public void OnDamageReceived(float damageAmout)
        {
            currentHealth -= damageAmout;
            EventBroker.CallOnDamageReceived(damageAmout);

            CheckForDeath();
        }

        private void CheckForDeath()
        {
            if (currentHealth <= 0f)
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
