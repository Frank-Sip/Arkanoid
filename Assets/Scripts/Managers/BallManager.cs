using System.Collections.Generic;
using UnityEngine;

public class BallManager
{
    private static readonly List<BallController> balls = new List<BallController>();
    private static readonly List<BallController> activeBalls = new List<BallController>();
    private static bool hasRespawned = false;
    private static int maxBalls = 3;
    private static bool isMultiballActive = false;

    public static void Register(BallController ball)
    {
        if (ball == null) return;
        
        if (!balls.Contains(ball))
        {
            balls.Add(ball);
        }
    }

    public static void SetActive(BallController ball)
    {
        if (ball == null) return;
        
        if (!activeBalls.Contains(ball))
        {
            activeBalls.Add(ball);
        }
    }

    public static void Unregister(BallController ball)
    {
        if (ball == null) return;

        balls.Remove(ball);
        activeBalls.Remove(ball);
        Debug.Log($"Ball unregistered. Remaining balls: {balls.Count}, Active balls: {activeBalls.Count}");
        
        if (activeBalls.Count == 0 && !hasRespawned)
        {
            hasRespawned = true;
            Debug.Log("Sin bolas activas, respawneando bola única");
            RespawnSingleBall();
        }
    }

    public static void RespawnSingleBall()
    {
        CleanupInactiveBalls();
        
        foreach (var ball in balls)
        {
            if (ball != null && !ball.IsLaunched)
            {
                Debug.Log("Ya existe una bola esperando en la paddle");
                return;
            }
        }

        Vector3 paddlePos = PaddlePhysics.bounds.center;
        Vector3 ballPos = new Vector3(paddlePos.x, paddlePos.y + 3f, 0f);

        BallController newBall = ServiceProvider.GetService<BallPool>().SpawnBall(ballPos);
        if (newBall != null)
        {
            newBall.SetWaitingOnPaddle();
            Register(newBall);
        }
    }
    
    private static void CleanupInactiveBalls()
    {
        List<BallController> ballsToRemove = new List<BallController>();
        foreach (var ball in balls)
        {
            if (ball == null || ball.target == null || !ball.target.gameObject.activeInHierarchy)
            {
                ballsToRemove.Add(ball);
            }
        }

        foreach (var ball in ballsToRemove)
        {
            balls.Remove(ball);
            activeBalls.Remove(ball);
            ServiceProvider.GetService<BallPool>().ReturnToPool(ball);
        }
    }
    
    public static void SpawnMultipleBalls()
    {
        int numberOfBalls = 2;
        int ballsToAdd = Mathf.Min(numberOfBalls, maxBalls - activeBalls.Count);

        if (ballsToAdd <= 0)
            return;

        for (int i = 0; i < ballsToAdd; i++)
        {
            Vector3 paddlePos = PaddlePhysics.bounds.center;
            Vector3 ballPos = new Vector3(paddlePos.x + Random.Range(-1f, 1f), paddlePos.y + 3f, 0f);

            BallController newBall = ServiceProvider.GetService<BallPool>().SpawnBall(ballPos);
            newBall.SetWaitingOnPaddle();
        }
    }

    public static void SpawnAndLaunchMultipleBalls(int numberOfBalls = 2)
    {
        isMultiballActive = true;

        int ballsToAdd = Mathf.Min(numberOfBalls, maxBalls - activeBalls.Count);

        if (ballsToAdd <= 0)
            return;

        Vector3 paddlePos = PaddlePhysics.bounds.center;
        Vector3 spawnBasePosition = new Vector3(paddlePos.x, paddlePos.y + 1f, 0f);

        float angleStep = 40f / (ballsToAdd > 1 ? ballsToAdd - 1 : 1);
        float startAngle = 70f;

        for (int i = 0; i < ballsToAdd; i++)
        {
            Vector3 spawnPosition = spawnBasePosition + new Vector3(Random.Range(-0.2f, 0.2f), 0.3f * i, 0f);

            float angle = startAngle + (angleStep * i);
            float angleRadians = angle * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians), 0);

            BallController newBall = ServiceProvider.GetService<BallPool>().SpawnBall(spawnPosition);
            newBall.Init();
            newBall.Direction = direction.normalized;
            newBall.IsLaunched = true;
            SetActive(newBall);
        }

        NotifyBallLaunched();
    }

    public static void NotifyBallLaunched()
    {
        hasRespawned = false;
    }

    public static IReadOnlyList<BallController> GetBalls()
    {
        return balls.AsReadOnly();
    }

    public static IReadOnlyList<BallController> GetActiveBalls()
    {
        return activeBalls.AsReadOnly();
    }

    public static int GetMaxBalls()
    {
        return maxBalls;
    }

    public static void SetMaxBalls(int max)
    {
        maxBalls = Mathf.Max(1, max);
    }

    public static void ResetAll()
    {
        Physics.autoSimulation = false;
        
        foreach (var ball in balls)
        {
            if (ball != null && ball.target != null)
            {
                ball.target.gameObject.SetActive(false);
            }
        }
        
        balls.Clear();
        activeBalls.Clear();
        
        ServiceProvider.GetService<BallPool>().ClearPool();
        
        hasRespawned = false;
        isMultiballActive = false;
        
        Physics.autoSimulation = true;
        
        RespawnSingleBall();
    }
}