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
        ActivateMultiball();
        DestroyPowerUp();
    }

    public void DestroyPowerUp()
    {
        PowerUpPool.Instance.ReturnToPool(this);
    }

    private void ActivateMultiball()
    {
        BallManager.SpawnMultipleBalls();
    }

    private void ResetPowerUp()
    {
        gameObject.SetActive(false);
    }
}