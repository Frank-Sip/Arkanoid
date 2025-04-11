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
            
            Debug.Log("La bola principal se ha cambiado a una de las bolas existentes");
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
        
        if (PaddlePhysics.bounds.width <= 0 || PaddlePhysics.bounds.height <= 0)
        {
            Debug.LogError("La paleta no está inicializada correctamente. No se puede posicionar la bola.");
            return;
        }
        
        Vector3 paddlePos = PaddlePhysics.bounds.center;
        Vector3 ballPos = new Vector3(paddlePos.x, paddlePos.y + 3f, 0f);

        BallController newBall = BallPool.Instance.SpawnMainBall(ballPos);
        if (newBall != null)
        {
            newBall.gameObject.SetActive(true);
            
            mainBall = newBall;
            balls.Add(newBall);
            
            newBall.SetWaitingOnPaddle();
            
            Debug.Log("Pelota principal respawneada en la posición: " + ballPos);
        }
        else
        {
            Debug.LogError("No se pudo crear una nueva bola desde el pool.");
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
