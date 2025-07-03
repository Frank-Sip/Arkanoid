using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WidePaddle", menuName = "Commands/Next", order = 1)]
public class WinCommand : CommandSO
{
    public override void Execute()
    {
        ServiceProvider.GetService<ConsoleManager>().ToggleConsole();
        GameManager.Instance.ChangeGameStatus(new GameplayState());
        
        var bricksCopy = new List<BrickController>(BrickManager.GetActiveBricks());
        foreach (var brick in bricksCopy)
        {
            if (brick != null)
            {
                brick.OnDestroyBrick();
            }
        }
        
        BrickManager.CheckGameCondition();
    }
}
