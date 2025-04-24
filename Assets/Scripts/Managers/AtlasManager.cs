using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtlasManager : MonoBehaviour
{
    private static Material _sharedAtlasMaterial;
    private static AtlasSO _atlasSO;

    public static Material GetSharedAtlasMaterial(AtlasSO atlasSO)
    {
        if (_atlasSO != atlasSO || _sharedAtlasMaterial == null)
        {
            _atlasSO = atlasSO;
            _sharedAtlasMaterial = atlasSO.sharedMaterial;
        }
        
        return _sharedAtlasMaterial;
    }
}
