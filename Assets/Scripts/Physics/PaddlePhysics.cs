using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PaddlePhysics
{
    private static Transform paddle;
    private static Transform visual;
    private static PaddleSO paddleConfig;
    private static ScreenEdgesSO screenConfig;

    public static Rect bounds;

    public static void Initiate(Transform paddleTransform, Transform visualTransform, PaddleSO paddleSO, ScreenEdgesSO screenSO)
    {
        paddle = paddleTransform;
        visual = visualTransform;
        paddleConfig = paddleSO;
        screenConfig = screenSO;

        Mesh mesh = visual.GetComponent<MeshFilter>()?.sharedMesh;
        if (mesh != null)
        {
            Bounds meshBounds = mesh.bounds;

            float baseWidth = meshBounds.size.x;
            float baseHeight = meshBounds.size.y;

            float scaleX = paddleConfig.width / baseWidth;
            float scaleY = paddleConfig.height / baseHeight;

            visual.localScale = new Vector3(scaleX, scaleY, 1f);

            // Centrado automático usando el centro del mesh
            Vector3 centerOffset = Vector3.Scale(meshBounds.center, visual.localScale);
            visual.localPosition = -centerOffset;

            visual.localRotation = Quaternion.identity;
        }
        else
        {
            // Fallback si no hay mesh
            visual.localScale = new Vector3(paddleSO.width, paddleSO.height, 1f);
            visual.localPosition = Vector3.zero;
        }
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
            paddle.position.y - paddleConfig.height / 2f,
            paddleConfig.width,
            paddleConfig.height
        );
    }
}