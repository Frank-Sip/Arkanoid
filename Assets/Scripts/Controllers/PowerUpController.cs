using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType
{
    Multiball,
    ExtraLife,
    WidePaddle
}

public class PowerUpController : MonoBehaviour
{
    [SerializeField] private PowerUpSO powerUpSO;
    [SerializeField] private ScreenEdgesSO screenEdgesSO;

    private PowerUpPhysics physics;
    private bool isSubscribed = false;

    private void Awake()
    {
        physics = new PowerUpPhysics();
        physics.Initiate(transform, powerUpSO, screenEdgesSO, this);

        if (!isSubscribed)
        {
            EventManager.OnReset += ResetPowerUp;
            isSubscribed = true;
        }
    }

    public void Frame()
    {
        physics.Frame();
    }

    public void CollideWithPaddle()
    {
        ActivatePowerUp();
        DestroyPowerUp();
    }

    public void DestroyPowerUp()
    {
        PowerUpPool.Instance.ReturnToPool(this);
    }

    private void ActivatePowerUp()
    {
        switch (powerUpSO.powerUpType)
        {
            case PowerUpType.Multiball:
                ActivateMultiball();
                break;
            case PowerUpType.ExtraLife:
                // Implementación futura
                break;
            case PowerUpType.WidePaddle:
                // Implementación futura
                break;
        }
    }

    private void ActivateMultiball()
    {
        int currentBalls = BallManager.GetActiveBalls().Count;
        int maxBalls = BallManager.GetMaxBalls();
        
        if (currentBalls >= maxBalls)
        {
            Debug.Log($"Máximo de bolas alcanzado ({maxBalls}). No se pueden crear más bolas.");
        }
        
        // Intenta lanzar 3 bolas inmediatamente, respetando el límite interno
        BallManager.SpawnAndLaunchMultipleBalls(3);
    }

    private void ResetPowerUp()
    {
        gameObject.SetActive(false);
    }
}