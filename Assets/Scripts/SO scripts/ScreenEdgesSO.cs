using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Edges", menuName = "ObjectConfig/ScreenEdgesSO")]
public class ScreenEdgesSO : ScriptableObject
{
    public float left = -8f;
    public float right = 8f;
    public float up = 5f;
    public float down = -13f;
}
