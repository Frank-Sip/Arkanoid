using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpPhysics
{
    private Transform powerUp;
    private PowerUpSO powerUpConfig;
    private ScreenEdgesSO screenConfig;
    private PowerUpController powerUpController;
    private AudioManager audioManager;

    private float radius => powerUpConfig.radius;
    private float speed => powerUpConfig.speed;

    public void Initiate(Transform t, PowerUpSO powerUpSO, ScreenEdgesSO screenSO, PowerUpController controller, AudioManager audioMgr)
    {
        powerUp = t;
        powerUpConfig = powerUpSO;
        screenConfig = screenSO;
        powerUpController = controller;
        audioManager = audioMgr;

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
        
        if (position.y < screenConfig.down - radius)
        {
            powerUpController.DestroyPowerUp();
            return;
        }
        
        Vector3 direction = Vector3.zero;
        if (CheckPaddleCollision(position, radius))
        {
            powerUpController.CollideWithPaddle();
            audioManager.PlaySFX(3);
            return;
        }

        powerUp.position = position;
    }
    
    private bool CheckPaddleCollision(Vector3 powerUpPosition, float powerUpRadius)
    {
        Rect paddleBounds = PaddlePhysics.bounds;
        
        Rect extendedBounds = new Rect(
            paddleBounds.x - powerUpRadius * 0.5f,
            paddleBounds.y - powerUpRadius * 0.5f,
            paddleBounds.width + powerUpRadius,
            paddleBounds.height + powerUpRadius
        );
        
        if (powerUpPosition.x >= extendedBounds.xMin && 
            powerUpPosition.x <= extendedBounds.xMax &&
            powerUpPosition.y >= extendedBounds.yMin && 
            powerUpPosition.y <= extendedBounds.yMax)
        {
            return true;
        }
        
        return false;
    }
}