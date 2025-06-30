using UnityEngine;

[CreateAssetMenu(fileName = "ParallaxConfig", menuName = "ObjectConfig/Parallax Config")]
public class ParallaxConfigSO : ScriptableObject
{
    public float backgroundSpeed = 0.05f;
    public float middleSpeed = 0.15f;
    public float foregroundSpeed = 0.3f;
}