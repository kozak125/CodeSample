using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public void ChangeColorRed()
    {
        GetComponent<Renderer>().material.color = Color.red;
    }

    public void ChangeColorBlue()
    {
        GetComponent<Renderer>().material.color = Color.blue;
    }
}
