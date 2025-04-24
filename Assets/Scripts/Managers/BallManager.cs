using System.Collections.Generic;
using UnityEngine;

public class BallManager
{
    private static List<BallController> balls = new List<BallController>();
    private static List<BallController> activeBalls = new List<BallController>();
    private static bool hasRespawned = false;
    private static int maxBalls = 3; 
    private static bool isMultiballActive = false; 

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
        if (activeBalls.Contains(ball))
        {
            activeBalls.Remove(ball);
            
            if (activeBalls.Count == 0 && !hasRespawned && !isMultiballActive)
            {
                hasRespawned = true;
                Debug.Log("Sin bolas activas, respawneando bola única");
                RespawnSingleBall();
            }

            else if (activeBalls.Count == 0 && isMultiballActive)
            {
                Debug.Log("Fin del modo multiball, respawneando bola única");
                isMultiballActive = false;
                hasRespawned = true;
                RespawnSingleBall();
            }
        }
    }

    private static void RespawnSingleBall()
    {
        Vector3 paddlePos = PaddlePhysics.bounds.center;
        Vector3 ballPos = new Vector3(paddlePos.x, paddlePos.y + 3f, 0f);
        
        BallController newBall = BallPool.Instance.SpawnBall(ballPos);
        newBall.SetWaitingOnPaddle();
        
        Debug.Log("Creando nueva bola");
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

            BallController newBall = BallPool.Instance.SpawnBall(ballPos);
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
        
        Debug.Log($"Lanzando {ballsToAdd} bolas, ángulo inicial: {startAngle}, paso: {angleStep}");
        
        for (int i = 0; i < ballsToAdd; i++)
        {
            Vector3 spawnPosition = spawnBasePosition + new Vector3(Random.Range(-0.2f, 0.2f), 0.3f * i, 0f);
            
            float angle = startAngle + (angleStep * i);
            
            float angleRadians = angle * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians), 0);
            
            BallController newBall = BallPool.Instance.SpawnBall(spawnPosition);
            newBall.Direction = direction.normalized;
            newBall.IsLaunched = true;
            SetActive(newBall);
            
            Debug.Log($"Bola {i} creada con ángulo {angle}, dirección: {direction}");
        }
        
        NotifyBallLaunched();
    }

    public static void NotifyBallLaunched()
    {
        hasRespawned = false;
    }

    public static List<BallController> GetBalls() => balls;
    public static List<BallController> GetActiveBalls() => activeBalls;

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
        List<BallController> ballsCopy = new List<BallController>(balls);
        
        activeBalls.Clear();
        
        foreach (BallController ball in ballsCopy)
        {
            if (ball != null && ball.gameObject.activeInHierarchy)
            {
                ball.gameObject.SetActive(false);
                BallPool.Instance.ReturnToPool(ball);
            }
        }
        
        hasRespawned = false;
        isMultiballActive = false;
        
        RespawnSingleBall();
    }
}