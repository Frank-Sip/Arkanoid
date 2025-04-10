using System.Collections.Generic;
using UnityEngine;

public static class PowerUpManager
{
    private static readonly List<PowerUpController> activePowerUps = new List<PowerUpController>();

    public static void Register(PowerUpController powerUp)
    {
        if (!activePowerUps.Contains(powerUp))
            activePowerUps.Add(powerUp);
    }

    public static void Unregister(PowerUpController powerUp)
    {
        activePowerUps.Remove(powerUp);
    }

    public static void Frame()
    {
        for (int i = activePowerUps.Count - 1; i >= 0; i--)
        {
            var powerUp = activePowerUps[i];
            if (powerUp != null && powerUp.gameObject.activeInHierarchy)
            {
                powerUp.Frame();
            }
            else
            {
                activePowerUps.RemoveAt(i);
            }
        }
    }

    public static void SpawnPowerUp(Vector3 position)
    {
        var powerUp = PowerUpPool.Instance.SpawnPowerUp(position);
        if (powerUp != null)
        {
            Register(powerUp);
        }
    }
} 