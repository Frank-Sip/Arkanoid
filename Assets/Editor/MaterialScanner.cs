using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class MaterialScanner : EditorWindow
{
    [MenuItem("Tools/Scan Materials In Scene")]
    static void Init()
    {
        MaterialScanner window = (MaterialScanner)EditorWindow.GetWindow(typeof(MaterialScanner));
        window.titleContent = new GUIContent("Material Scanner");
        window.Show();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Escanear materiales en escena"))
        {
            ScanMaterials();
        }
    }

    void ScanMaterials()
    {
        HashSet<Material> uniqueMaterials = new HashSet<Material>();
        int renderersCount = 0;

        Renderer[] renderers = FindObjectsOfType<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            renderersCount++;

            foreach (Material mat in renderer.sharedMaterials)
            {
                if (mat != null)
                {
                    uniqueMaterials.Add(mat);
                }
            }
        }

        Debug.Log($"Scanned renderers: {renderersCount}");
        Debug.Log($"Unique materials found (sharedMaterials): {uniqueMaterials.Count}");

        foreach (Material mat in uniqueMaterials)
        {
            Debug.Log($"Material: {mat.name}", mat);
        }
    }
}