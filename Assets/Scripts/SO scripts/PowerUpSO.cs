using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp", menuName = "ObjectConfig/PowerUp")]
public class PowerUpSO : ScriptableObject
{
    [Header("Basic Settings")]
    public float speed = 10f;
    public float radius = 0.3f;
    public PowerUpType powerUpType = PowerUpType.Multiball;
    public AtlasApplier atlas;
    
    [Header("Multiball Settings")]
    [Tooltip("Number of extra balls to spawn")]
    public int ballCount = 2;

    [Header("Wide Paddle Settings")]
    public float widthMultiplier = 1.5f;
    public float widePaddleDuration = 5f;

    [Header("Extra Life Settings")]
    public int lifeCount = 1;

    [Header("Speed Boost Settings")]
    public float speedMultiplier = 1.5f;
    public float speedBoostDuration = 5f;
}