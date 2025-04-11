using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BallManager
{
    private static readonly List<BallController> balls = new List<BallController>();
    
    private static BallController mainBall;

    public static void Register(BallController ball)
    {
        if (!balls.Contains(ball))
        {
            balls.Add(ball);
        }
        
        if (mainBall == null)
        {
            mainBall = ball;
        }
    }

    public static void RegisterAdditional(BallController ball)
    {
        if (!balls.Contains(ball))
        {
            balls.Add(ball);
        }
    }

    public static void Unregister(BallController ball)
    {
        balls.Remove(ball);

        if (balls.Count == 0)
        {
            mainBall = null;
            RespawnMainBall();
            return;
        }
        
        if (ball == mainBall)
        {
            mainBall = balls[0];
            
            if (balls.Count == 1)
            {
                mainBall.SetWaitingOnPaddle();
            }
        }
        else if (balls.Count == 1 && balls[0] != mainBall)
        {
            mainBall = balls[0];
            mainBall.SetWaitingOnPaddle();
        }
    }

    private static void RespawnMainBall()
    {
        if (mainBall != null && mainBall.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("Intento de respawn de bola principal cuando ya existe una.");
            return;
        }
        
        if (balls.Count > 0)
        {
            balls.Clear();
        }
        
        Vector3 paddlePos = PaddlePhysics.bounds.center;
        Vector3 ballPos = new Vector3(paddlePos.x, paddlePos.y + 3f, 0f);

        BallController newBall = BallPool.Instance.SpawnBall(ballPos);
        if (newBall != null)
        {
            newBall.gameObject.SetActive(true);
            
            mainBall = newBall;
            balls.Add(newBall);
            
            newBall.SetWaitingOnPaddle();
        }
    }

    public static List<BallController> GetBalls() => balls;
    
    public static BallController GetMainBall() => mainBall;
    
    public static int GetBallCount() => balls.Count;
    
    public static void ClearBalls()
    {
        balls.Clear();
        mainBall = null;
    }

}
