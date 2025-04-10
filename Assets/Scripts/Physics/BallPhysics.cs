using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPhysics
{
    private Transform ball;
    private BallSO ballConfig;
    private ScreenEdgesSO screenConfig;
    private BallController ballController;

    private float radius => ballConfig.radius;
    private float speed => ballConfig.speed;

    public void Initiate(Transform t, BallSO ballSO, ScreenEdgesSO screenSO, BallController controller)
    {
        ball = t;
        ballConfig = ballSO;
        screenConfig = screenSO;
        ballController = controller;

        ApplyScaleAndCenterMesh();
    }

    private void ApplyScaleAndCenterMesh()
    {
        MeshRenderer meshRenderer = ball.GetComponentInChildren<MeshRenderer>();
        if (meshRenderer != null)
        {
            Vector3 meshSize = meshRenderer.bounds.size;
            float maxDimension = Mathf.Max(meshSize.x, meshSize.y, meshSize.z);
            float scaleFactor = (radius * 2f) / maxDimension;

            ball.localScale = Vector3.one * scaleFactor;

            Vector3 offset = meshRenderer.localBounds.center;
            ball.localPosition -= offset * scaleFactor;
        }
    }

    public static Vector3 GetInitialDirection()
    {
        float x = Random.Range(-1f, 1f);
        float y = Mathf.Sqrt(1f - x * x);
        return new Vector3(x, y, 0f).normalized;
    }

    public void Frame()
    {
        if (ball == null) return;

        Vector3 position = ball.position;
        Vector3 direction = ballController.Direction;

        position += direction.normalized * speed * Time.deltaTime;

        if (PaddlePhysics.CheckCollision(position, radius, ref direction, out Vector3 correction))
        {
            position += correction;
        }
        else if (position.x < screenConfig.left + radius || position.x > screenConfig.right - radius)
        {
            direction.x *= -1;
            position.x = Mathf.Clamp(position.x, screenConfig.left + radius, screenConfig.right - radius);
        }
        else if (BrickPhysics.CheckCollision(position, radius, ref direction, out Vector3 brickCorrection))
        {
            position += brickCorrection;
        }
        else if (CheckBallToBallCollision(ref position, ref direction, radius, ballController))
        {
            //Already handled, leave it empty
        }
        else if (position.y > screenConfig.up - radius)
        {
            direction.y *= -1;
            position.y = screenConfig.up - radius;
        }
        else if (position.y < screenConfig.down)
        {
            ballController.DestroyBall();
            return;
        }

        ball.position = position;
        ballController.Direction = direction;
    }

    private bool CheckBallToBallCollision(ref Vector3 position, ref Vector3 direction, float radius, BallController self)
    {
        foreach (var other in BallManager.GetBalls())
        {
            if (other == null || other == self) continue;

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