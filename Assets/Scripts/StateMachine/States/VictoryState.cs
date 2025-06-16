using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryState : GameState
{
    public override void Enter(GameManager gameManager)
    {
        Time.timeScale = 0f;
        gameManager.VictoryLayout.SetActive(true);
        var audioManager = ServiceProvider.GetService<AudioManager>();
        audioManager.PlayBGM(2);
    }

    public override void Tick(GameManager gameManager)
    {
        
    }

    public override void Exit(GameManager gameManager)
    {
        gameManager.VictoryLayout.SetActive(false);
    }
}