using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleController : MonoBehaviour
{
    [SerializeField] private PaddleSO paddleSo;
    [SerializeField] private ScreenEdgesSO screenEdgesSO;

    private void Awake()
    {
        PaddlePhysics.Initiate(transform, paddleSo, screenEdgesSO);
    }
}
