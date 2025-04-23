#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class AtlasFixer : MonoBehaviour
{
    [MenuItem("Tools/Fijar materiales de Atlas")]
    public static void FixAllAtlasAppliers()
    {
        AtlasApplier[] allAppliers = FindObjectsOfType<AtlasApplier>(true);
        foreach (var applier in allAppliers)
        {
            if (applier.atlasMaster != null && applier.GetComponent<Renderer>() != null)
            {
                applier.GetComponent<Renderer>().sharedMaterial = applier.atlasMaster.sharedMaterial;
                EditorUtility.SetDirty(applier.gameObject);
            }
        }

        Debug.Log("Atlas materials updated.");
    }
}
#endif