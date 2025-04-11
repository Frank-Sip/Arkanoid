using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private BallSO ballSo;
    [SerializeField] private ScreenEdgesSO screenEdgesSO;
    
    private bool followPaddle = true;
    private bool fromPowerUp = false;

    public Vector3 Direction { get; set; }
    public bool IsLaunched { get; private set; } = false;

    private BallPhysics physics = new BallPhysics();

    private void Awake()
    {
        BallManager.Register(this);
        physics.Initiate(transform, ballSo, screenEdgesSO, this);
        Direction = BallPhysics.GetInitialDirection();
    }

    public void Frame()
    {
        if (!IsLaunched && followPaddle)
        {
            Vector3 paddlePos = PaddlePhysics.bounds.center;
            transform.position = new Vector3(paddlePos.x, paddlePos.y + ballSo.radius * 3, 0f);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                IsLaunched = true;
                followPaddle = false;
                Direction = BallPhysics.GetInitialDirection();
            }
            return;
        }

        physics.Frame();
    }
    
    public void LaunchImmediately()
    {
        IsLaunched = true;
        followPaddle = false;
        fromPowerUp = true;
    }

    public bool IsMainBall()
    {
        return BallManager.GetMainBall() == this;
    }
    
    public void SetWaitingOnPaddle()
    {
        followPaddle = true;
        IsLaunched = false;
        fromPowerUp = false;
        
        Vector3 paddlePos = PaddlePhysics.bounds.center;
        transform.position = new Vector3(paddlePos.x, paddlePos.y + ballSo.radius * 3, 0f);
        
        gameObject.SetActive(true);
    }

    public void DestroyBall()
    {
        BallManager.Unregister(this);
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