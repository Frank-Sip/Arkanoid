using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefeatState : GameState
{
    public override void Enter(GameManager gameManager)
    {
        Debug.Log("Defeat...");
        Time.timeScale = 0f;
    }

    public override void Tick(GameManager gameManager)
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            /*gameManager.ChangeGameStatus(new MainMenuState());
            GameManager.ReloadGame();*/
            
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public override void Exit(GameManager gameManager)
    {
        Debug.Log("Leaving DefeatState");
    }
}