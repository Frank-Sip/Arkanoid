using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType
{
    Multiball,
    ExtraLife,
    WidePaddle
}

[CreateAssetMenu(fileName = "PowerUp", menuName = "ObjectConfig/PowerUp")]
public class PowerUpSO : ScriptableObject
{
    public float speed = 5f;
    public float radius = 0.5f;
    public PowerUpType powerUpType = PowerUpType.Multiball;
}