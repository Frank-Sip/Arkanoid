using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PaddleController", menuName = "GameObject/PaddleControllerSO")]
public class PaddleController : ScriptableObject
{
    public int initialLives = 3;
    private int defaultInitialLives = 3;
    private int currentLives;
    
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
    
    private float originalSpeed;
    private bool isSpeedBoosted;
    private float speedBoostTimer = -1f;

    public void Init(Transform parent)
    {
        GameObject paddleGO = Instantiate(paddlePrefab, parent);
        paddleTransform = paddleGO.transform;
        visual = paddleTransform.GetChild(0);
        initialPosition = paddleTransform.position;
        originalWidth = paddleSO.width;
        originalSpeed = paddleSO.speed;
        
        defaultInitialLives = initialLives;
        currentLives = initialLives;

        PaddlePhysics.Initiate(paddleTransform, visual, paddleSO, screenEdgesSO);
        
        if (atlasApplier != null)
        {
            atlasApplier.ApplyAtlas(visual.gameObject);
        }
        
        ServiceProvider.GetService<UIManager>().SetCounterValue("LivesLeft", currentLives);
    }

    public void Frame(float deltaTime)
    {
        PaddlePhysics.Frame();

        if (powerUpTimer > 0f)
        {
            powerUpTimer -= deltaTime;
            
            ServiceProvider.GetService<UIManager>().SetCounterValue("WidePaddleTimer", Mathf.CeilToInt(powerUpTimer));
            
            if (powerUpTimer <= 0f)
            {
                StopWidePaddlePowerUp();
            }
        }

        if (speedBoostTimer > 0f)
        {
            speedBoostTimer -= deltaTime;
            ServiceProvider.GetService<UIManager>().SetCounterValue("SpeedUpTimer", Mathf.CeilToInt(speedBoostTimer));
            
            if (speedBoostTimer <= 0f)
            {
                StopSpeedBoostPowerUp();
            }
        }
    }

    public void AddLife(int value)
    {
        currentLives += value;
        ServiceProvider.GetService<UIManager>().SetCounterValue("LivesLeft", currentLives);
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
        
        if (isSpeedBoosted)
        {
            StopSpeedBoostPowerUp();
        }
        
        initialLives = defaultInitialLives;
        currentLives = initialLives;
        ServiceProvider.GetService<UIManager>().SetCounterValue("LivesLeft", currentLives);
        ServiceProvider.GetService<UIManager>().SetCounterValue("WidePaddleTimer", 0);
        ServiceProvider.GetService<UIManager>().SetCounterValue("SpeedBoostTimer", 0);
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
        ServiceProvider.GetService<UIManager>().SetCounterValue("WidePaddleTimer", Mathf.CeilToInt(duration));
    }

    public void StopWidePaddlePowerUp()
    {
        paddleSO.width = originalWidth;
        PaddlePhysics.UpdateWidth(originalWidth);
        isWidePaddle = false;
        powerUpTimer = -1f;
        ServiceProvider.GetService<UIManager>().SetCounterValue("WidePaddleTimer", 0);
    }
    
    public void ActivateSpeedBoost(float speedMultiplier = 1.5f, float duration = 5f)
    {
        float newSpeed = originalSpeed * speedMultiplier;
        paddleSO.speed = newSpeed;
        isSpeedBoosted = true;
        speedBoostTimer = duration;
        ServiceProvider.GetService<UIManager>().SetCounterValue("SpeedUpTimer", Mathf.CeilToInt(duration));

    }

    public void StopSpeedBoostPowerUp()
    {
        paddleSO.speed = originalSpeed;
        isSpeedBoosted = false;
        speedBoostTimer = -1f;
        ServiceProvider.GetService<UIManager>().SetCounterValue("SpeedUpTimer", 0);

    }
}