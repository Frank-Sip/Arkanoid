using UnityEngine;

[CreateAssetMenu(fileName = "AtlasSO", menuName = "Atlas/AtlasSO")]
public class AtlasSO : ScriptableObject
{
    public AtlasID[] entries;
    public Material sharedMaterial;
    
    public Rect GetUVRect(AtlasType type)
    {
        foreach (var entry in entries)
        {
            if (entry.type == type)
            {
                return entry.uvRect;
            }
        }
        return new Rect(0, 0, 1, 1);
    }
}