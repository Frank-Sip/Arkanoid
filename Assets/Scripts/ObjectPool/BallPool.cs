using UnityEngine;

public class BallPool : MonoBehaviour
{
    [SerializeField] private int initialBallCount = 10;
    [SerializeField] private Transform poolContainer;
    private int expandBallCount = 5;

    private ObjectPool<BallController> pool;
    public static BallPool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        InitializePool();
        BallManager.RespawnSingleBall();
    }

    private void InitializePool()
    {
        BallController ballControllerSO = GameManager.Instance.ballControllerSO;
        
        if (pool == null)
        {
            pool = new ObjectPool<BallController>(
                ballControllerSO.ballPrefab,
                ballControllerSO,
                poolContainer,
                initialBallCount,
                expandBallCount,
                createFunc: () => ballControllerSO.Clone()
            );
        }
    }
    
    public void ClearPool()
    {
        if (pool != null)
        {
            var balls = BallManager.GetBalls();
            foreach (var ball in balls)
            {
                if (ball != null && ball.target != null)
                {
                    ball.target.gameObject.SetActive(false);
                    ReturnToPool(ball);
                }
            }
        }
    }

    public BallController SpawnBall(Vector3 position)
    {
        if (pool == null)
        {
            InitializePool();
        }

        var (controller, instance) = pool.Get();
        
        instance.transform.localScale = Vector3.one;
        instance.transform.position = position;
        instance.transform.SetParent(poolContainer);
        
        controller.ResetState();
        controller.target = instance.transform;
        controller.InitializePhysics();

        BallManager.Register(controller);

        return controller;
    }

    public void ReturnToPool(BallController controller)
    {
        if (controller == null) return;

        var instance = controller.target?.gameObject;
        if (instance != null)
        {
            instance.SetActive(false);
            
            controller.ResetState();
            instance.transform.localScale = Vector3.one;
            instance.transform.SetParent(poolContainer);
            
            var rigidbody = instance.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }
            
            controller.target = null;
        
            pool.Return(controller, instance);
        }
    }
}