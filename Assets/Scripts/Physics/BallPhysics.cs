using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BallPhysics
{
    private static Transform ball;
    private static Vector3 direction = new Vector3(1, 1, 0);
    private static BallSO ballConfig;
    private static ScreenEdgesSO screenConfig;

    public static void Inititate(Transform t, BallSO ballSO, ScreenEdgesSO screenSO)
    {
        ResetBallDirection();
        
        ball = t;
        ballConfig = ballSO;
        screenConfig = screenSO;

        ball.localScale = Vector3.one * (ballConfig.radius * 2f);
    }

    public static void ResetBallDirection()
    {
        float x = Random.value < 0.5f ? -1f : 1f;
        direction = new Vector3(x, -1f, 0);
    }

    public static void Frame()
    {
        if (ball == null) return;

        Vector3 position = ball.position;
        position += direction.normalized * ballConfig.speed * Time.deltaTime;
        
        if (position.x < screenConfig.left + ballConfig.radius || position.x > screenConfig.right - ballConfig.radius)
        {
            direction.x *= -1;
            position.x = Mathf.Clamp(position.x, screenConfig.left + ballConfig.radius, screenConfig.right - ballConfig.radius);
        }
        
        if (position.y > screenConfig.up - ballConfig.radius)
        {
            direction.y *= -1;
            position.y = screenConfig.up - ballConfig.radius;
        }
        
        if (Collisions.CheckCollisions(position, ballConfig.radius))
        {
            float offset = position.x - PaddlePhysics.bounds.center.x;
            direction = new Vector3(offset, 1f, 0f).normalized;
            position.y = PaddlePhysics.bounds.yMax + ballConfig.radius;
        }
        
        if (position.y < screenConfig.down)
        {
            direction = Vector3.zero;
            Debug.Log("You lose");
        }

        ball.position = position;
    }
}