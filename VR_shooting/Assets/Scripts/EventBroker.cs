using System;

namespace VRShooter
{
    public static class EventBroker
    {
        public static event Action<int> OnEnemyDestroyed;
        public static event Action OnGameOver;

        public static void CallOnEnemyDestroyed(int pointValue)
        {
            OnEnemyDestroyed?.Invoke(pointValue);
        }

        public static void CallOnGameOver()
        {
            OnGameOver?.Invoke();
        }
    }
}
