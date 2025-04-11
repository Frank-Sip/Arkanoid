using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPool : MonoBehaviour
{
    [SerializeField] private BallController ballPrefab;
    [SerializeField] private int initialBallCount = 0;
    [SerializeField] private Transform poolContainer;
    private int expandBallCount = 10;
    private ObjectPool<BallController> pool;
    public static BallPool Instance { get; private set; }
    
    private void Awake()
    {
        Instance = this;
        pool = new ObjectPool<BallController>(ballPrefab, poolContainer, initialBallCount, expandBallCount);
    }
    
    public BallController SpawnBall(Vector3 position)
    {
        var ball = pool.Get();
        ball.transform.position = position;
        BallManager.Register(ball);
        return ball;
    }

    public void ReturnToPool(BallController ball)
    {
        BallManager.Unregister(ball);
        pool.Return(ball);
    }
}
