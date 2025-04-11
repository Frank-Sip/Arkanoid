using System.Collections.Generic;
using System.Linq;
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
        var powerUpsToUpdate = activePowerUps.ToList();
        
        for (int i = powerUpsToUpdate.Count - 1; i >= 0; i--)
        {
            var powerUp = powerUpsToUpdate[i];
            if (powerUp != null && powerUp.gameObject.activeInHierarchy)
            {
                powerUp.Frame();
            }
        }
        
        activePowerUps.RemoveAll(p => p == null || !p.gameObject.activeInHierarchy);
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