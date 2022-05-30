using System;
using UnityEngine;

public class EventBroker
{
    public static Action<int> OnEnemyDestroyed;

    public static void CallOnEnemyDestroyed(int pointValue)
    {
        OnEnemyDestroyed?.Invoke(pointValue);
    }

    public static Action OnGameOver;

    public static void callOnGameOver()
    {
        OnGameOver?.Invoke();
    }
}
