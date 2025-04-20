using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BallManager
{
    private static List<BallController> balls = new List<BallController>();
    private static List<BallController> activeBalls = new List<BallController>();

    public static void Register(BallController ball)
    {
        if (!balls.Contains(ball))
        {
            balls.Add(ball);
        }
    }

    public static void SetActive(BallController ball)
    {
        if (!activeBalls.Contains(ball))
        {
            activeBalls.Add(ball);
        }
    }

    public static void Unregister(BallController ball)
    {
        activeBalls.Remove(ball);

        if (activeBalls.Count <= 0)
        {
            RespawnSingleBall();
        }
    }

    private static void RespawnSingleBall()
    {
        Vector3 paddlePos = PaddlePhysics.bounds.center;
        Vector3 ballPos = new Vector3(paddlePos.x, paddlePos.y + 3f, 0f);

        BallController newBall = BallPool.Instance.SpawnBall(ballPos);
        newBall.SetWaitingOnPaddle();
    }
    
    public static void SpawnMultipleBalls()
    {
        int numberOfBalls = 2;
        
        for (int i = 0; i < numberOfBalls; i++)
        {
            Vector3 paddlePos = PaddlePhysics.bounds.center;
            Vector3 ballPos = new Vector3(paddlePos.x + Random.Range(-1f, 1f), paddlePos.y + 3f, 0f);

            BallController newBall = BallPool.Instance.SpawnBall(ballPos);
            newBall.SetWaitingOnPaddle();
        }
    }

    public static List<BallController> GetBalls() => balls;
    public static List<BallController> GetActiveBalls() => activeBalls;
}
