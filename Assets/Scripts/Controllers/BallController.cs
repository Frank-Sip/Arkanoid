using UnityEngine;

[CreateAssetMenu(fileName = "BallController", menuName = "GameObject/BallControllerSO")]
public class BallController : ScriptableObject
{
    [SerializeField] private BallSO ballSo;
    [SerializeField] private ScreenEdgesSO screenEdgesSO;
    [SerializeField] private AtlasApplier atlasApplier;
    public GameObject ballPrefab;

    [HideInInspector] public Transform target;
    private bool followPaddle = true;
    private GameObject ballInstance;

    public Vector3 Direction { get; set; }
    public bool IsLaunched { get; set; } = false;

    private BallPhysics physics;
    private Vector3 initialPosition;
    private bool isSubscribed = false;

    public BallController Clone()
    {
        var clone = Instantiate(this);
        clone.physics = new BallPhysics();
        clone.followPaddle = true;
        clone.IsLaunched = false;
        clone.ballInstance = null;
        clone.target = null;
        clone.isSubscribed = false;
        clone.atlasApplier = this.atlasApplier;
        clone.ballPrefab = this.ballPrefab;
        return clone;
    }

    public void Init(Transform parent = null)
    {
        if (target == null)
        {
            ballInstance = Instantiate(ballPrefab);
            if (parent != null)
            {
                ballInstance.transform.SetParent(parent);
            }
            target = ballInstance.transform;
            InitializePhysics();

            if (!isSubscribed)
            {
                EventManager.OnReset += ResetBall;
                isSubscribed = true;
            }
        }
        
        ApplyAtlas();
    }

    public void Frame()
    {
        if (!IsLaunched && followPaddle)
        {
            Vector3 paddlePos = PaddlePhysics.bounds.center;
            target.position = new Vector3(paddlePos.x, paddlePos.y + ballSo.radius * 3, 0f);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                IsLaunched = true;
                Direction = BallPhysics.GetInitialDirection();
                BallManager.SetActive(this);
                BallManager.NotifyBallLaunched();
            }

            return;
        }

        physics?.Frame();
    }
    
    public void InitializePhysics()
    {
        physics = new BallPhysics();
        AudioManager audioMgr = ServiceProvider.GetService<AudioManager>();
        physics.Initiate(target, ballSo, screenEdgesSO, this, audioMgr);
        Direction = BallPhysics.GetInitialDirection();
    }

    public void SetWaitingOnPaddle()
    {
        followPaddle = true;
        IsLaunched = false;
    
        if (target != null)
        {
            Vector3 paddlePos = PaddlePhysics.bounds.center;
            target.position = new Vector3(paddlePos.x, paddlePos.y + ballSo.radius * 3, 0f);
            ApplyAtlas();
        }
    }

    public void DestroyBall()
    {
        if (target != null)
        {
            var rigidbody = target.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }
            
            ResetState();
            
            target.localScale = Vector3.one;
            BallManager.Unregister(this);
            ServiceProvider.GetService<BallPool>().ReturnToPool(this);
            
            target = null;
        }
    }
    
    private void ApplyAtlas()
    {
        if (target != null && atlasApplier != null)
        {
            Transform visual = target.GetChild(0);
            atlasApplier.ApplyAtlas(visual.gameObject);
        }
    }
    
    public void ResetBall()
    {
        DestroyBall();
    }

    private void OnDestroy()
    {
        if (isSubscribed)
        {
            EventManager.OnReset -= ResetBall;
        }
    }
    
    public void ResetState()
    {
        followPaddle = true;
        IsLaunched = false;
        Direction = Vector3.zero;
    
        if (target != null)
        {
            target.localScale = Vector3.one;
            ApplyAtlas();
        }
    }
}