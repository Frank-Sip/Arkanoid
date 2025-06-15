using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPhysics
{
    private Transform ball;
    private BallSO ballConfig;
    private ScreenEdgesSO screenConfig;
    private BallController ballController;
    private AudioManager audioManager;

    private float radius => ballConfig.radius;
    private float speed => ballConfig.speed;

    public void Initiate(Transform t, BallSO ballSO, ScreenEdgesSO screenSO, BallController controller, AudioManager audio)
    {
        ball = t;
        ballConfig = ballSO;
        screenConfig = screenSO;
        ballController = controller;
        audioManager = audio;

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
    }    public static Vector3 GetInitialDirection()
    {
        float x = Random.Range(-0.5f, 0.5f);  
        float y = Mathf.Sqrt(1f - x * x);
        return new Vector3(x, y, 0f).normalized;
    }

    public void Frame()
    {
        if (ball == null) return;

        Vector3 position = ball.position;
        Vector3 direction = ballController.Direction;

        position += direction.normalized * speed * Time.deltaTime;        if (PaddlePhysics.CheckCollision(position, radius, ref direction, out Vector3 correction))
        {
            position += correction;
            position = new Vector3(position.x, position.y, 0f);
            direction = new Vector3(direction.x, direction.y, 0f);
            audioManager.PlaySFX(0);
            ServiceProvider.GetService<UIManager>().IncrementCounter("PaddleHits");
        }else if (position.x < screenConfig.left + radius || position.x > screenConfig.right - radius)
        {
            direction.x *= -1;
            direction = new Vector3(direction.x, direction.y, 0f);
            position.x = Mathf.Clamp(position.x, screenConfig.left + radius, screenConfig.right - radius);
            position = new Vector3(position.x, position.y, 0f);
            audioManager.PlaySFX(0);
        }        else if (BrickPhysics.CheckCollision(position, radius, ref direction, out Vector3 brickCorrection))
        {
            position += brickCorrection;
            position = new Vector3(position.x, position.y, 0f);
            direction = new Vector3(direction.x, direction.y, 0f);
            audioManager.PlaySFX(1);
        }
        else if (CheckBallToBallCollision(ref position, ref direction, radius, ballController))
        {
            
        }        else if (position.y > screenConfig.up - radius)
        {
            direction.y *= -1;
            direction = new Vector3(direction.x, direction.y, 0f);
            position.y = screenConfig.up - radius;
            position = new Vector3(position.x, position.y, 0f);
            audioManager.PlaySFX(0);
        }
        else if (position.y < screenConfig.down + radius)
        {
            ballController.DestroyBall();
            audioManager.PlaySFX(2);
            return;
        }        ball.position = new Vector3(position.x, position.y, 0f);
        ballController.Direction = new Vector3(direction.x, direction.y, 0f);
    }

    private bool CheckBallToBallCollision(ref Vector3 position, ref Vector3 direction, float radius, BallController self)
    {
        foreach (var other in BallManager.GetBalls())
        {
            if (other == null || other == self || !other.target.gameObject.activeSelf) continue;

            Vector3 otherPos = other.target.transform.position;
            Vector3 delta = otherPos - position;
            float dist = delta.magnitude;
            float combinedRadius = radius * 2f;            if (dist < combinedRadius && dist > 0.0001f)
            {
                Vector3 normal = delta.normalized;
                normal = new Vector3(normal.x, normal.y, 0f).normalized;

                Vector3 thisDir = direction;
                Vector3 otherDir = other.Direction;

                direction = Vector3.Reflect(thisDir, normal);
                other.Direction = Vector3.Reflect(otherDir, -normal);
                
                direction = new Vector3(direction.x, direction.y, 0f);
                other.Direction = new Vector3(other.Direction.x, other.Direction.y, 0f);

                float penetration = combinedRadius - dist;
                Vector3 correction = normal * (penetration / 2f);
                correction = new Vector3(correction.x, correction.y, 0f);
                position -= correction;
                position = new Vector3(position.x, position.y, 0f);
                
                Vector3 otherCorrectedPos = other.target.transform.position + correction;
                other.target.transform.position = new Vector3(otherCorrectedPos.x, otherCorrectedPos.y, 0f);
                
                audioManager.PlaySFX(0);
                
                return true;
            }
        }

        return false;
    }
}