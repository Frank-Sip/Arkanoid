using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    [SerializeField] private PowerUpSO config;
    [SerializeField] private ScreenEdgesSO screenEdges;

    private PowerUpPhysics physics = new PowerUpPhysics();

    public void Initiate(PowerUpSO newConfig, ScreenEdgesSO screen)
    {
        config = newConfig;
        screenEdges = screen;
        physics.Initiate(transform, config, screenEdges, this);
    }

    public void Frame()
    {
        physics.Frame();
    }

    public void Activate()
    {
        PowerUpManager.Unregister(this);
        gameObject.SetActive(false);
    }

    public void Deactivate()
    {
        PowerUpManager.Unregister(this);
        gameObject.SetActive(false);
    }
}