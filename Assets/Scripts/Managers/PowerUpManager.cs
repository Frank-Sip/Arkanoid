using System.Collections.Generic;
using UnityEngine;

public static class PowerUpManager
{
    private static readonly List<PowerUpController> activePowerUps = new List<PowerUpController>();
    private static int totalPowerUpsSpawned = 0; 
    private static int maxPowerUpsPerGame = 3; 

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
        for (int i = 0; i < activePowerUps.Count; i++)
        {
            activePowerUps[i].Frame();
        }
    }

    public static bool CanSpawnPowerUp()
    {
        return totalPowerUpsSpawned < maxPowerUpsPerGame;
    }

    public static PowerUpController SpawnPowerUp(Vector3 position)
    {
        if (totalPowerUpsSpawned >= maxPowerUpsPerGame)
        {
            Debug.Log($"Límite de power-ups alcanzado ({maxPowerUpsPerGame}). No se generarán más.");
            return null;
        }
        
        totalPowerUpsSpawned++;
        
        PowerUpController powerUp = PowerUpPool.Instance.SpawnPowerUp(position);
        if (powerUp != null)
        {
            Register(powerUp);
            Debug.Log($"Power-up generado ({totalPowerUpsSpawned}/{maxPowerUpsPerGame})");
        }
        
        return powerUp;
    }

    public static void ResetAll()
    {
        foreach (var powerUp in activePowerUps)
        {
            if (powerUp != null)
                powerUp.target.gameObject.SetActive(false);
        }

        activePowerUps.Clear();
    }

    public static void ResetPowerUpCount()
    {
        totalPowerUpsSpawned = 0;
        Debug.Log("Contador de power-ups reseteado");
    }
}