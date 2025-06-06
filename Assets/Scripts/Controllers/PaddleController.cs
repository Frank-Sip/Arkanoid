using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PaddleController", menuName = "GameObject/PaddleControllerSO")]
public class PaddleController : ScriptableObject
{
    [SerializeField] private PaddleSO paddleSO;
    [SerializeField] private ScreenEdgesSO screenEdgesSO;
    [SerializeField] private GameObject paddlePrefab;
    [SerializeField] private AtlasApplier atlasApplier;

    private Transform paddleTransform;
    private Transform visual;
    private Vector3 initialPosition;
    private float originalWidth;
    private bool isWidePaddle;
    private float powerUpTimer = -1f;

    public void Init(Transform parent)
    {
        GameObject paddleGO = Instantiate(paddlePrefab, parent);
        paddleTransform = paddleGO.transform;
        visual = paddleTransform.GetChild(0);
        initialPosition = paddleTransform.position;
        originalWidth = paddleSO.width;

        PaddlePhysics.Initiate(paddleTransform, visual, paddleSO, screenEdgesSO);
        
        if (atlasApplier != null)
        {
            atlasApplier.ApplyAtlas(visual.gameObject);
        }
    }

    public void Frame(float deltaTime)
    {
        PaddlePhysics.Frame();

        if (powerUpTimer > 0f)
        {
            powerUpTimer -= deltaTime;
            if (powerUpTimer <= 0f)
            {
                StopWidePaddlePowerUp();
            }
        }
    }

    public void Reset()
    {
        if (paddleTransform != null)
        {
            paddleTransform.position = initialPosition;
        }

        if (isWidePaddle)
        {
            StopWidePaddlePowerUp();
        }
    }

    public void ActivateWidePaddle(float widthMultiplier = 1.5f, float duration = 5f)
    {
        float newWidth = originalWidth * widthMultiplier;
        paddleSO.width = newWidth;
        PaddlePhysics.UpdateWidth(newWidth);
        isWidePaddle = true;
        powerUpTimer = duration;
    }

    public void StopWidePaddlePowerUp()
    {
        paddleSO.width = originalWidth;
        PaddlePhysics.UpdateWidth(originalWidth);
        isWidePaddle = false;
        powerUpTimer = -1f;
    }
}