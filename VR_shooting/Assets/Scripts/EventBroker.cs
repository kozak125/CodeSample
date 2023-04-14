using System;

namespace VRShooter
{
    public static class EventBroker
    {
        public static event Action<int> OnEnemyDestroyed;
        public static event Action OnGameOver;
        public static event Action<float> OnDamageReceived;

        public static void CallOnEnemyDestroyed(int pointValue)
        {
            OnEnemyDestroyed?.Invoke(pointValue);
        }

        public static void CallOnGameOver()
        {
            OnGameOver?.Invoke();
        }

        public static void CallOnDamageReceived(float damageAmount)
		{
            OnDamageReceived?.Invoke(damageAmount);
		}
    }
}
