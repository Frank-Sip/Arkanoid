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
    private RectTransform[] parallaxLayers;
    private Vector2[] parallaxInitialPositions;
    private ParallaxConfigSO parallaxConfig;
    
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
        
        UpdateParallax();
    }
    
    public void SetupParallax(ParallaxConfigSO config, RectTransform[] layers)
    {
        parallaxConfig = config;
        parallaxLayers = layers;
    
        if (parallaxLayers != null && parallaxLayers.Length == 3)
        {
            parallaxInitialPositions = new Vector2[3];
            for (int i = 0; i < 3; i++)
            {
                if (parallaxLayers[i] != null)
                {
                    parallaxInitialPositions[i] = parallaxLayers[i].anchoredPosition;
                }
            }
        }
    }
    
    private void UpdateParallax()
    {
        if (parallaxConfig == null || parallaxLayers == null || paddleTransform == null)
            return;
    
        float paddleX = paddleTransform.position.x;

        if (parallaxLayers[0] != null)
        {
            float offset = paddleX * parallaxConfig.backgroundSpeed; parallaxLayers[0].anchoredPosition = new Vector2(parallaxInitialPositions[0].x + offset, parallaxLayers[0].anchoredPosition.y);
        }

        if (parallaxLayers[1] != null)
        {
            float offset = paddleX * parallaxConfig.middleSpeed; parallaxLayers[1].anchoredPosition = new Vector2(parallaxInitialPositions[1].x + offset, parallaxLayers[1].anchoredPosition.y);
        }

        if (parallaxLayers[2] != null)
        {
            float offset = paddleX * parallaxConfig.foregroundSpeed;
            parallaxLayers[2].anchoredPosition = new Vector2(parallaxInitialPositions[2].x + offset, parallaxLayers[2].anchoredPosition.y);
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