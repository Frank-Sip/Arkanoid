using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class InstancedMaterialsScanner : EditorWindow
{
    [MenuItem("Tools/Scan Instanced Materials")]
    static void Init()
    {
        InstancedMaterialsScanner window = (InstancedMaterialsScanner)EditorWindow.GetWindow(typeof(InstancedMaterialsScanner));
        window.titleContent = new GUIContent("Instanced Materials Scanner");
        window.Show();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Escanear materiales instanciados"))
        {
            ScanInstancedMaterials();
        }
    }

    void ScanInstancedMaterials()
    {
        HashSet<Material> instancedMaterials = new HashSet<Material>();
        int materialCount = 0;
        
        Renderer[] renderers = FindObjectsOfType<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.materials)
            {
                if (mat != null && !instancedMaterials.Contains(mat))
                {
                    instancedMaterials.Add(mat);
                    materialCount++;
                }
            }
        }

        Debug.Log($"Scanned renderers: {renderers.Length}");
        Debug.Log($"Instanced materials found: {materialCount}");

        foreach (Material mat in instancedMaterials)
        {
            Debug.Log($"Instanced material: {mat.name}", mat);
        }
    }
}