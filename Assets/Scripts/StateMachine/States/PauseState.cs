using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseState : GameState
{
    public override void Enter(GameManager gameManager)
    {
        Debug.Log("Press Esc to resume");
        Time.timeScale = 0f;
    }

    public override void Update(GameManager gameManager)
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.ChangeGameStatus(new GameplayState());
        }
    }

    public override void Exit(GameManager gameManager)
    {
        Debug.Log("Leaving Pause");
    }
}
