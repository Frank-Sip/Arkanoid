using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp", menuName = "ObjectConfig/PowerUpSO")]
public class PowerUpSO : ScriptableObject
{
    public float width = 1f;
    public float height = 1f;
    public float fallSpeed = 5f;
    public PowerUpType type;
}

public enum PowerUpType
{
    Multiball
} 