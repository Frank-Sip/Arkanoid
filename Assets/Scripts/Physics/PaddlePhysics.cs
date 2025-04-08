using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PaddlePhysics
{
    private static Transform paddle;
    private static float speed = 10f;
    public static Rect bounds;

    public static void Initiate(Transform t)
    {
        paddle = t;
    }

    public static void Frame()
    {
        if (paddle == null)
        {
            return;
        }

        float input = Input.GetAxisRaw("Horizontal");
        Vector3 position = paddle.position;
        position += Vector3.right * input * speed * Time.deltaTime;
        position.x = Mathf.Clamp(position.x, -7.5f, 7.5f);
        paddle.position = position;

        bounds = new Rect(paddle.position.x - 1.5f, paddle.position.y - 0.25f, 3f, 0.5f);
    }
}
