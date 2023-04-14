using System;

public static class EventBroker
{
    public static event Action<int> OnEnemyDestroyed;
    public static event Action OnGameOver;
    public static event Action<int> OnDamageTaken;

    public static void CallOnEnemyDestroyed(int pointValue)
    {
        OnEnemyDestroyed?.Invoke(pointValue);
    }

    public static void CallOnGameOver()
    {
        OnGameOver?.Invoke();
    }

    public static void CallOnDamageTaken(int damageAmount)
	{
        OnDamageTaken?.Invoke(damageAmount);
	}
}
