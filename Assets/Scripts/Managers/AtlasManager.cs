using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtlasManager
{
    private static readonly Dictionary<AtlasSO, Material> SharedMaterials = new();

    public static Material GetSharedAtlasMaterial(AtlasSO atlasSO)
    {
        if (!SharedMaterials.TryGetValue(atlasSO, out Material material))
        {
            material = atlasSO.sharedMaterial;
            SharedMaterials[atlasSO] = material;
        }
        return material;
    }
}
