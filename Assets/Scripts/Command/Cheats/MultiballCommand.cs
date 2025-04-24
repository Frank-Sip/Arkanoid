using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Multiball", menuName = "Commands/Multiball", order = 0)]
public class MultiballCommand : CommandSO
{
    public override void Execute()
    {
        int currentBalls = BallManager.GetActiveBalls().Count;
        int maxBalls = BallManager.GetMaxBalls();

        if (currentBalls >= maxBalls)
        {
            Debug.Log($"Máximo de bolas alcanzado ({maxBalls}). No se pueden crear más bolas.");
            return;
        }
        
        int ballsToAdd = Mathf.Min(3, maxBalls - currentBalls);

        BallManager.SpawnAndLaunchMultipleBalls(ballsToAdd);
    }
}