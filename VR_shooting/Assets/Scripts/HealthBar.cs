using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Slider damageBar;
    
    private Slider healthBar;
    private WaitForSeconds waitSecondsToSubstractDamage = new WaitForSeconds(1f);
    private WaitForSeconds damageSubstractionSpeed = new WaitForSeconds(0.1f);
    private int damageSubstractionSmoothing = 1;
    private bool isCoroutineRunning = false;
    
    public void Setup(int maxHealth, ref Action<int> onDamageTaken)
    {
        healthBar = GetComponent<Slider>();
        healthBar.value = healthBar.maxValue = maxHealth;
        damageBar.value = damageBar.maxValue = maxHealth;
        
        onDamageTaken += SubstractHealth;
        EventBroker.OnGameOver += GameOver;
    }

    private void SubstractHealth(int amount)
    {
        healthBar.value -= amount;
        if (!isCoroutineRunning)
        {
            StartCoroutine(nameof(SmoothSubstractDamage));
        }
    }

    private IEnumerator SmoothSubstractDamage()
    {
        isCoroutineRunning = true;
        yield return waitSecondsToSubstractDamage;
        while (damageBar.value > healthBar.value)
        {
            damageBar.value -= damageSubstractionSmoothing;
            yield return damageSubstractionSpeed;
        }

        isCoroutineRunning = false;
    }

    private void GameOver()
    {
        gameObject.SetActive(false);
        damageBar.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventBroker.OnGameOver -= GameOver;
    }
}
