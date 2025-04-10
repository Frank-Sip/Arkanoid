using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private BallSO ballSo;
    [SerializeField] private ScreenEdgesSO screenEdgesSO;

    public Vector3 Direction { get; set; }

    private BallPhysics physics = new BallPhysics();

    private void Awake()
    {
        BallManager.Register(this);
        physics.Initiate(transform, ballSo, screenEdgesSO, this);
        Direction = BallPhysics.GetInitialDirection();
    }

    public void Frame()
    {
        physics.Frame();
    }

    public void DestroyBall()
    {
        BallPool.Instance.ReturnToPool(this);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (ballSo == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, ballSo.radius);
    }
#endif
}