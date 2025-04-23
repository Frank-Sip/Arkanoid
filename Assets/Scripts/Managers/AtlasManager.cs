using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtlasManager : MonoBehaviour
{
    private static Material _sharedAtlasMaterial;

    public static Material GetSharedAtlasMaterial(AtlasSO atlasSO)
    {
        return atlasSO.sharedMaterial;
    }
}
