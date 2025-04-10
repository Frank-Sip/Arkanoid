using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AtlasManager
{
    private static int columns = 8;
    private static int rows = 7;
    private static MaterialPropertyBlock block = new MaterialPropertyBlock();

    private static Dictionary<string, List<Vector2Int>> atlasCoords = new Dictionary<string, List<Vector2Int>>()
    {
        { "paddle", new List<Vector2Int> { new(5, 5) } },
        { "ball", new List<Vector2Int> { new(1, 6) } },
        { "brick", new List<Vector2Int>
            {
                new(2, 2), new(2, 3), new(2, 4), new(2, 5), new(2, 6), new(2, 7), new(1, 4), new(1, 5), new(4, 4)
            }
        },
    };
    
    public static void ActivateAtlas(Renderer renderer, string type)
    {
        if (!atlasCoords.TryGetValue(type, out List<Vector2Int> coords))
        {
            Debug.LogWarning($"Atlas type '{type}' not found");
            return;
        }
        
        Material newMat = Object.Instantiate(renderer.sharedMaterial);
        renderer.material = newMat;
        
        Vector2Int chosen = coords[Random.Range(0, coords.Count)];
        ApplyCoords(renderer, chosen);
    }
    
    private static void ApplyCoords(Renderer renderer, Vector2Int coord)
    {
        Vector2 scale = new Vector2(1f / columns, 1f / rows);
        Vector2 offset = new Vector2(scale.x * coord.x, scale.y * coord.y);

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(block);
        block.SetVector("_MainTex_ST", new Vector4(scale.x, scale.y, offset.x, offset.y));
        renderer.SetPropertyBlock(block);
    }
}
