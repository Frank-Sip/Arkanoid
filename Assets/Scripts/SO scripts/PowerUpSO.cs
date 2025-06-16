using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp", menuName = "ObjectConfig/PowerUp")]
public class PowerUpSO : ScriptableObject
{
    public float speed = 10f;
    public float radius = 0.3f;
    public PowerUpType powerUpType = PowerUpType.Multiball;
    public AtlasApplier atlas;
}