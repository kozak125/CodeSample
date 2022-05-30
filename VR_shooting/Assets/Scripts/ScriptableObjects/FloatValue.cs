using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Float Value")]
public class FloatValue : ScriptableObject
{
    [SerializeField]
    private float value;

    public float Value => value;
}
