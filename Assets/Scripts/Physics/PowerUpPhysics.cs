using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpPhysics
{
    private Transform powerUp;
    private PowerUpSO powerUpConfig;
    private ScreenEdgesSO screenConfig;
    private PowerUpController powerUpController;

    private float radius => powerUpConfig.radius;
    private float speed => powerUpConfig.speed;

    public void Initiate(Transform t, PowerUpSO powerUpSO, ScreenEdgesSO screenSO, PowerUpController controller)
    {
        powerUp = t;
        powerUpConfig = powerUpSO;
        screenConfig = screenSO;
        powerUpController = controller;

        ApplyScaleAndCenterMesh();
    }

    private void ApplyScaleAndCenterMesh()
    {
        MeshRenderer meshRenderer = powerUp.GetComponentInChildren<MeshRenderer>();
        if (meshRenderer != null)
        {
            Vector3 meshSize = meshRenderer.bounds.size;
            float maxDimension = Mathf.Max(meshSize.x, meshSize.y, meshSize.z);
            float scaleFactor = (radius * 2f) / maxDimension;

            powerUp.localScale = Vector3.one * scaleFactor;

            Vector3 offset = meshRenderer.localBounds.center;
            powerUp.localPosition -= offset * scaleFactor;
        }
    }

    public void Frame()
    {
        if (powerUp == null) return;

        Vector3 position = powerUp.position;
        
        position += Vector3.down * speed * Time.deltaTime;
        
        Vector3 direction = Vector3.zero;
        if (PaddlePhysics.CheckCollision(position, radius, ref direction, out Vector3 correction))
        {
            position += correction;
            powerUpController.CollideWithPaddle();
        }
        
        else if (position.y < screenConfig.down)
        {
            powerUpController.DestroyPowerUp();
        }

        powerUp.position = position;
    }
}