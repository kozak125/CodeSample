using UnityEngine;

namespace VRShooter.Player
{
    public class Player : MonoBehaviour, IDamagable
    {
        [SerializeField]
        private FloatValue maxHealth;

        private float currentHealth;

		public void OnDamageReceived(float damageAmout)
        {
            currentHealth -= damageAmout;
            EventBroker.CallOnDamageReceived(damageAmout);

            CheckForDeath();
        }

		private void Start()
		{
            currentHealth = maxHealth.Value;
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
