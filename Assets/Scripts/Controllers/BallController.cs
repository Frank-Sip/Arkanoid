using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private BallSO ballSo;
    [SerializeField] private ScreenEdgesSO screenEdgesSO;
    private bool followPaddle = true;

    public Vector3 Direction { get; set; }
    public bool IsLaunched { get; set; } = false;

    private BallPhysics physics = new BallPhysics();
    private Vector3 initialPosition;
    private bool isSubscribed = false;

    private void Awake()
    {
        BallManager.Register(this);
        AudioManager audioMgr = FindObjectOfType<AudioManager>();
        physics.Initiate(transform, ballSo, screenEdgesSO, this, audioMgr);
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
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        BallManager.Unregister(this);
        BallPool.Instance.ReturnToPool(this);
    }

    private void ResetBall()
    {
        BallManager.Unregister(this);
        
        transform.position = initialPosition;
        SetWaitingOnPaddle();
        
        BallPool.Instance.ReturnToPool(this);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (ballSo == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, ballSo.radius);
        
        if (gameObject.activeInHierarchy && IsLaunched)
        {
            int totalActive = BallManager.GetActiveBalls().Count;
            int maxBalls = BallManager.GetMaxBalls();
            
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, 
                                      $"Bolas: {totalActive}/{maxBalls}");
        }
    }
#endif
}