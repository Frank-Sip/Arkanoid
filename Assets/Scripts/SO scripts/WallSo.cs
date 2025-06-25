using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wall", menuName = "ObjectConfig/WallSO")]
public class WallSO : ScriptableObject
{
    public float width = 1f;
    public float height = 5f;
    public float length = 1f;
}