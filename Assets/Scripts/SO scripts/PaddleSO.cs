using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Paddle", menuName = "ObjectConfig/PaddleSO")]
public class PaddleSO : ScriptableObject
{
    public float speed = 10f;
    public float width = 3f;
    public float height = 0.5f;
}
