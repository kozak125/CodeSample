using System;
using UnityEngine;

public class EventBroker
{
    public static Action<int> OnEnemyDestroyed;
    public static Action OnGameOver;

    public static void CallOnEnemyDestroyed(int pointValue)
    {
        OnEnemyDestroyed?.Invoke(pointValue);
    }

    public static void CallOnGameOver()
    {
        OnGameOver?.Invoke();
    }
}
