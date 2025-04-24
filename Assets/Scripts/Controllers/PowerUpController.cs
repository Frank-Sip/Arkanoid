using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PowerUpController : MonoBehaviour
{
    [SerializeField] public PowerUpSO powerUpSO;
    [SerializeField] private ScreenEdgesSO screenEdgesSO;

    private PowerUpPhysics physics;
    private bool isSubscribed = false;

    private void Awake()
    {
        physics = new PowerUpPhysics();
        AudioManager audioMgr = FindObjectOfType<AudioManager>();
        physics.Initiate(transform, powerUpSO, screenEdgesSO, this, audioMgr);

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
            case PowerUpType.WidePaddle:
                ActivateWidePaddle();
                break;
            case PowerUpType.ExtraLife:
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

    private void ActivateWidePaddle()
    {
        PaddleController paddleController = FindObjectOfType<PaddleController>();
        if (paddleController != null)
        {
            paddleController.ActivateWidePaddle(1.5f, 5f);
            Debug.Log("Power-up Wide Paddle activado");
        }
        else
        {
            Debug.LogWarning("No se encontró el PaddleController para activar Wide Paddle");
        }
    }

    private void ResetPowerUp()
    {
        gameObject.SetActive(false);
    }
}