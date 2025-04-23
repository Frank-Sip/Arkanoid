using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayState : GameState
{
    public override void Enter(GameManager gameManager)
    {
        Debug.Log("Entering GameplayState");
        Time.timeScale = 1f;
        gameManager.GameStateLayout.SetActive(true);
    }

    public override void Tick(GameManager gameManager)
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.ChangeGameStatus(new PauseState());
        }
    }

    public override void Exit(GameManager gameManager)
    {
        Debug.Log("Leaving GameplayState");
        gameManager.GameStateLayout.SetActive(false);
    }
}
