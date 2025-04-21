using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private BallSO ballSo;
    [SerializeField] private ScreenEdgesSO screenEdgesSO;
    private bool followPaddle = true;

    public Vector3 Direction { get; set; }
    public bool IsLaunched { get; private set; } = false;

    private BallPhysics physics = new BallPhysics();
    private Vector3 initialPosition;
    private bool isSubscribed = false;

    private void Awake()
    {
        BallManager.Register(this);
        physics.Initiate(transform, ballSo, screenEdgesSO, this);
        Direction = BallPhysics.GetInitialDirection();

        initialPosition = transform.position;

        if (!isSubscribed)
        {
            EventManager.OnReset += ResetBall;
            isSubscribed = true;
        }
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
                Direction = BallPhysics.GetInitialDirection();
                BallManager.SetActive(this);
                BallManager.NotifyBallLaunched();
            }

            return;
        }

        physics.Frame();
    }

    public void SetWaitingOnPaddle()
    {
        followPaddle = true;
        IsLaunched = false;
    }

    public void DestroyBall()
    {
        BallManager.Unregister(this);
        BallPool.Instance.ReturnToPool(this);
    }

    private void ResetBall()
    {
        transform.position = initialPosition;
        SetWaitingOnPaddle();
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