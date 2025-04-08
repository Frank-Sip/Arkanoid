using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ball", menuName = "ObjectConfig/BallSO")]
public class BallSO : ScriptableObject
{
    public float speed = 15f;
    public float radius = 0.25f;
}
