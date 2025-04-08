using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private BallSO ballSo;
    [SerializeField] private ScreenEdgesSO screenEdgesSO;

    private void Awake()
    {
        BallPhysics.Initiate(transform, ballSo, screenEdgesSO);
    }
}
