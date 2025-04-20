using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp", menuName = "ObjectConfig/PowerUp")]
public class PowerUpSO : ScriptableObject
{
    public float speed = 2f;
    public float radius = 0.5f;
}