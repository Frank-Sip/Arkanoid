using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryState : GameState
{
    public override void Enter(GameManager gameManager)
    {
        Debug.Log("Victory!");
        Time.timeScale = 0f;
    }

    public override void Tick(GameManager gameManager)
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameManager.ResetGame();
            gameManager.ChangeGameStatus(new GameplayState());
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public override void Exit(GameManager gameManager)
    {
        Debug.Log("Leaving VictoryState");
    }
}