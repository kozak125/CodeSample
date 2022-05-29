using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable
{
    public Action OnAttacked;

    public void GetDamaged()
    {
        Debug.Log("Ouch!");
    }
}
