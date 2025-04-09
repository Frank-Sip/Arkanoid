using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private BallSO ballSo;
    [SerializeField] private ScreenEdgesSO screenEdgesSO;
    
    public Vector3 Direction { get; set; }
    
    private void Awake()
    {
        BallManager.Register(this);
        BallPhysics.Initiate(transform, ballSo, screenEdgesSO, this);
        Direction = BallPhysics.GetInitialDirection();
    }

    public void Frame()
    {
        BallPhysics.Frame(transform, ballSo, screenEdgesSO, this);
    }

    public void DestroyBall()
    {
        BallPool.Instance.ReturnToPool(this);
    }
}
