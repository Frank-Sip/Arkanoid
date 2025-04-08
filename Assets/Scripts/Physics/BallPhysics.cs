using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BallPhysics
{
    private static Transform ball;
    private static Vector3 direction = new Vector3(1, 1, 0);
    private static float speed = 15f;
    private static float radius = 0.25f;

    public static void Inititate(Transform t)
    {
        ball = t;
    }

    public static void Frame()
    {
        if (ball == null)
        {
            return;
        }

        //Ball-Screen collisions
        Vector3 position = ball.position;
        position += direction.normalized * speed * Time.deltaTime;

        if (position.x < -8f + radius || position.x > 8f - radius)
        {
            direction.x *= -1;
            position.x = Mathf.Clamp(position.x, -8f + radius, 8f - radius);
        }

        if (position.y > 4.5f - radius)
        {
            direction.y *= -1;
            position.y = 4.5f - radius;
        }

        //Ball-Paddle collisions
        Rect ballRect = new Rect(position.x - radius, position.y - radius, radius * 2, radius * 2);
        
        if (Collisions.CheckCollisions(position, radius))
        {
            float offset = position.x - PaddlePhysics.bounds.center.x;
            direction = new Vector3(offset, 1f, 0f).normalized;

            position.y = PaddlePhysics.bounds.yMax + radius;
        }

        if (position.y < -13f)
        {
            direction = Vector3.zero;
            Debug.Log("You lose");
        }

        ball.position = position;
    }
}