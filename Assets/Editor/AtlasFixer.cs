#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AtlasFixer : MonoBehaviour
{
    [MenuItem("Tools/Fijar materiales de Atlas")]
    public static void FixAllAtlasAppliers()
    {
        AtlasApplier[] allAppliers = FindObjectsOfType<AtlasApplier>(true);
        
        Dictionary<AtlasSO, Material> atlasMaterials = new Dictionary<AtlasSO, Material>();
        
        foreach (var applier in allAppliers)
        {
            if (applier.atlasMaster != null && !atlasMaterials.ContainsKey(applier.atlasMaster))
            {
                atlasMaterials[applier.atlasMaster] = applier.atlasMaster.sharedMaterial;
            }
        }
        
        foreach (var applier in allAppliers)
        {
            if (applier.atlasMaster != null && applier.GetComponent<Renderer>() != null)
            {
                applier.GetComponent<Renderer>().sharedMaterial = atlasMaterials[applier.atlasMaster];
                EditorUtility.SetDirty(applier.gameObject);
            }
        }

        Debug.Log($"Atlas materials updated. Found {allAppliers.Length} Atlas objects using {atlasMaterials.Count} unique materials.");
    }
}
#endif