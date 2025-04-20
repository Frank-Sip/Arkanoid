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
        for (int i = 0; i < activePowerUps.Count; i++)
        {
            activePowerUps[i].Frame();
        }
    }

    public static void ResetAll()
    {
        foreach (var powerUp in activePowerUps)
        {
            if (powerUp != null)
                powerUp.gameObject.SetActive(false);
        }

        activePowerUps.Clear();
    }
}