using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameState : GameState
{
    public override void Enter(GameManager gameManager)
    {
        SceneManager.LoadScene("Level 1");
        Time.timeScale *= 0;
        Debug.Log("Press Space to Start");
    }

    public override void Update(GameManager gameManager)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameManager.ChangeGameStatus(new GameplayState());
        }
    }

    public override void Exit(GameManager gameManager)
    {
        Debug.Log("Leaving StartGameState");
    }
}
