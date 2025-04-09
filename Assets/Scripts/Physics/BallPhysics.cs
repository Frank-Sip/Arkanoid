using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BallPhysics
{
    private static Transform ball;
    private static Vector3 direction = new Vector3(1, 1, 0);
    private static BallSO ballConfig;
    private static ScreenEdgesSO screenConfig;
    private static BallController ballController;

    public static void Initiate(Transform t, BallSO ballSO, ScreenEdgesSO screenSO, BallController controller)
    {
        ResetBallDirection();
        
        ball = t;
        ballConfig = ballSO;
        screenConfig = screenSO;
        ballController = controller;

        ball.localScale = Vector3.one * (ballConfig.radius * 2f);
    }
    
    public static Vector3 GetInitialDirection()
    {
        float x = Random.value < 0.5f ? -1f : 1f;
        return new Vector3(x, -1f, 0f).normalized;
    }

    public static void ResetBallDirection()
    {
        float x = Random.value < 0.5f ? -1f : 1f;
        direction = new Vector3(x, -1f, 0);
    }

    public static void Frame(Transform ball, BallSO ballConfig, ScreenEdgesSO screenConfig, BallController ballController)
    {
        if (ball == null) return;

        float radius = ballConfig.radius;
        float speed = ballConfig.speed;

        Vector3 position = ball.position;
        Vector3 direction = ballController.Direction;

        position += direction.normalized * speed * Time.deltaTime;

        if (Collisions.CheckCollisions(position, radius)) //Ball-Paddle collision
        {
            float offset = position.x - PaddlePhysics.bounds.center.x;
            direction = new Vector3(offset, 1f, 0f).normalized;
            position.y = PaddlePhysics.bounds.yMax + radius;
        }
        else if (position.x < screenConfig.left + radius || position.x > screenConfig.right - radius) //Ball-Walls collision
        {
            direction.x *= -1;
            position.x = Mathf.Clamp(position.x, screenConfig.left + radius, screenConfig.right - radius);
        }
        else if (BrickPhysics.CheckCollision(position, radius)) //Ball-Brick collision
        {
            direction.y *= -1;
        }
        else if (CheckBallToBallCollision(ref position, ref direction, radius, ballController))
        {
            
        }
        else if (position.y > screenConfig.up - radius) //Ball-Top collision
        {
            direction.y *= -1;
            position.y = screenConfig.up - radius;
        }
        else if (position.y < screenConfig.down) //Ball-Bottom collision
        {
            ballController.DestroyBall();
            return;
        }

        ball.position = position;
        ballController.Direction = direction;
    }
    
    private static bool CheckBallToBallCollision(ref Vector3 position, ref Vector3 direction, float radius, BallController self)
    {
        foreach (var other in BallManager.GetBalls())
        {
            if (other == null || other == self)
            {
                continue;
            }

            Vector3 otherPos = other.transform.position;
            Vector3 delta = otherPos - position;
            float dist = delta.magnitude;
            float combinedRadius = radius * 2f;

            if (dist < combinedRadius && dist > 0.0001f)
            {
                Vector3 normal = delta.normalized;

                Vector3 thisDir = direction;
                Vector3 otherDir = other.Direction;

                direction = Vector3.Reflect(thisDir, normal);
                other.Direction = Vector3.Reflect(otherDir, -normal);
                
                float penetration = combinedRadius - dist;
                Vector3 correction = normal * (penetration / 2f);
                position -= correction;
                other.transform.position += correction;

                return true;
            }
        }

        return false;
    }
}