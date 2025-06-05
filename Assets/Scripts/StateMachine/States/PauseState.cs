using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseState : GameState
{
    public override void Enter(GameManager gameManager)
    {
        Time.timeScale = 0f;
        gameManager.PauseLayout.SetActive(true);

    }

    public override void Tick(GameManager gameManager)
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ServiceProvider.GetService<ConsoleManager>().ToggleConsole();
        }
    }

    public override void Exit(GameManager gameManager)
    {
        gameManager.PauseLayout.SetActive(false);
    }
}
