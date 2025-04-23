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
            Vector3 centerOffset = Vector3.Scale(meshBounds.center, visual.localScale);
            visual.localPosition = -centerOffset;
            visual.localRotation = Quaternion.identity;
        }
        else
        {
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

        bounds = new Rect(paddle.position.x - halfWidth, paddle.position.y - paddleConfig.height / 2f, paddleConfig.width, paddleConfig.height);
    }

    public static bool CheckCollision(Vector3 ballPos, float radius, ref Vector3 direction, out Vector3 correction)
    {
        Rect ballRect = new Rect(ballPos.x - radius, ballPos.y - radius, radius * 2, radius * 2);
        correction = Vector3.zero;

        if (!bounds.Overlaps(ballRect)) return false;
        
        float hitPoint = (ballPos.x - bounds.x) / bounds.width;
        float hitFactor = (hitPoint - 0.5f) * 2f;
        
        float maxBounceAngle = 75f;
        float angle = Mathf.Deg2Rad * (maxBounceAngle * hitFactor);
        
        direction = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0f).normalized;
        
        correction = Vector3.up * (radius - Mathf.Abs(ballPos.y - bounds.yMax));

        return true;
    }

    public static void UpdateWidth(float newWidth)
    {
        if (paddle == null || visual == null) return;
        
        paddleConfig.width = newWidth;
        
        Mesh mesh = visual.GetComponent<MeshFilter>()?.sharedMesh;
        if (mesh != null)
        {
            Bounds meshBounds = mesh.bounds;
            float baseWidth = meshBounds.size.x;
            float baseHeight = meshBounds.size.y;
            
            float scaleX = newWidth / baseWidth;
            float scaleY = paddleConfig.height / baseHeight;
            
            visual.localScale = new Vector3(scaleX, scaleY, 1f);
            
            Vector3 centerOffset = Vector3.Scale(meshBounds.center, visual.localScale);
            visual.localPosition = -centerOffset;
        }
        else
        {
            visual.localScale = new Vector3(newWidth, paddleConfig.height, 1f);
        }
        
        bounds = new Rect(paddle.position.x - newWidth / 2f, paddle.position.y - paddleConfig.height / 2f, 
                         newWidth, paddleConfig.height);
    }
}