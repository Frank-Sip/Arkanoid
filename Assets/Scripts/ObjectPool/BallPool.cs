using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPool : MonoBehaviour
{
    [SerializeField] private BallController ballPrefab;
    [SerializeField] private int initialBallCount = 10;
    [SerializeField] private Transform poolContainer;
    private int expandBallCount = 10;
    private ObjectPool<BallController> pool;
    public static BallPool Instance { get; private set; }
    
    private BallController initialBall;
    
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
        
        // Si es la primera bola que se crea, considerarla la bola inicial
        if (initialBall == null)
        {
            initialBall = ball;
        }
        
        return ball;
    }

    public void ReturnToPool(BallController ball)
    {
        if (ball == initialBall)
        {
            ball.gameObject.SetActive(false);
            return;
        }
        
        ball.gameObject.SetActive(false);
        pool.Return(ball);
    }
    
    public BallController GetInitialBall()
    {
        return initialBall;
    }
}
