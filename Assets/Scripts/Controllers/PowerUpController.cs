using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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
        StartCoroutine(DelayedActivation());
    }

    private IEnumerator DelayedActivation()
    {
        yield return null;
        
        ActivatePowerUp();
        
        DestroyPowerUp();
    }

    public void DestroyPowerUp()
    {
        PowerUpManager.Unregister(this);
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
                break;
            case PowerUpType.WidePaddle:
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
            return;
        }
        
        int ballsToAdd = Mathf.Min(3, maxBalls - currentBalls);
        Debug.Log($"Activando multiball con {ballsToAdd} bolas nuevas");
        
        BallManager.SpawnAndLaunchMultipleBalls(ballsToAdd);
    }

    private void ResetPowerUp()
    {
        gameObject.SetActive(false);
    }
}