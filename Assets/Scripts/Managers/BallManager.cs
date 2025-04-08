using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    private static List<BallController> balls = new List<BallController>();

    public static void Register(BallController ball)
    {
        if (!balls.Contains(ball))
        {
            balls.Add(ball);
        }
    }

    public static void Unregister(BallController ball)
    {
        if (balls.Contains(ball))
        {
            balls.Remove(ball);
        }

        if (balls.Count <= 0)
        {
            Debug.Log("You lose!");
        }
    }
    
    public static List<BallController> GetBalls()
    {
        return balls;
    }
}
