using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider healthBar;
    
    public void Setup(int maxHealth, ref Action<int> onDamageTaken)
    {
        healthBar = GetComponent<Slider>();
        healthBar.value = healthBar.maxValue = maxHealth;
        
        onDamageTaken += SubstractHealth;
    }

    private void SubstractHealth(int amount)
    {
        healthBar.value -= amount;
    }
}
