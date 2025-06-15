using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Multiball", menuName = "Commands/Multiball", order = 0)]
public class MultiballCommand : CommandSO
{
    public override void Execute()
    {
        BallManager.SpawnAndLaunchMultipleBalls(2);
    }
}