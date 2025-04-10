using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleController : MonoBehaviour
{
    [SerializeField] private PaddleSO paddleSO;
    [SerializeField] private ScreenEdgesSO screenEdgesSO;
    [SerializeField] private Transform visual;

    public void Initiate()
    {
        PaddlePhysics.Initiate(transform, visual, paddleSO, screenEdgesSO);
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(PaddlePhysics.bounds.center, PaddlePhysics.bounds.size);
        
        if (visual != null)
        {
            Gizmos.color = Color.red;
            Bounds meshBounds = visual.GetComponent<MeshFilter>()?.sharedMesh?.bounds ?? new Bounds(Vector3.zero, Vector3.one);
            Vector3 size = Vector3.Scale(meshBounds.size, visual.lossyScale);
            Vector3 center = visual.position + Vector3.Scale(meshBounds.center, visual.lossyScale);
            Gizmos.DrawWireCube(center, size);
        }
    }
#endif
}