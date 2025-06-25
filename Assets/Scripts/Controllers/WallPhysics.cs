using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallPhysics
{
    public static void Initiate(Transform wallTransform, Transform visual, WallSO config)
    {
        Mesh mesh = visual.GetComponent<MeshFilter>()?.sharedMesh;
        if (mesh != null)
        {
            Bounds meshBounds = mesh.bounds;

            float baseWidth = meshBounds.size.x;
            float baseHeight = meshBounds.size.y;
            float baseLength = meshBounds.size.z;

            float scaleX = config.width / baseWidth;
            float scaleY = config.height / baseHeight;
            float scaleZ = config.length / baseLength;

            visual.localScale = new Vector3(scaleX, scaleY, scaleZ);
            Vector3 centerOffset = Vector3.Scale(meshBounds.center, visual.localScale);
            visual.localPosition = -centerOffset;
            visual.localRotation = Quaternion.identity;
        }
        else
        {
            visual.localScale = new Vector3(config.width, config.height, config.length);
            visual.localPosition = Vector3.zero;
        }
    }

#if UNITY_EDITOR
    public static void DrawVisualGizmo(Transform visual)
    {
        Mesh mesh = visual.GetComponent<MeshFilter>()?.sharedMesh;
        if (mesh == null) return;

        Gizmos.color = Color.gray;

        Matrix4x4 localToWorld = visual.localToWorldMatrix;
        Gizmos.matrix = localToWorld;
        Gizmos.DrawWireCube(mesh.bounds.center, mesh.bounds.size);
        Gizmos.matrix = Matrix4x4.identity;
    }
#endif
}