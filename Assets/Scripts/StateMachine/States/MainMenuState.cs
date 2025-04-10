using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuState : GameState
{
    public override void Enter(GameManager gameManager)
    {
        Time.timeScale = 0f;
        Debug.Log("Press any key to Play");
    }

    public override void Tick(GameManager gameManager)
    {
        if (Input.anyKeyDown)
        {
            gameManager.ChangeGameStatus(new GameplayState());
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public override void Exit(GameManager gameManager)
    {
        Debug.Log("Leaving Main Menu");
    }
}
