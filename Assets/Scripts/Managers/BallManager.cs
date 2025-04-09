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

    public static void Unregister(BallController ball)
    {
        activeBalls.Remove(ball);

        if (activeBalls.Count <= 0)
        {
            Debug.Log("You lose!");
        }
    }
    
    public static List<BallController> GetBalls()
    {
        return balls;
    }
    
    public static List<BallController> GetActiveBalls()
    {
        return activeBalls;
    }
}
