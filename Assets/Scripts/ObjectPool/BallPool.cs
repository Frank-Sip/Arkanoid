using UnityEngine;

public class BallPool
{
    private Transform poolContainer;
    private readonly int initialBallCount = 10;
    private readonly int expandBallCount = 10;
    private ObjectPool<BallController> pool;
    private BallController ballControllerSO;

    public BallPool(Transform container, BallController ballControllerSO)
    {
        this.poolContainer = container;
        this.ballControllerSO = ballControllerSO;
        InitializePool();
    }

    private void InitializePool()
    {
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