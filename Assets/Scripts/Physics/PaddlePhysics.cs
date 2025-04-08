using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PaddlePhysics
{
    private static Transform paddle;
    private static PaddleSO paddleConfig;
    private static ScreenEdgesSO screenConfig;

    public static Rect bounds;

    public static void Initiate(Transform t, PaddleSO paddleSO, ScreenEdgesSO screenSO)
    {
        paddle = t;
        paddleConfig = paddleSO;
        screenConfig = screenSO;

        paddle.localScale = new Vector3(paddleConfig.width, paddleConfig.height, 1f);
    }

    public static void Frame()
    {
        if (paddle == null) return;

        float input = Input.GetAxisRaw("Horizontal");
        Vector3 position = paddle.position;
        position += Vector3.right * input * paddleConfig.speed * Time.deltaTime;
        
        float halfWidth = paddleConfig.width / 2f;
        position.x = Mathf.Clamp(position.x, screenConfig.left + halfWidth, screenConfig.right - halfWidth);

        paddle.position = position;

        bounds = new Rect(
            paddle.position.x - halfWidth,
            paddle.position.y - (paddleConfig.height / 2f),
            paddleConfig.width,
            paddleConfig.height
        );
    }
}
