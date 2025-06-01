using System.Collections.Generic;
using UnityEngine;

public class GameplayState : GameState
{
    public override void Enter(GameManager gameManager)
    {
        Time.timeScale = 1f;
        gameManager.GameStateLayout.SetActive(true);
        var audioManager = ServiceProvider.GetService<AudioManager>();
        audioManager.PlayBGM(1);
    }

    public override void Tick(GameManager gameManager)
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.ChangeGameStatus(new PauseState());
            return;
        }
        
        var currentBalls = new List<BallController>(BallManager.GetBalls());
        foreach (var ball in currentBalls)
        {
            if (ball != null && ball.target != null && ball.target.gameObject.activeInHierarchy)
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