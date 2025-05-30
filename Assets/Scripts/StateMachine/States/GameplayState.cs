using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayState : GameState
{
    public override void Enter(GameManager gameManager)
    {
        Time.timeScale = 1f;
        gameManager.GameStateLayout.SetActive(true);
        gameManager.AudioManager.PlayBGM(1);
    }

    public override void Tick(GameManager gameManager)
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.ChangeGameStatus(new PauseState());
        }
        
        foreach (var ball in BallManager.GetBalls())
        {
            if (ball != null && ball.gameObject.activeInHierarchy)
            {
                ball.Frame();
            }
        }
    }


    public override void Exit(GameManager gameManager)
    {
        gameManager.GameStateLayout.SetActive(false);
    }
}
