using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PaddleController", menuName = "GameObject/PaddleControllerSO")]
public class PaddleController : ScriptableObject
{
    public int initialLives = 3;
    
    [SerializeField] private PaddleSO paddleSO;
    [SerializeField] private ScreenEdgesSO screenEdgesSO;
    [SerializeField] private GameObject paddlePrefab;
    [SerializeField] private AtlasApplier atlasApplier;
    private int currentLives;

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
        currentLives = initialLives;

        PaddlePhysics.Initiate(paddleTransform, visual, paddleSO, screenEdgesSO);
        
        if (atlasApplier != null)
        {
            atlasApplier.ApplyAtlas(visual.gameObject);
        }
        
        ServiceProvider.GetService<UIManager>().SetCounterValue("LivesCounter", currentLives);
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
        
        currentLives = initialLives;
        ServiceProvider.GetService<UIManager>().SetCounterValue("LivesLeft", currentLives);
    }
    
    public void LoseLife()
    {
        currentLives--;
        ServiceProvider.GetService<UIManager>().SetCounterValue("LivesLeft", currentLives);
        
        if (currentLives <= 0)
        {
            GameManager.Instance.ChangeGameStatus(new DefeatState());
        }
    }
    
    public int GetCurrentLives()
    {
        return currentLives;
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